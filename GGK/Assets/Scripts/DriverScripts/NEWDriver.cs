using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NEWDriver : MonoBehaviour
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
    public Rigidbody sphere;
    public Transform kartNormal;
    public float maxSteerAngle = 20f; //Multiplier for wheel turning speed
    public float startingDragThreshold = 5f; //Speed threshold to apply drag

    [Header("Sphere Collider stuff")]
    public float colliderOffset = 0.4f; //Offset for the sphere collider to position kart correctly
    public Transform spherePosTransform; //Reference to the sphere collider transform


    [Header("Drift Settings")]
    //To determine drifting stuff in update
    bool isDrifting;
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
    public GameObject frontLTireItself;
    public GameObject frontTireL;
    public GameObject frontRTireItself;
    //Back tires GO
    public GameObject backTireR;
    public GameObject backTireL;

    [Header("Reference to the kartModel transform for Animation")]
    public Transform kartModel;

    [Header("Particle Effects")]
    public ParticleSystem driftSparksLeftBack;
    public ParticleSystem driftSparksLeftFront;
    public ParticleSystem driftSparksRightFront;
    public ParticleSystem driftSparksRightBack;


    [Header("Raycast Settings")]
    public LayerMask groundLayer;
    public float groundDist = 0.3f;
    float groundedTimer = 0f;
    public float groundedBufferTime = 0.15f; // tweakable


    // Ground snapping variables
    [SerializeField] Transform modelTransform;
    [SerializeField] Transform playerModelTransform;
    public bool isGrounded;
    public bool attemptingDrift;
    public float groundCheckDistance = 8.6f;
    public float suspensionOffset = 6.6f;
    public float rotationAlignSpeed = 5.0f;

    //Tween stuff
    Tween driftRotationTween;
    float driftVisualAngle = 10f;
    float driftTweenDuration = 0.4f;
    bool lastDriftDirectionLeft;


    // Start is called before the first frame update
    void Start()
    {
        sphere.drag = 0.5f;
        velocity = Vector3.zero;


        //accelerationRate = 3500f;
        //deccelerationRate = 1f;
        //minSpeed = 1;
        //maxSpeed = 150;
        //turnSpeed = 60;

        driftSparksLeftBack.Stop();
        driftSparksLeftFront.Stop();
        driftSparksRightFront.Stop();
        driftSparksRightBack.Stop();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleGroundCheck();
        ApplyWheelVisuals();

        //Follow Collider
        transform.position = new Vector3(spherePosTransform.transform.position.x, spherePosTransform.transform.position.y - colliderOffset, spherePosTransform.transform.position.z);



        //------------Movement stuff---------------------

        //Acceleration
        if (movementDirection.z != 0f)
        {
            //velocity.y = 0f;
            acceleration = kartModel.transform.forward * (movementDirection.z * accelerationRate * Time.deltaTime);

            //sphere.velocity += acceleration * Time.fixedDeltaTime;
            //
            //sphere.velocity = Vector3.ClampMagnitude(sphere.velocity, maxSpeed);
        }
        else
        {
            //sphere.velocity *= 1f - (deccelerationRate * Time.fixedDeltaTime);
            acceleration *= 1f - (deccelerationRate * Time.fixedDeltaTime);

            //Stop the vehicle once we reach a certain minimum speed
            if (sphere.velocity.magnitude < minSpeed)
            {
                sphere.velocity = Vector3.zero;
                acceleration = Vector3.zero;
            }
        }


        //------------Turning stuff---------------------

        //to check if we are going backwards...
        float backwardsCheck = Vector3.Dot(transform.forward, sphere.velocity);

        //We should rename this var. Applies a turn multiplier when drifting
        float newTurnSpeed = isDrifting ? turnSpeed * driftTurnMultiplier : turnSpeed;

        //If we are not stationary
        if (!(sphere.velocity == Vector3.zero))
        {
            //Calculate the turning direction based on the movement direction input and multiply it by our turn speed
            float turningDirection = movementDirection.x * newTurnSpeed;

            //drifting
            if (isDrifting)
            {
                //Keep drifting
                Drift();

                //Recalculate our turningDirection value bc of drifting
                turningDirection = movementDirection.x * newTurnSpeed;

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

            acceleration = turning * acceleration;
        }
        //If we are not moving, we don't need to turn
        else
        {
            turning = Quaternion.Euler(0f, 0f, 0f);
        }

        //Falling down
        if (!isGrounded && !attemptingDrift)
        {

            // Apply extra downward force to fall faster
            //rBody.AddForce(Vector3.down * 3000f, ForceMode.Acceleration); // Tune value as needed            

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
                kartNormal.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
        //else
        //{
        //    Vector3 forward = kartNormal.forward;
        //    Vector3 right = kartNormal.right;
        //
        //    Vector3 velocity = sphere.velocity;
        //
        //    // Decompose velocity
        //    float forwardSpeed = Vector3.Dot(velocity, forward);
        //    float sidewaysSpeed = Vector3.Dot(velocity, right);
        //
        //    // Reduce sideways (lateral) movement to simulate traction
        //    sidewaysSpeed *= 0.1f; // Tune this value for how much slide you want (0 = full grip)
        //
        //    // Rebuild velocity with mostly forward motion
        //    sphere.velocity = forward * forwardSpeed + right * sidewaysSpeed;
        //}

        // Apply extra downward force to fall faster
        sphere.AddForce(Vector3.down * 1000f, ForceMode.Acceleration); // Tune value as needed

        //Update the wheel rotations
        float steerAngle = movementDirection.x * maxSteerAngle;

        frontTireL.transform.localRotation = Quaternion.Euler(0, steerAngle, 0f);
        frontTireR.transform.localRotation = Quaternion.Euler(0, steerAngle, 0f);

        //rBody.Move(transform.position + velocity * Time.fixedDeltaTime, transform.rotation * turning);
        // Apply movement

        sphere.AddForce(acceleration, ForceMode.Acceleration);
        transform.rotation = transform.rotation * turning;

    }



    void HandleGroundCheck()
    {
        RaycastHit hitNear;


        if (Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 8.0f, groundLayer))
        {
            isGrounded = true;
            //Normal Rotation
            kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
            kartNormal.Rotate(0, transform.eulerAngles.y, 0);
        }
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
        if (isGrounded && !attemptingDrift)
        {
            isGrounded = false;

            StartCoroutine(DriftHopEnabler());

            // Animate visual jump
            kartModel.DOLocalMoveY(1.0f, 0.2f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad);

            // Optional squash/stretch
            kartModel.DOScale(new Vector3(1.1f, 0.9f, 1.1f), 0.1f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutSine);

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
                    driftTime += (Time.deltaTime + -movementDirection.x) * 5;
                    break;
                default:
                    driftTime += (Time.deltaTime + 1) * 2;
                    break;
            }

        }
        else
        {
            switch (movementDirection.x)
            {
                //Turning right
                case > 0.1f:
                    driftTime += (Time.deltaTime + movementDirection.x) * 5;
                    break;
                //Turning left
                case < -0.1f:
                    driftTime += Time.deltaTime + -movementDirection.x;
                    break;
                default:
                    driftTime += (Time.deltaTime + 1) * 2;
                    break;
            }
        }

        Vector3 localVel = transform.InverseTransformDirection(sphere.velocity);

        // Add lateral sliding
        float slidePower = minDriftSteer;
        float direction = isDriftingLeft ? -1f : 1f;
        localVel.x = Mathf.Lerp(localVel.x, direction * slidePower, driftFactor);

        localVel.z *= 0.80f;

        acceleration += localVel;

        // Sparks!
        if (isDriftingLeft && driftTime >= minDriftTime && !driftSparksLeftBack.isPlaying)
        {
            driftSparksLeftBack.Play();
            driftSparksLeftFront.Play();

        }
        else if (!isDriftingLeft && driftTime >= minDriftTime && !driftSparksRightBack.isPlaying)
        {
            driftSparksRightBack.Play();
            driftSparksRightFront.Play();
        }
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
            //Vector3 boost = transform.forward * driftBoostForce;
            StartCoroutine(Boost(driftBoostForce, 2.5f));
            //rBody.AddForce(boost, ForceMode.VelocityChange);
            Debug.Log("Drift Boost Applied");
        }

        isDrifting = false;
        driftTime = 0f;

        // Reset visuals
        driftRotationTween?.Kill();
        driftRotationTween = kartModel.DOLocalRotate(Vector3.zero, driftTweenDuration)
            .SetEase(Ease.InOutSine);

        driftSparksLeftBack.Stop();
        driftSparksLeftFront.Stop();
        driftSparksRightFront.Stop();
        driftSparksRightBack.Stop();


    }

    IEnumerator Boost(float boostForce, float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            Vector3 boostDirection = transform.forward * driftBoostForce;

            sphere.AddForce(boostDirection, ForceMode.VelocityChange);
            yield return new WaitForFixedUpdate();
        }

    }

    IEnumerator DriftHopEnabler()
    {
        //Disabling raycast
        attemptingDrift = true;

        yield return new WaitForSeconds(0.5f);

        //Check if player wants to drift either direction
        if (Mathf.Abs(movementDirection.x) > 0.1f && Mathf.Abs(sphere.velocity.x) > 20)
        {
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

        attemptingDrift = false;

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();

        movementDirection.z = movementDirection.y;

        movementDirection.y = 0; //We are not gonna jump duh
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
    }

    public void OnTurn(InputAction.CallbackContext context)
    {
        movementDirection.x = context.ReadValue<float>();
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a ray to visualize the ground check
        Gizmos.color = Color.red;
        //Gizmos.DrawRay(frontRTireItself.transform.position, Vector3.down * groundCheckDistance);        
        //Gizmos.DrawRay(frontLTireItself.transform.position, Vector3.down * groundCheckDistance);
        //Gizmos.DrawRay(backTireR.transform.position, Vector3.down * groundCheckDistance);
        //Gizmos.DrawRay(backTireL.transform.position, Vector3.down * groundCheckDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, sphere.velocity);
        Gizmos.DrawRay(transform.position, Vector3.down * 4.0f);
    }
}
