using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class NEWDriver : NetworkBehaviour
{
    // root reference of the prefab
    public Transform rootTransform;
    public KartCheckpoint kartCheckpoint;

    [Header("Input System Settings")]
    public PlayerInput playerInput;
    public bool STUNBUTTON = false; //To determine if the stun button is pressed or not, used in the input system
    // Keep
    [Header("Do not Change")]
    public Vector3 acceleration; //How fast karts velocity changes        
    public Vector3 movementDirection;
    public Quaternion turning;


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
    public GameObject backTireR;
    public GameObject backTireL;
    public GameObject steeringWheel;
    Quaternion baseRotation; //Base rotation of the steering wheel for resetting
    [HideInInspector]
    public bool turnWheels = true;


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
    public ParticleSystem dazedStars;
    public VFXHandler vfxHandler;
    int driftTier;
    int currentDriftTier = 0; //To check if we are in the same drift tier or not, so we can change the color of the particles accordingly


    [Header("Raycast Settings")]
    public LayerMask groundLayer;
    // Ground snapping variables
    public bool isGrounded;
    public bool attemptingDrift;
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



    // Player info for API
    public PlayerInfo playerInfo;
    private GameManager gameManagerObj;



    // Start is called before the first frame update
    void Start()
    {
        gameManagerObj = FindAnyObjectByType<GameManager>();

        if (gameManagerObj)
        {
            playerInfo = new PlayerInfo(gameManagerObj.playerInfo);
        }

        gameManagerObj.FillMapRaced(this);

        sphere.drag = 0.5f;
        sphere.isKinematic = false;
        StopParticles();


        baseRotation = steeringWheel.transform.localRotation;

        if (!IsSpawned)
        {
            playerInput.enabled = true;
            SpeedCameraEffect.instance.FollowKart(rootTransform);
            SpeedAndTimeDisplay.instance.TrackKart(gameObject);
            MiniMapHud.instance.trackingPlayer = gameObject;
            MiniMapHud.instance.AddKart(gameObject);
            PlacementManager.instance.AddKart(gameObject, kartCheckpoint);
            PlacementManager.instance.TrackKart(kartCheckpoint);
            SpeedLineHandler.instance.trackingPlayer = this;

            playerInput.actions["Pause"].started += FindAnyObjectByType<PauseHandler>(FindObjectsInactive.Include).TogglePause;
        }

    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerInput.enabled = true;
            SpeedCameraEffect.instance.FollowKart(rootTransform);
            SpeedAndTimeDisplay.instance.TrackKart(gameObject);
            PlacementManager.instance.TrackKart(kartCheckpoint);
            //if it's the owner, send its CharacterData settings over to other clients
            AppearanceSettings appearance = gameObject.GetComponent<AppearanceSettings>();
            if (appearance) appearance.SetKartAppearanceRpc(CharacterData.Instance.characterName, CharacterData.Instance.characterColor);

            MiniMapHud.instance.trackingPlayer = gameObject;
            SpeedLineHandler.instance.trackingPlayer = this;

            playerInput.actions["Pause"].started += FindAnyObjectByType<PauseHandler>(FindObjectsInactive.Include).TogglePause;
        }
        if (IsServer)
        {
            // PlacementManager.instance.AddKart(gameObject, kartCheckpoint);
        }
        PlacementManager.instance.AddKart(gameObject, kartCheckpoint);

        MiniMapHud.instance.AddKart(gameObject);

        TwoDimensionalAnimMultiplayer multiplayerAnim = transform.parent.GetComponent<TwoDimensionalAnimMultiplayer>();
        if (multiplayerAnim) multiplayerAnim.driver = this;

        playerInfo.raceStartTime = DateTime.Now;
    }

    public override void OnNetworkDespawn()
    {
        MiniMapHud.instance.RemoveKart(gameObject);
        Debug.Log($"ClientId {OwnerClientId} has despawned/disconnected");
    }

    public void StopParticles()
    {
        ////-------------Particles----------------
        //foreach (ParticleSystem ps in particleSystemsBR)
        //{
        //    ps.Stop();
        //}
        //foreach (ParticleSystem ps in particleSystemsBL)
        //{
        //    ps.Stop();
        //}
        //foreach (ParticleSystem ps in TireScreechesLtoR)
        //{
        //    ps.Stop();
        //}
        //foreach (ParticleSystem ps in transitionSparksLtoR)
        //{
        //    ps.Stop();
        //}
        //foreach (ParticleSystem ps in boostFlames)
        //{
        //    ps.Stop();
        //}
        //
        //airTrickParticles.Stop();

        vfxHandler.StopAllParticles();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleGroundCheck();

        if (turnWheels)
        {
            ApplyWheelVisuals();
        }

        //Follow Collider
        transform.position =
            new Vector3(spherePosTransform.transform.position.x,
            spherePosTransform.transform.position.y - colliderOffset,
            spherePosTransform.transform.position.z);



        //------------Movement stuff---------------------

        //Stunned
        if (isStunned) movementDirection = Vector3.zero;

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

        //------------Confused Timer---------------------
        if (isConfused)
        {
            confusedTimer -= Time.deltaTime;

            if (confusedTimer <= 0)
            {
                isConfused = false;
                movementDirection *= -1; // Just here to forces confusion to activate even if you don't change movement input
            }
        }

        ////DELETE AFTER HAVING STUN WORKING
        //if(STUNBUTTON)
        //{
        //    Stun(2f);
        //    STUNBUTTON = false; // Reset stun button state
        //}
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

    /// <summary>
    /// Begin to make an attempt at drifting
    /// If the conditions for drifiting are met,
    /// you'll move to the next drifting script
    /// </summary>
    public void AttemptDrift()
    {
        if (isGrounded && !attemptingDrift)
        {
            isDrifting = true;
            isGrounded = false;

            StartCoroutine(DriftHopEnabler());

            // Animate visual jump
            driftRotationTween?.Kill(); // Kill any existing drift rotation tween
            driftRotationTween = DOTween.Sequence()
                .Append(kartModel.DOLocalMoveY(1f, 0.1f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad))
                .Append(kartModel.parent.DOScale(new Vector3(1.1f, 0.9f, 1.1f), 0.05f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutSine));

        }
    }

    /// <summary>
    /// Begin the drifting script once the conditions are met.
    /// Allows the kart to hit a sharper angle when turning
    /// </summary>
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

        if (isDriftingLeft)
        {
            if (driftTier > 0)
            {
                vfxHandler.ParticleSystemsL();

                if (driftTier > currentDriftTier)
                {
                    vfxHandler.TierTransitionSparksL();
                }
            }
            vfxHandler.TireScreechesL();
        }
        else if (!isDriftingLeft)
        {
            if (driftTier > 0)
            {
                vfxHandler.ParticleSystemsR();
                if (driftTier > currentDriftTier)
                {
                    vfxHandler.TierTransitionSparksR();
                }
            }
            vfxHandler.TireScreechesR();

        }
        currentDriftTier = driftTier;
    }

    /// <summary>
    /// Ends the specific drift state.
    /// Provides a boost after drifting for a short time
    /// </summary>
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

        vfxHandler.StopDriftVFX();
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

        vfxHandler.ColorDrift(c);

    }

    public void AirTrick()
    {
        AirTricking = true;

        if (!airTrickInProgress)
        {
            if (movementDirection.x > 0.1f)
            {
                airTrickInProgress = true;
                //Perform right air trick
                driftRotationTween?.Kill(); // Kill any existing drift rotation tween

                vfxHandler.PlayAirTrickVFX(false);

                airTrickTween = DOTween.Sequence()
                .Append(kartModel.DOLocalRotate(new Vector3(0f, 0f, -360f), 0.4f, RotateMode.FastBeyond360).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    airTrickInProgress = false;
                    airTrickCount++;
                }));

                //Apply tiny directional force
                Vector3 boostDirection = kartNormal.right * airTrickForce;
                sphere.AddForce(boostDirection, ForceMode.VelocityChange);
            }
            else if (movementDirection.x < -0.1f)
            {
                airTrickInProgress = true;
                //Perform left air trick
                driftRotationTween?.Kill(); // Kill any existing drift rotation tween

                vfxHandler.PlayAirTrickVFX(true);

                airTrickTween = DOTween.Sequence()
                .Append(kartModel.DOLocalRotate(new Vector3(0f, 0f, 360f), 0.4f, RotateMode.FastBeyond360).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    airTrickInProgress = false;
                    airTrickCount++;
                }));

                //Apply tiny directional force
                Vector3 boostDirection = -kartNormal.right * airTrickForce;
                sphere.AddForce(boostDirection, ForceMode.VelocityChange);
            }
            else
            {
                airTrickInProgress = true;
                //Perform default air trick
                driftRotationTween?.Kill(); // Kill any existing drift rotation tween

                airTrickTween = DOTween.Sequence()
                .Append(kartModel.DOLocalRotate(new Vector3(360f, 0f, 0f), 0.4f, RotateMode.FastBeyond360).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    airTrickInProgress = false;
                    airTrickCount++;
                }));


            }
        }
    }

    public IEnumerator Boost(float boostForce, float duration)
    {
        vfxHandler.PlayBoostVFX();
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

        vfxHandler.StopBoostVFX();
    }

    public IEnumerator DriftHopEnabler()
    {
        //Disabling raycast
        attemptingDrift = true;

        int TurnCount = 0;
        bool isInputLeft = false;

        for (int i = 0; i < 22; i++)
        {


            //Check if player wants to drift either direction
            if (Mathf.Abs(movementDirection.x) > 0.1f && Mathf.Abs(sphere.velocity.x) > 5 && isDrifting)
            {
                TurnCount++;
                isInputLeft = movementDirection.x < 0f;

                if (TurnCount > 6 && i > 12)
                {
                    break;
                }
            }
            else if (!isDrifting)
            {
                TurnCount = 0;
                yield return null;
            }
            else if (TurnCount! > 6)
            {
                TurnCount--;
            }

            yield return new WaitForFixedUpdate();
        }
        //Check if player wants to drift either direction
        if (TurnCount > 6)
        {
            driftMethodCaller = true;

            //Resetting traction
            currentTraction = 0f;

            isDriftingLeft = isInputLeft;
            driftTime = 0f;


            //--------------Drift Animation-----------------

            float yRot = isDriftingLeft ? -40f : 40f;
            float zTilt = isDriftingLeft ? 2f : -2f;
            float snapTilt = isDriftingLeft ? -20f : 20f;

            driftRotationTween?.Kill();
            kartModel.localPosition = Vector3.zero; // Reset kart model position before starting the drift animation
            kartModel.localRotation = Quaternion.identity; // Reset kart model rotation before starting the drift animation
            kartModel.localScale = new Vector3(56.31424f, 56.31424f, 56.31424f); // Reset kart model scale before starting the drift animation

            // Main drift lean
            driftRotationTween = DOTween.Sequence()
                .Append(kartModel.DOLocalRotate(new Vector3(0f, yRot * 4f, zTilt), 0.7f).SetEase(Ease.OutBack))    //slide out
                .Join(kartModel.DOLocalRotate(new Vector3(0f, yRot, snapTilt), 0.7f).SetEase(Ease.OutQuart))            //tilt side mid drift
                .Join(kartModel.DOLocalMoveY(0.5f, 0.7f).SetEase(Ease.OutQuart))                                     //subtle elevation                       
                .Append(kartModel.DOLocalRotate(new Vector3(0f, yRot, zTilt), 0.15f).SetEase(Ease.InQuad))          //untilt
                .Join(kartModel.DOLocalMoveY(0f, 0.15f).SetEase(Ease.OutBack))                                    //sutble descend
                .Append(kartModel.DOLocalRotate(new Vector3(0f, yRot / 2f, zTilt), 2.0f).SetEase(Ease.OutSine));       //slide in slightly
        }
        else
        {
            EndDrift();
        }

        attemptingDrift = false;
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

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        // Reverse Inputs
        if (isConfused)
        {
            input *= -1;
        }

        movementDirection = input;

        movementDirection.z = movementDirection.y;

        movementDirection.y = 0; //We are not gonna jump duh

        // determines when driving starts and when driving ends
        if (context.started)
        {
            isDriving = true;
        }
        else if (context.canceled)
        {
            isDriving = false;

            // // stops engine sound
            // if (soundPlayer.isPlaying)
            // {
            //     soundPlayer.Stop();
            // }
        }
    }

    public void OnDrift(InputAction.CallbackContext context)
    {
        if (canDrift)
        {
            if (context.started)
            {
                if (isGrounded)
                {
                    AttemptDrift();
                }
                else if (!isGrounded && !attemptingDrift)
                {
                    AirTrick();
                }
            }
            else if (context.canceled)
            {
                EndDrift();
            }
        }
    }

    public void OnTurn(InputAction.CallbackContext context)
    {
        float input = context.ReadValue<float>();

        // Reverse Inputs
        if (isConfused)
        {
            input *= -1;
        }

        controllerX = input;
        UpdateControllerMovement(context);
    }

    public void OnAcceleration(InputAction.CallbackContext context)
    {
        float input = context.ReadValue<float>();

        // Reverse Inputs
        if (isConfused)
        {
            input *= -1;
        }

        controllerZ = input;
        UpdateControllerMovement(context);

        if (context.started)
        {
            isDriving = true;
        }
        else if (context.canceled)
        {
            isDriving = false;
        }
    }

    public void OnLookBack(InputAction.CallbackContext context)
    {
        SpeedCameraEffect.instance.OnLookBack(context);
    }

    private void UpdateControllerMovement(InputAction.CallbackContext context)
    {
        Vector2 moveInput = new Vector2(controllerX, controllerZ);
        if (moveInput.y > 0)
        {
            moveInput.y = 1;
        }
        else if (moveInput.y < 0)
        {
            moveInput.y = -1;
        }
        else
        {
            moveInput.y = 0;
        }

        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        movementDirection.x = moveInput.x;
        movementDirection.z = moveInput.y;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, sphere.velocity);
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position + (transform.up * 0.2f), -kartNormal.up * groundCheckDistance);
    }

    /// <summary>
    /// Checks if the kart is driving on the road or offroad
    /// If they drive on the offroad they'll be slowed down as needed
    /// </summary>
    private void HandleOffRoad(RaycastHit hit)
    {
        float slowFactor = 0.35f;
        // Checks if the driver isn't on the Road
        if (!hit.collider.CompareTag("Road"))
        {
            sphere.AddForce(-acceleration * slowFactor, ForceMode.Acceleration);
        }
    }

    public void Stun(float duration)
    {
        StopCoroutine(DriftHopEnabler());
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

    public void Recover()
    {
        StopCoroutine(DriftHopEnabler());
        StopCoroutine(TurboTwist());
        StopCoroutine(Boost(driftBoostForce, 0.4f));

        driftTime = 0f;
        isDrifting = false;
        AirTricking = false;
        airTrickCount = 0;
        airTrickInProgress = false;
        airTrickTween?.Kill();
        driftRotationTween?.Kill();
    }

    public void SendThisPlayerData()
    {
        // Debug.Log("Called SendPlayerData from NEWDriver");
        // if (!playerInfo.isGuest && IsOwner)
        // {
        //     SendThisPlayerDataRpc();
        // }

        //client sending their own data
        if (!playerInfo.isGuest) { gameManagerObj.GetComponent<APIManager>().PostPlayerData(playerInfo); }

    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void SendThisPlayerDataRpc()
    {
        gameManagerObj.GetComponent<APIManager>().PostPlayerData(playerInfo);
    }

    // client rpc for each server controlled value that the client needs to know for it's PlayerInfo.cs
    // SendTo.Owner should work
    [Rpc(SendTo.Owner, RequireOwnership = false)]
    public void IncrementFellOffMapRpc()
    {
        playerInfo.fellOffMap++;
    }

    [Rpc(SendTo.Owner, RequireOwnership = false)]
    public void IncrementHazardUsageTier1Rpc()
    {
        playerInfo.defenseUsage["oilSpill1"]++;
    }

    [Rpc(SendTo.Owner, RequireOwnership = false)]
    public void IncrementHazardUsageTier2Rpc()
    {
        playerInfo.defenseUsage["brickwall"]++;
    }

    [Rpc(SendTo.Owner, RequireOwnership = false)]
    public void IncrementHazardUsageTier3Rpc()
    {
        playerInfo.defenseUsage["confuseritchie"]++;
    }

    [Rpc(SendTo.Owner, RequireOwnership = false)]
    public void IncrementHazardUsageTier4Rpc()
    {
        playerInfo.defenseUsage["fakepowerupbrick"]++;
    }

    [Rpc(SendTo.Owner, RequireOwnership = false)]
    public void IncrementDefenseUsageTier1Rpc()
    {
        playerInfo.defenseUsage["defense1"]++;
    }

    [Rpc(SendTo.Owner, RequireOwnership = false)]
    public void IncrementDefenseUsageTier2Rpc()
    {
        playerInfo.defenseUsage["defense2"]++;
    }

    [Rpc(SendTo.Owner, RequireOwnership = false)]
    public void IncrementDefenseUsageTier3Rpc()
    {
        playerInfo.defenseUsage["defense3"]++;
    }

    [Rpc(SendTo.Owner, RequireOwnership = false)]
    public void IncrementDefenseUsageTier4Rpc()
    {
        playerInfo.defenseUsage["defense4"]++;
    }

    [Rpc(SendTo.Owner, RequireOwnership = false)]
    public void IncrementOffenseUsageTier1Rpc()
    {
        playerInfo.offenceUsage["puck1"]++;
    }

    [Rpc(SendTo.Owner, RequireOwnership = false)]
    public void IncrementOffenseUsageTier2Rpc()
    {
        playerInfo.offenceUsage["puck2"]++;
    }

    [Rpc(SendTo.Owner, RequireOwnership = false)]
    public void IncrementOffenseUsageTier3Rpc()
    {
        playerInfo.offenceUsage["puck3"]++;
    }

    [Rpc(SendTo.Owner, RequireOwnership = false)]
    public void IncrementOffenseUsageTier4Rpc()
    {
        playerInfo.offenceUsage["puck4"]++;
    }

    [Rpc(SendTo.Owner, RequireOwnership = false)]
    public void IncrementBoostUsageTier1Rpc()
    {
        playerInfo.boostUsage["speedBoost1"]++;
    }

    [Rpc(SendTo.Owner, RequireOwnership = false)]
    public void IncrementBoostUsageTier2Rpc()
    {
        playerInfo.boostUsage["speedBoost2"]++;
    }

    [Rpc(SendTo.Owner, RequireOwnership = false)]
    public void IncrementBoostUsageTier3Rpc()
    {
        playerInfo.boostUsage["speedBoost3"]++;
    }

    [Rpc(SendTo.Owner, RequireOwnership = false)]
    public void IncrementBoostUsageTier4Rpc()
    {
        playerInfo.boostUsage["speedBoost4"]++;
    }
}