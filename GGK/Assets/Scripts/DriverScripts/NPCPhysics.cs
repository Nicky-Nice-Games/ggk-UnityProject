using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using System.Globalization;
using UnityEngine;
using UnityEngine.AI;

public class NPCPhysics : NetworkBehaviour
{
    // Keep
    [Header("Do not Change")]
    public Vector3 acceleration; //How fast karts velocity changes        
    public Vector3 movementDirection;
    public Quaternion turning;

    [Header("Navigation")]
    public GameObject parent;
    public KartCheckpoint KC;
    public GameObject destination;
    [SerializeField]
    private int destinationAhead;
    private Vector3 checkpointOffset;
    [SerializeField]
    Vector3 randomizedTarget;
    public bool leftGrounded;
    public bool rightGrounded;
    bool lastLeftGrounded = true;
    bool lastRightGrounded = true;


    [Header("Kart Settings")]
    //acceleration, decceleration
    public float accelerationRate, deccelerationRate, airDeccelerationRate;
    public float minSpeed = 5f;
    public float maxSpeed = 60f;
    public float airTurnSpeed = 30f; //Turning speed in the air, to prevent kart from turning too fast in the air
    public float turnSpeed = 40;
    public float maxSteerAngleTires = 20f; //Multiplier for wheel turning speed    
    public float maxSteeringAngle = 10f; //Maximum steering angle for the steering wheel
    public Transform kartNormal;
    public float gravity = 20;
    float controllerX;
    float controllerZ;

    [Header("Traction Settings")]
    public float tractionCoefficient = 6f;
    public float tractionLerpSpeed = 2f;
    public float currentTraction = 6f; //Current traction value, used for lerping traction force


    [Header("Sphere Collider stuff")]
    public float colliderOffset = 1.69f; //Offset for the sphere collider to position kart correctly
    public Transform spherePosTransform; //Reference to the sphere collider transform
    public Rigidbody sphere;

    [Header("Drift Settings")]
    //To determine drifting stuff in update
    public bool isDrifting;
    bool driftMethodCaller = false;
    bool turboTwisting;
    public float turboTwistWindow;
    float driftTime = 0f;
    public float driftFactor = 2f;
    public float driftTurnMultiplier = 1.5f;
    public float driftFowardCompensation = 1.2f;
    public float minDriftTime = 200f;
    public float driftBoostForce = 1f;
    public float hopForce = 8f;
    public float minDriftSteer = 40f;
    public float boostMaxSpeed = 55f;
    public float driftMaxSpeed = 34f; //Maximum speed when drifting
    [HideInInspector]
    public bool canDrift = true;

    //To determine drifting direction
    bool isDriftingLeft;



    [Header("Air Tricks Settings")]
    public float airTrickForce = 10f; //Force applied when performing an air trick
    public float airTrickBoostForce = 5f; //Boost force applied after landing
    public ParticleSystem airTrickParticles; //Particle system for air tricks
    int airTrickCount;
    bool AirTricking;
    bool airTrickInProgress;


    [Header("Wheel references")]
    //Front tires GO
    public GameObject frontTireR;
    public GameObject frontTireL;
    public GameObject steeringWheel;
    Quaternion baseRotation; //Base rotation of the steering wheel for resetting


    [Header("Reference to the kartModel transform for Animation")]
    public Transform kartModel;

    [Header("Particle Effects")]
    public List<ParticleSystem> particleSystemsBR;
    public List<ParticleSystem> particleSystemsBL;
    public List<ParticleSystem> TireScreechesLtoR;
    public List<ParticleSystem> transitionSparksLtoR;
    public List<Color> turboColors;
    public ParticleSystem driftSparksLeftBack;
    public ParticleSystem driftSparksLeftFront;
    public ParticleSystem driftSparksRightFront;
    public ParticleSystem driftSparksRightBack;
    public List<ParticleSystem> boostFlames;
    int driftTier;
    int currentDriftTier = 0; //To check if we are in the same drift tier or not, so we can change the color of the particles accordingly


    [Header("Raycast Settings")]
    public LayerMask groundLayer;
    // Ground snapping variables
    public bool isGrounded;
    bool attemptingDrift;
    float airTime;
    public float groundCheckDistance = 1.05f;
    public float rotationAlignSpeed = 0.05f;
    public float horizontalOffset = 0.2f; // Horizontal offset for ground check raycast
    [HideInInspector]
    public bool doGroundCheck = true;

