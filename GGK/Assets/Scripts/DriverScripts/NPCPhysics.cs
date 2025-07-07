using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPhysics : MonoBehaviour
{
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
    bool isStunned;

    [Header("Sound Settings")]
    AudioSource soundPlayer;

    [SerializeField]
    AudioClip driveSound;

    public bool isDriving;

    [Header("Confused Settings")]
    public bool isConfused;
    public float confusedTimer;



    // Start is called before the first frame update
    void Start()
    {
        sphere.drag = 0.5f;

        StopParticles();

        baseRotation = steeringWheel.transform.localRotation;
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
        HandleGroundCheck();
        ApplyWheelVisuals();

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
}
