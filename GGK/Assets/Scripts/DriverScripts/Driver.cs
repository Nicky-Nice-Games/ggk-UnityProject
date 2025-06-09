using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Driver : MonoBehaviour
{
    // Keep
    [Header("Do not Change")]
    public Vector3 acceleration; //How fast karts velocity changes
    public Vector3 velocity; //How fast kart is moving
    private Vector3 forward;
    public Vector3 movementDirection;
    public Quaternion turning;

    [Header("Kart Settings")]
    //acceleration, decceleration
    public float accelerationRate, deccelerationRate;
    public float minSpeed, maxSpeed;
    public float turnSpeed;
    public Rigidbody rBody;
    public float maxSteerAngle = 20f; //Multiplier for wheel turning speed

    [Header("Drift Settings")]
    //To determine drifting stuff in update
    [SerializeField]
    public bool isDrifting;
    float driftTime = 0f;
    public float driftFactor = 0.8f;
    public float driftTurnMultiplier = 1.5f;
    public float minDriftTime = 1f;
    public float driftBoostForce = 100f;
    public float hopForce = 8f;
    public float minDriftSteer = 5f;

    //To determine drifting direction
    bool isDriftingLeft;

    [Header("Wheel references")]
    //Front tires GO
    public GameObject frontTireR;
    public GameObject frontTireL;
    //Back tires GO
    public GameObject backTireR;
    public GameObject backTireL;

    [Header("Reference to the kartModel transform for Animation")]
    public Transform kartModel;

    [Header("Raycast Settings")]
    public LayerMask groundLayer;
    public float groundDist = 0.3f;

    // Ground snapping variables
    [SerializeField] Transform modelTransform;
    [SerializeField] Transform playerModelTransform;
    public bool isGrounded;
    public float groundCheckDistance = 8.6f;
    public float suspensionOffset = 6.6f;
    public float rotationAlignSpeed = 5.0f;

    //Tween stuff
    Tween driftRotationTween;
    float driftVisualAngle = 10f;
    float driftTweenDuration = 0.4f;
    bool lastDriftDirectionLeft;

    public float airGravityMultiplier = 2f;
    private Gamemanager gamemanagerObj;

    [Header("Sound Settings")]

    AudioSource soundPlayer;

    [SerializeField]
    AudioClip driveSound;

    public bool isDriving;

    // Start is called before the first frame update
    void Start()
    {
        rBody.drag = 0.5f;
        velocity = Vector3.zero;
        rBody.freezeRotation = true;

        accelerationRate = 7500f;
        deccelerationRate = 1f;
        minSpeed = 1;
        maxSpeed = 280f;
        turnSpeed = 75;


        soundPlayer = GetComponent<AudioSource>();

        gamemanagerObj = FindAnyObjectByType<Gamemanager>();
    }

    void Update()
    {
        // plays sound when kart is moving
        if (isDriving)
        {
            if (!soundPlayer.isPlaying)
            {
                soundPlayer.PlayOneShot(driveSound);
            }
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleGroundCheck();
        ApplyWheelVisuals();

        // Apply extra downward force to fall faster
        rBody.AddForce(Vector3.down * 3000f, ForceMode.Acceleration); // Tune value as needed



        //------------Movement stuff---------------------

        //Acceleration
        if (movementDirection.z != 0f)
        {
            acceleration = forward * (movementDirection.z * accelerationRate * Time.deltaTime);

            velocity += acceleration * Time.fixedDeltaTime;

            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        }
        else
        {
            velocity *= 1f - (deccelerationRate * Time.fixedDeltaTime);

            //Stop the vehicle once we reach a certain minimum speed
            if (velocity.magnitude < minSpeed) velocity = Vector3.zero;
        }


        //------------Turning stuff---------------------

        //to check if we are going backwards...
        float backwardsCheck = Vector3.Dot(transform.forward, velocity);

        //We should rename this var. Applies a turn multiplier when drifting
        float newTurnSpeed = isDrifting ? turnSpeed * driftTurnMultiplier : turnSpeed;

        //If we are not stationary
        if (!(velocity == Vector3.zero))
        {
            //Calculate the turning direction based on the movement direction input and multiply it by our turn speed
            float turningDirection = movementDirection.x * newTurnSpeed;

            //drifting
            if (isDrifting)
            {
                //Keep drifting
                Drift();

                // turn influence to apply to turning variable
                turningDirection += isDriftingLeft ? -minDriftSteer : minDriftSteer;


            }

            //If we are going backwards, we need to turn in the opposite direction
            if (backwardsCheck < 0)
            {
                //Applying our calculated turning direction to the turning variable
                turning = Quaternion.Euler(0f, -(turningDirection * Time.fixedDeltaTime), 0f);
            }
            else
            {
                //Applying our calculated turning direction to the turning variable
                turning = Quaternion.Euler(0f, turningDirection * Time.fixedDeltaTime, 0f);
            }

            velocity = turning * velocity;
        }
        //If we are not moving, we don't need to turn
        else
        {
            turning = Quaternion.Euler(0f, 0f, 0f);
        }

        //Falling down
        if (!isGrounded)
        {


            // Mid air rotation
            float rotationSpeed = 2f;
            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);


            Debug.Log("Float angle diff: " + Quaternion.Angle(transform.rotation, targetRotation));
            if (Quaternion.Angle(transform.rotation, targetRotation) < .5)
            {
                targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }


        //Update the wheel rotations
        float steerAngle = movementDirection.x * maxSteerAngle;

        frontTireL.transform.localRotation = Quaternion.Euler(0, steerAngle, 0f);
        frontTireR.transform.localRotation = Quaternion.Euler(0, steerAngle, 0f);

        //rBody.Move(transform.position + velocity * Time.fixedDeltaTime, transform.rotation * turning);
        // Apply movement
        rBody.velocity = velocity;
        rBody.MoveRotation(transform.rotation * turning);
        transform.rotation = rBody.rotation;
    }



    void HandleGroundCheck()
    {
        Vector3[] wheelPositions = new Vector3[4]
        {
        frontTireR.transform.position,
        frontTireL.transform.position,
        backTireR.transform.position,
        backTireL.transform.position
        };

        Vector3 avgPos = Vector3.zero;
        Vector3 avgNormal = Vector3.zero;
        int hitCount = 0;

        foreach (Vector3 pos in wheelPositions)
        {
            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer))
            {
                avgPos += hit.point;
                avgNormal += hit.normal;
                hitCount++;
            }
        }

        if (hitCount > 0)
        {
            isGrounded = true;
            avgPos /= hitCount;
            avgNormal.Normalize();

            Quaternion targetRot = Quaternion.FromToRotation(transform.up, avgNormal) * transform.rotation;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationAlignSpeed);
        }
        else
        {
            isGrounded = false;
        }

        rBody.drag = isGrounded ? 0.5f : 0.05f;

        // Forward vector along ground
        forward = isGrounded ? Vector3.Cross(transform.right, avgNormal).normalized : transform.forward;

    }

    void ApplyWheelVisuals()
    {
        float steerAngle = movementDirection.x * maxSteerAngle;
        frontTireL.transform.localRotation = Quaternion.Euler(0, steerAngle, 0f);
        frontTireR.transform.localRotation = Quaternion.Euler(0, steerAngle, 0f);
    }

    /// <summary>
    /// Begin to make an attempt at drifting
    /// If the conditions for drifiting are met,
    /// you'll move to the next drifting script
    /// </summary>
    public void AttemptDrift()
    {
        if (isGrounded)
        {
            isGrounded = false;
            rBody.AddForce(Vector3.up * hopForce, ForceMode.Impulse);

            if (Mathf.Abs(movementDirection.x) > 0.1f && Mathf.Abs(velocity.x) > 20)
            {
                rBody.AddForce(Vector3.up * hopForce, ForceMode.Impulse);
                isDrifting = true;
                isDriftingLeft = movementDirection.x < 0f;
                driftTime = 0f;

                Debug.Log("Started Drift: " + (isDriftingLeft ? "Left" : "Right"));

                // Visual drift lean
                float yRot = isDriftingLeft ? -driftVisualAngle : driftVisualAngle;
                float zTilt = isDriftingLeft ? driftVisualAngle : -driftVisualAngle;

                driftRotationTween?.Kill();
                driftRotationTween = kartModel.DOLocalRotate(new Vector3(0f, yRot, zTilt), driftTweenDuration)
                    .SetEase(Ease.InOutSine);
            }
        }
    }

    /// <summary>
    /// Begin the drifting script once the conditions are met.
    /// Allows the kart to hit a sharper angle when turning
    /// </summary>
    public void Drift()
    {
        if (!isDrifting) return;

        driftTime += Time.deltaTime;

        Vector3 localVel = transform.InverseTransformDirection(rBody.velocity);

        // Add lateral sliding
        float slidePower = minDriftSteer;
        float direction = isDriftingLeft ? -1f : 1f;
        localVel.x = Mathf.Lerp(localVel.x, direction * slidePower, driftFactor);

        localVel.z *= 0.99f;

        rBody.velocity = transform.TransformDirection(localVel);
    }

    /// <summary>
    /// Ends the specific drift state.
    /// Provides a boost after drifting for a short time
    /// </summary>
    public void EndDrift()
    {

        if (!isDrifting) return;

        if (driftTime > minDriftTime)
        {
            Vector3 boost = transform.forward * driftBoostForce;
            rBody.AddForce(boost, ForceMode.VelocityChange);
            Debug.Log("Drift Boost Applied");
        }

        isDrifting = false;
        driftTime = 0f;

        // Reset visuals
        driftRotationTween?.Kill();
        driftRotationTween = kartModel.DOLocalRotate(Vector3.zero, driftTweenDuration)
            .SetEase(Ease.InOutSine);

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();

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

            // stops engine sound
            if (soundPlayer.isPlaying)
            {
                soundPlayer.Stop();
            }
        }
    }

    public void OnDrift(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            AttemptDrift();
        }
        else if (context.canceled)
        {
            EndDrift();
        }
    }

    public void OnAcceleration(InputAction.CallbackContext context)
    {
        movementDirection.z = context.ReadValue<float>();

        // determines when driving starts and when driving ends
        if (context.started)
        {
            isDriving = true;
        }
        else if (context.canceled)
        {
            isDriving = false;
        }
    }

    public void OnTurn(InputAction.CallbackContext context)
    {
        movementDirection.x = context.ReadValue<float>();
    }

    public void EndGame(InputAction.CallbackContext context)
    {
        if (gamemanagerObj)
        {
            gamemanagerObj.GameFinished();
        }
    }
}