    //Tween stuff
    Tween driftRotationTween;
    Tween airTrickTween;
    float driftVisualAngle = 10f;
    float driftTweenDuration = 0.4f;

    //Stun Settings
    [SerializeField]
    bool isStunned;

    [Header("Sound Settings")]
    AudioSource soundPlayer;

    [SerializeField]
    AudioClip driveSound;

    public bool isDriving;

    [Header("Confused Settings")]
    public bool isConfused;
    public float confusedTimer;
    public Transform childNormal;

    private float backingOutTimer = 0f;
    private float backingOutDuration = 1.2f; // seconds
    private bool isBackingOut = false;
    private Vector2 backingOutDirection;

    // Start is called before the first frame update
    void Start()
    {
        float offsetRadius = 8f; 
        checkpointOffset = new Vector3(
            Random.Range(-offsetRadius, offsetRadius),
            0f,
            Random.Range(-offsetRadius, offsetRadius)
        );

        sphere.drag = 0.5f;

        StopParticles();

        baseRotation = steeringWheel.transform.localRotation;

        Transform childTransform = parent.transform.GetChild(1);

        KC = childTransform.GetComponent<KartCheckpoint>();
        if (!IsSpawned)
        {
            MiniMapHud.instance.AddKart(gameObject);
            PlacementManager.instance.AddKart(gameObject, KC);
        }
    }
    public override void OnNetworkSpawn()
    {
        Debug.Log("NetworkNPC");
        Transform childTransform = parent.transform.GetChild(1);
        KC = childTransform.GetComponent<KartCheckpoint>();
        MiniMapHud.instance.AddKart(gameObject);

        if (IsOwner)
        {
            AppearanceSettings settings = GetComponent<AppearanceSettings>();
            settings.SetKartAppearanceRpc(settings.name, settings.color);
        }

        PlacementManager.instance.AddKart(gameObject, KC);
    }

    public void StopParticles()
    {
        //-------------Particles----------------
        foreach (ParticleSystem ps in particleSystemsBR)
        {
            ps.Stop();
        }
        foreach (ParticleSystem ps in particleSystemsBL)
        {
            ps.Stop();
        }
        foreach (ParticleSystem ps in TireScreechesLtoR)
        {
            ps.Stop();
        }
        foreach (ParticleSystem ps in transitionSparksLtoR)
        {
            ps.Stop();
        }
        foreach (ParticleSystem ps in boostFlames)
        {
            ps.Stop();
        }

        airTrickParticles.Stop();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsSpawned)
        {
            if (!IsOwner) return;
        }
        HandleGroundCheck();
        ApplyWheelVisuals();

        transform.position =
            new Vector3(spherePosTransform.transform.position.x,
            spherePosTransform.transform.position.y - colliderOffset,
            spherePosTransform.transform.position.z);

        //------------Movement stuff---------------------

        //Stunned
        if (isStunned)
        { 
            movementDirection = Vector3.zero;
            return;
        }

        //Acceleration
        if (movementDirection.z != 0f && isGrounded)
        {
            //Setting acceleration 
            if ((sphere.velocity.magnitude > maxSpeed) || (isDrifting && sphere.velocity.magnitude > driftMaxSpeed))
            {
                acceleration = Vector3.zero; //If we are going too fast, stop accelerating
            }
            else
            {
                acceleration = kartModel.forward * movementDirection.z * accelerationRate * Time.deltaTime;
            }

        }
        else if (isGrounded)
        {
            //Decceleration
            acceleration *= 1f - (deccelerationRate * Time.fixedDeltaTime);

            //Stop the vehicle once we reach a certain minimum speed
            if (sphere.velocity.magnitude < minSpeed)
            {
                sphere.velocity = Vector3.zero;
                acceleration = Vector3.zero;
            }
        }
        else
        {
            //In the air, decelerating bc of drag
            acceleration *= 1f - (airDeccelerationRate * Time.fixedDeltaTime);
        }

        //------------Turning stuff---------------------

        //to check if we are going backwards...
        float backwardsCheck = Vector3.Dot(transform.forward, sphere.velocity);

        //Applies a turn multiplier when drifting
        float driftTurnSpeed = isDrifting ? turnSpeed * driftTurnMultiplier : turnSpeed;

        //If we are not stationary
        if (!(sphere.velocity == Vector3.zero))
        {
            //Calculate the turning direction based on the movement direction input and multiply it by our turn speed
            float turningDirection = isGrounded ? movementDirection.x * driftTurnSpeed : movementDirection.x * airTurnSpeed;
            //float turningDirection = movementDirection.x * driftTurnSpeed;

            //drifting
            if (driftMethodCaller)
            {
                //Gradually increase traction during drift
                currentTraction = Mathf.Lerp(currentTraction, tractionCoefficient, Time.fixedDeltaTime * tractionLerpSpeed);


                //Keep drifting
                Drift();

                //Recalculate our turningDirection value bc of drifting
                turningDirection = movementDirection.x * driftTurnSpeed;

                // turn influence to apply to turning variable
                turningDirection += isDriftingLeft ? -minDriftSteer : minDriftSteer;

                if (isGrounded && airTime < 1.5f)
                {
                    acceleration *= driftFowardCompensation * Time.deltaTime; //Compensate for the forward force when drifting
                }
                else
                {
                    EndDrift();
                }

            }

            //If we are going backwards, we need to turn in the opposite direction
            if (backwardsCheck < 0)
            {
                //Applying our calculated turning direction to the turning variable
                turning = Quaternion.Euler(0f, -(turningDirection * Time.fixedDeltaTime), 0f);

                EndDrift();
            }
            else
            {
                //Applying our calculated turning direction to the turning variable
                turning = Quaternion.Euler(0f, turningDirection * Time.fixedDeltaTime, 0f);
            }

            if (isGrounded && movementDirection.x != 0f)
            {
                Vector3 turnCompensationForce = kartModel.forward * (accelerationRate * 0.0075f * Mathf.Abs(movementDirection.x));
                sphere.AddForce(turnCompensationForce, ForceMode.Acceleration);

            }

            acceleration = turning * acceleration;
        }
        //If we are not moving, we don't need to turn
        else
        {
            turning = Quaternion.Euler(0f, 0f, 0f);
            EndDrift();
        }

        //Falling down
        if (!isGrounded && !attemptingDrift)
        {
            float airRotationSpeed = 2.0f;

            // Target upright rotation based on Yaw (keep current Y, reset pitch/roll)
            Quaternion targetUpright = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

            // Smoothly rotate the kart's visual and normal upright orientation back to upright
            kartNormal.rotation = Quaternion.Slerp(kartNormal.rotation, targetUpright, Time.deltaTime * airRotationSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetUpright, Time.deltaTime * airRotationSpeed);

            if (AirTricking)
            {

            }
            else
            {
                airTrickCount = 0; //Reset air trick count when not air tricking
            }
        }
        else
        {
            if (AirTricking)
            {
                StartCoroutine(Boost(airTrickBoostForce, 0.5f * airTrickCount)); //Apply boost when landing
                airTrickInProgress = false;
                airTrickTween?.Kill(); // Kill any existing air trick tween
                kartModel.localRotation = Quaternion.identity; // Reset kart model rotation after air trick
                kartModel.localPosition = Vector3.zero; // Reset kart model position after air trick

            }
            AirTricking = false; //Reset air tricking state

        }

        // Apply extra downward force to fall faster
        sphere.AddForce(-kartNormal.up * gravity, ForceMode.Acceleration);

        sphere.AddForce(acceleration, ForceMode.Acceleration);
        transform.rotation = transform.rotation * turning;

        //------------Traction---------------------
        if (isGrounded)
        {
            Vector3 forward = kartNormal.forward;
            Vector3 right = kartNormal.right;

            Vector3 velocity = sphere.velocity;

            //Lateral (sideways) velocity
            float lateralSpeed = Vector3.Dot(velocity, right);
            Vector3 lateralVelocity = right * lateralSpeed;

            //Apply opposite force to simulate traction
            Vector3 tractionForce = -lateralVelocity * currentTraction;

            sphere.AddForce(tractionForce, ForceMode.Acceleration);
        }



        //------------Navigation---------------------
        int destinationID = KC.checkpointId + destinationAhead;
        if (destinationID >= KC.checkpointList.Count)
        {
            destinationID = 0;
        }
        if (KC.checkpointList.Count > 0)
        {
            destination = KC.checkpointList[destinationID];
            Vector3 checkpointForward = destination.transform.forward;
            Vector3 offsetBehindCheckpoint = destination.transform.position - checkpointForward * 2f; // 2 units behind

            randomizedTarget = destination.transform.position + checkpointOffset;
        }

        if (isGrounded)
        {
            Vector3 checkpointForward = destination.transform.forward;
            Vector3 targetPos = destination.transform.position - checkpointForward * 2f;

            Vector3 dirToTarget = targetPos - transform.position;
            dirToTarget.y = 0f;

            if (dirToTarget.magnitude < 1f)
            {
                // You're too close to the target, extend further back so there's a clear direction
                targetPos = destination.transform.position - checkpointForward * 5f;
                dirToTarget = targetPos - transform.position;
                dirToTarget.y = 0f;
            }
            Vector3 localDir = transform.InverseTransformDirection(dirToTarget.normalized);
            movementDirection = new Vector3(localDir.x, 0f, localDir.z);

            // --- Fix for stuck-turning (no forward movement)
            if (isGrounded && movementDirection.z <= 0.05f)
            {
                movementDirection.z = 0.25f;
            }
            CheckRoadEdges();

            //movementDirection = Vector3.ClampMagnitude(localDir, 1f);
            AvoidObstacle();
        }
        Debug.DrawLine(transform.position, transform.position + transform.forward * 3f, Color.green);



    }


    void CheckRoadEdges()
    {
        float edgeRayLength = 5f;
        float raySideOffset = 1.5f;
        float rayForwardOffset = 0.5f;

        Vector3 originLeft = transform.position + (transform.right * -raySideOffset) + (transform.forward * rayForwardOffset) + (transform.up * 1.2f);
        Vector3 originRight = transform.position + (transform.right * raySideOffset) + (transform.forward * rayForwardOffset) + (transform.up * 1.2f);

        RaycastHit hitLeft, hitRight;

        Vector3 rayDir = -transform.up;
        leftGrounded = Physics.Raycast(originLeft, rayDir, out hitLeft, edgeRayLength, groundLayer);
        rightGrounded = Physics.Raycast(originRight, rayDir, out hitRight, edgeRayLength, groundLayer);



        float edgeAvoidStrength = 2f;

        if (!leftGrounded && rightGrounded)
        {
            movementDirection.x += edgeAvoidStrength;
        }
        else if (!rightGrounded && leftGrounded)
        {
            movementDirection.x -= edgeAvoidStrength;
        }
        else if (!leftGrounded && !rightGrounded)
        {
            movementDirection.z = Mathf.Max(movementDirection.z - 0.5f, 0f); // brake/slow
        }

    //movementDirection.x = Mathf.Clamp(movementDirection.x, -1f, 1f);
    }

    void AvoidObstacle()
    {
        float rayForwardLength = 7f;
        float raySideOffset = 1.0f;
        float rayVerticalOffset = 1.2f;
        float avoidStrength = 2f;

        Vector3 rayDir = childNormal.forward;

        Vector3 originCenter = transform.position + (transform.up * rayVerticalOffset);
        Vector3 originLeft = originCenter + (transform.right * -raySideOffset);
        Vector3 originRight = originCenter + (transform.right * raySideOffset);

        bool obstacleCenter = Physics.Raycast(originCenter, rayDir, rayForwardLength, groundLayer);
        bool obstacleLeft = Physics.Raycast(originLeft, rayDir, rayForwardLength, groundLayer);
        bool obstacleRight = Physics.Raycast(originRight, rayDir, rayForwardLength, groundLayer);

        Debug.DrawRay(originCenter, rayDir * rayForwardLength, Color.red);
        Debug.DrawRay(originLeft, rayDir * rayForwardLength, Color.yellow);
        Debug.DrawRay(originRight, rayDir * rayForwardLength, Color.yellow);

        if (obstacleCenter)
        {
            // If center ray hits, try to pick a side based on which side is clearer
            if (!obstacleLeft && obstacleRight)
            {
                movementDirection.x -= avoidStrength;
            }
            else if (!obstacleRight && obstacleLeft)
            {
                movementDirection.x += avoidStrength;
            }
            else
            {
                movementDirection.z -= avoidStrength;
            }

            movementDirection.z = Mathf.Max(movementDirection.z - 0.5f, 0f); // brake slightly
        }
        else if (obstacleLeft && !obstacleRight)
        {
            movementDirection.x += avoidStrength;
        }
        else if (obstacleRight && !obstacleLeft)
        {
            movementDirection.x -= avoidStrength;
        }

        // ========== PERPENDICULAR STUCK CHECK ==========

        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 velocity = sphere.velocity;
        velocity.y = 0;

        float dot = Vector3.Dot(forward, velocity.normalized);

        if (!isBackingOut && Mathf.Abs(dot) < 0.25f && velocity.magnitude < 1f && obstacleCenter)
        {
            isBackingOut = true;
            backingOutTimer = backingOutDuration;

            // Choose a consistent backward + slight left or right offset
            float sideOffset = Random.value > 0.5f ? 0.8f : -0.8f;
            backingOutDirection = new Vector2(sideOffset, -1f); // (x: side, z: reverse)
        }

        if (isBackingOut)
        {
            backingOutTimer -= Time.deltaTime;

            movementDirection.x = backingOutDirection.x;
            movementDirection.z = backingOutDirection.y;


            if (backingOutTimer <= 0f)
            {
                isBackingOut = false;
            }
        }

        movementDirection.x = Mathf.Clamp(movementDirection.x, -1f, 1f);
    }

    void HandleGroundCheck()
    {
        RaycastHit hitNear;

        if (doGroundCheck)
        {
            if (Physics.Raycast(transform.position + (transform.up * .2f), -kartNormal.up, out hitNear, groundCheckDistance, groundLayer))
            {
                airTime = 0f; //Reset air time when grounded
                isGrounded = true;

                //Normal Rotation
                kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * rotationAlignSpeed);
                kartNormal.Rotate(0, transform.eulerAngles.y, 0);

                HandleOffRoad(hitNear);
            }
            else
            {
                airTime += Time.deltaTime; //Keeping track of air time
                isGrounded = false;
            }
        }
    }

    private void HandleOffRoad(RaycastHit hit)
    {
        float slowFactor = 0.35f;
        // Checks if the driver isn't on the Road
        if (!hit.collider.CompareTag("Road"))
        {
            sphere.AddForce(-acceleration * slowFactor, ForceMode.Acceleration);
        }
    }

    void ApplyWheelVisuals()
    {
        float steerAngle = movementDirection.x * maxSteerAngleTires;
        frontTireL.transform.localRotation = Quaternion.Euler(0, steerAngle, 0f);
        frontTireR.transform.localRotation = Quaternion.Euler(0, steerAngle, 0f);
        // Steering Wheel Rotation
        float targetAngle = movementDirection.x * maxSteeringAngle;
        //Quaternion targetRot = Quaternion.Euler(0, targetAngle, 0);
        //steeringWheel.transform.localRotation = Quaternion.Slerp(steeringWheel.transform.localRotation, targetRot, Time.deltaTime * turnSpeed);

        //Vector3 currentEuler = steeringWheel.transform.localEulerAngles;
        //steeringWheel.transform.localEulerAngles = new Vector3(
        //    currentEuler.x,
        //    targetAngle,
        //    currentEuler.z
        //);

        steeringWheel.transform.localRotation = baseRotation * Quaternion.AngleAxis(targetAngle, Vector3.up);
    }

    public IEnumerator Boost(float boostForce, float duration)
    {
        foreach (ParticleSystem ps in boostFlames)
        {
            if (!ps.isPlaying)
            {
                ps.Play();
            }
        }
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            Vector3 boostDirection = Vector3.zero;
            if (sphere.velocity.magnitude < boostMaxSpeed)
            {
                boostDirection = kartNormal.forward * driftBoostForce;
            }

            sphere.AddForce(boostDirection, ForceMode.VelocityChange);
            yield return new WaitForFixedUpdate();
        }

        foreach (ParticleSystem ps in boostFlames)
        {
            if (ps.isPlaying)
            {
                ps.Stop();
            }
        }
    }


    public void Drift()
    {
        if (!isDrifting) return;

        //driftTime += Time.deltaTime;

        // If we are drifting to the direction of the drift we increase the drift time accordingly
        if (isDriftingLeft)
        {
            switch (movementDirection.x)
            {
                //Turning right
                case > 0.1f:
                    driftTime += Time.deltaTime + movementDirection.x;
                    break;
                //Turning left
                case < -0.1f:
                    driftTime += (Time.deltaTime + -movementDirection.x) * 3;
                    break;
                default:
                    driftTime += (Time.deltaTime + 1) * 1.5f;
                    break;
            }

        }
        else
        {
            switch (movementDirection.x)
            {
                //Turning right
                case > 0.1f:
                    driftTime += (Time.deltaTime + movementDirection.x) * 3;
                    break;
                //Turning left
                case < -0.1f:
                    driftTime += (Time.deltaTime + 1) + -movementDirection.x;
                    break;
                default:
                    driftTime += (Time.deltaTime + 1) * 1.5f;
                    break;
            }
        }

        if (movementDirection.z == 0)
        {
            if (turboTwisting)
            {

            }
            else
            {

                StartCoroutine(TurboTwist());
                turboTwisting = true;
            }

        }

        float direction = isDriftingLeft ? -1f : 1f;

        sphere.AddForce(kartModel.right * direction * driftFactor, ForceMode.Acceleration);

        //--------------------Particles----------------                        
        ColorDrift();

        if (isDriftingLeft && driftTier > 0)
        {
            if (!particleSystemsBL[0].isPlaying)
            {
                //Activate left drift sparks when we get the boost
                foreach (ParticleSystem ps in particleSystemsBL)
                {
                    if (!ps.isPlaying)
                    {
                        ps.Play();
                    }
                }
            }

            if (driftTier > currentDriftTier)
            {
                //Adding transition sparks
                transitionSparksLtoR[0].Play();
                transitionSparksLtoR[2].Play();
                transitionSparksLtoR[4].Play();
                transitionSparksLtoR[5].Play();
            }
        }
        else if (!isDriftingLeft && driftTier > 0)
        {

            if (!particleSystemsBR[0].isPlaying)
            {
                //Activate right drift sparks when we get the boost
                foreach (ParticleSystem ps in particleSystemsBR)
                {
                    if (!ps.isPlaying)
                    {
                        ps.Play();
                    }
                }
            }


            if (driftTier > currentDriftTier)
            {

                //Adding transition sparks
                transitionSparksLtoR[1].Play();
                transitionSparksLtoR[3].Play();
                transitionSparksLtoR[6].Play();
                transitionSparksLtoR[7].Play();
            }

        }
        //Tire Screech Particles
        if (isDriftingLeft && !TireScreechesLtoR[0].isPlaying)
        {
            TireScreechesLtoR[0].Play();
            TireScreechesLtoR[2].Play();

            if (TireScreechesLtoR[1].isPlaying)
            {
                TireScreechesLtoR[1].Stop();
                TireScreechesLtoR[3].Stop();
                particleSystemsBR.ForEach(ps => ps.Stop());

            }
        }
        else if (!isDriftingLeft && !TireScreechesLtoR[1].isPlaying)
        {
            TireScreechesLtoR[1].Play();
            TireScreechesLtoR[3].Play();

            if (TireScreechesLtoR[0].isPlaying)
            {
                TireScreechesLtoR[0].Stop();
                TireScreechesLtoR[2].Stop();
                particleSystemsBL.ForEach(ps => ps.Stop());
            }
        }

        currentDriftTier = driftTier;
    }


    public void EndDrift()
    {

        if (!isDrifting) return;


        //---------Boost Types----------------
        if (driftTime > minDriftTime * 3f)
        {
            StartCoroutine(Boost(driftBoostForce, 1.6f));
        }
        else if (driftTime > minDriftTime * 2f)
        {
            StartCoroutine(Boost(driftBoostForce, 0.8f));
        }
        else if (driftTime > minDriftTime)
        {
            StartCoroutine(Boost(driftBoostForce, 0.4f));
        }

        isDrifting = false;
        driftMethodCaller = false;
        driftTime = 0f;

        // Reset traction
        currentTraction = tractionCoefficient;

        // Reset visuals

        driftRotationTween?.Kill();
        driftRotationTween = kartModel.DOLocalRotate(Vector3.zero, driftTweenDuration)
            .SetEase(Ease.InOutSine);
        driftRotationTween = kartModel.DOLocalMoveY(0f, driftTweenDuration / 3f)
            .SetEase(Ease.InOutSine);


        particleSystemsBL.ForEach(ps => ps.Stop());
        particleSystemsBR.ForEach(ps => ps.Stop());
        TireScreechesLtoR.ForEach(ps => ps.Stop());
        transitionSparksLtoR.ForEach(ps => ps.Stop());

    }



    public void ColorDrift()
    {
        Color c = Color.clear;


        if (driftTime > minDriftTime * 3f)
        {
            //red
            c = turboColors[3];
            driftTier = 3;
        }
        else if (driftTime > minDriftTime * 2f)
        {
            //Orange
            c = turboColors[2];
            driftTier = 2;
        }
        else if (driftTime > minDriftTime)
        {
            //Red
            c = turboColors[1];
            driftTier = 1;
        }
        else
        {
            //Default color
            c = turboColors[0];
            driftTier = 0;
        }

        foreach (ParticleSystem ps in particleSystemsBL)
        {
            var main = ps.main;
            main.startColor = c;
        }
        foreach (ParticleSystem ps in particleSystemsBR)
        {
            var main = ps.main;
            main.startColor = c;
        }
        foreach (ParticleSystem ps in TireScreechesLtoR)
        {
            var main = ps.main;
            main.startColor = c;

        }
        foreach (ParticleSystem ps in transitionSparksLtoR)
        {
            var main = ps.main;
            main.startColor = c;
        }

    }

    IEnumerator TurboTwist()
    {
        int TurnCount = 0;

        //Checks for 25 frames if we want to turbo twist
        for (int i = 0; i < 25; i++)
        {
            //Check if player wants to drift the opposite direction
            if (isDriftingLeft && movementDirection.x > 0.1f && movementDirection.z == 0)
            {
                TurnCount++;
            }
            else if (!isDriftingLeft && movementDirection.x < -0.1f && movementDirection.z == 0)
            {
                TurnCount++;
            }

            if (TurnCount > 3)
            {

                break;
            }

            yield return new WaitForFixedUpdate();
        }

        //The closer the value is to 1 from below the closer we are to the minimum drift time
        float recenBoostvalue = Mathf.Abs(driftTime - minDriftTime * currentDriftTier);

        if (TurnCount > 3)
        {
            isDriftingLeft = !isDriftingLeft; //Change the direction of the drift

            if (recenBoostvalue <= turboTwistWindow)
            {
                StartCoroutine(Boost(driftBoostForce, 0.4f));
            }


            //--------------Drift Animation-----------------

            float yRot = isDriftingLeft ? -40f : 40f;
            float zTilt = isDriftingLeft ? 2f : -2f;
            float snapTilt = isDriftingLeft ? -20f : 20f;

            driftRotationTween?.Kill();

            // Main drift lean
            driftRotationTween = DOTween.Sequence()
                .Append(kartModel.DOLocalRotate(new Vector3(0f, yRot * 4f, zTilt), 0.7f).SetEase(Ease.OutBack))    //slide out
                .Join(kartModel.DOLocalRotate(new Vector3(0f, yRot, snapTilt), 0.7f).SetEase(Ease.OutQuart))            //tilt side mid drift
                .Join(kartModel.DOLocalMoveY(0.5f, 0.7f).SetEase(Ease.OutQuart))                                     //subtle elevation                       
                .Append(kartModel.DOLocalRotate(new Vector3(0f, yRot, zTilt), 0.15f).SetEase(Ease.InQuad))          //untilt
                .Join(kartModel.DOLocalMoveY(0f, 0.15f).SetEase(Ease.OutBack))                                    //sutble descend
                .Append(kartModel.DOLocalRotate(new Vector3(0f, yRot / 2f, zTilt), 2.0f).SetEase(Ease.OutSine));       //slide in slightly

        }

        yield return new WaitForSeconds(0.6f);
        turboTwisting = false; //Reset the turbo twisting state
    }

    public void Stun(float duration)
    {
        StopCoroutine(TurboTwist());
        StopCoroutine(Boost(driftBoostForce, 0.4f));

        driftTime = 0f;
        isDrifting = false;
        AirTricking = false;
        airTrickInProgress = false;
        airTrickTween?.Kill();
        driftRotationTween?.Kill();

        StartCoroutine(StunCoroutine(duration));



    }

    IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;

        driftRotationTween = DOTween.Sequence()
            .Append(kartModel.DOLocalRotate(new Vector3(0f, 360f, 0f), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutQuad));

        yield return new WaitForSeconds(duration);

        driftRotationTween?.Kill();
        kartModel.localRotation = Quaternion.identity; // Reset kart model rotation after stun
        isStunned = false;
    }

}
