using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;
using Unity.Mathematics;
using Unity.VisualScripting;
using static UnityEngine.GraphicsBuffer;

public class NPCDriver : MonoBehaviour
{
    [Header("References")]
    public Transform followTarget;
    public Rigidbody rBody;
    public Transform kartModel;

    [Header("Movement Settings")]
    public float accelerationRate = 3500f;
    public float maxSpeed = 150f;
    public float minSpeed = 1f;
    public float deccelerationRate = 1f;
    public float turnSpeed = 60f;
    public float maxSteerAngle = 20f;

    [Header("Grounding")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 8.6f;
    public float suspensionOffset = 6.6f;
    public float rotationAlignSpeed = 5.0f;
    public float airGravityMultiplier = 2f;

    [Header("Spline Transition")]
    public float returnSpeed = 200;
    public float arrivalThreshold = 0.5f;
    public float blendDuration = 0.5f;
    [SerializeField] private float distanceSquaredToFollow;

    public Vector3 velocity = Vector3.zero;
    public bool isGrounded = false;
    [SerializeField] private bool returningToTarget = false;
    private float blendTimer = 0f;
    [SerializeField]
    private float topMaxSpeed;
    public bool boosted = false;
    public float boostDistanceMultiplier = 4f; // 2000 * 4 = 8000

    [SerializeField]
    private bool isRecoveringFromHit = false;
    [SerializeField]
    private float recoveryTimer = 0f;
    public float recoveryDuration = 2.5f; // Duration to recover full control

    public float TopMaxSpeed { get { return topMaxSpeed; } }
    private float bumpCooldown = 0f;

    [Header("Obstacle Avoidance")]
    public float obstacleCheckDistance = 20f;
    public LayerMask obstacleLayer;
    [SerializeField]
    private bool avoidingObstacle = false;
    private float avoidanceTimer = 0f;
    private Vector3 avoidanceDirection;
    public float avoidanceDuration = 1.5f;
    private float avoidanceBlendTimer = 0f;
    public float avoidanceBlendDuration = 0.5f;

    [Header("Edge Avoidance")]
    public float edgeCheckDistance = 3f;
    public float steerBackStrength = 1.5f;
    public LayerMask trackLayer;
    [SerializeField]
    private bool correctingEdge = false;
    private float edgeCorrectTimer = 0f;
    public float edgeCorrectDuration = 1f;

    void Start()
    {
        rBody.drag = 0.5f;
        rBody.freezeRotation = true;
        topMaxSpeed = maxSpeed;
        rBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //rBody.interpolation = RigidbodyInterpolation.Interpolate;
        boosted = false;
    }

    void FixedUpdate()
    {
        if (bumpCooldown > 0f)
        {
            bumpCooldown -= Time.fixedDeltaTime;
        }

        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, -transform.up, out hit, groundCheckDistance, groundLayer);

        if (isGrounded)
        {
            Vector3 snapPos = hit.point + hit.normal * suspensionOffset;
            transform.position = snapPos;

            if (velocity.magnitude > 1f)
            {
                Vector3 forwardDir = Vector3.ProjectOnPlane(velocity.normalized, hit.normal).normalized;
                Quaternion groundAlignedRotation = Quaternion.LookRotation(forwardDir, hit.normal);
                transform.rotation = Quaternion.Slerp(transform.rotation, groundAlignedRotation, rotationAlignSpeed * Time.deltaTime);

            }
            else
            {
                // Just align to ground if not moving
                Quaternion groundRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, groundRot, rotationAlignSpeed * Time.deltaTime);
            }
        }
        else
        {
            // Mid air rotation
            float rotationSpeed = 2f;
            Quaternion currentRotation = transform.rotation;
            Quaternion intendedRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);



            if (Quaternion.Angle(transform.rotation, intendedRotation) < .5)
            {
                intendedRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(currentRotation, intendedRotation, Time.deltaTime * rotationSpeed);
            }

        }

        rBody.drag = isGrounded ? 0.5f : 0.05f;

        Vector3 forward = isGrounded ? Vector3.Cross(transform.right, hit.normal).normalized : transform.forward;



        distanceSquaredToFollow = math.distancesq(transform.position, followTarget.position);

        float maxDistance = boosted ? 4000f * boostDistanceMultiplier : 4000f;
        if (distanceSquaredToFollow > maxDistance && !avoidingObstacle && !correctingEdge && !isRecoveringFromHit)
        {
            returningToTarget = true;
        }
        else if (distanceSquaredToFollow < 1200 || avoidingObstacle || correctingEdge || isRecoveringFromHit)
        {
            returningToTarget = false;
        }


        // Behavior when returning to spline target
        if (returningToTarget)
        {
            followTarget.GetComponent<SplineAnimate>().enabled = false;
            Vector3 toTarget = followTarget.position - transform.position;
            Quaternion targetRot = Quaternion.LookRotation(toTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 180f * Time.deltaTime);
            //accelerationRate = 2000f;
            Vector3 desiredVelocity = toTarget.normalized * returnSpeed;

            if (!isGrounded)
            {
                desiredVelocity += Vector3.down * 9.81f * airGravityMultiplier * Time.fixedDeltaTime;
            }
            rBody.velocity = desiredVelocity;
            //velocity += toTarget.normalized * (accelerationRate * Time.fixedDeltaTime);



            if (toTarget.magnitude < arrivalThreshold)
            {
                returningToTarget = false;
                blendTimer = blendDuration;
            }

            return;
        }
        else if (blendTimer > 0f)
        {
            // Smoothing phase after re-alignment
            blendTimer -= Time.deltaTime;

            Vector3 direction = (followTarget.position - transform.position).normalized;
            accelerationRate = Mathf.Lerp(accelerationRate, 3500f, Time.deltaTime);
            velocity += direction * (accelerationRate * Time.fixedDeltaTime);
        }
        else if (isRecoveringFromHit)
        {
            recoveryTimer -= Time.deltaTime;

            // Linearly increase values back to normal
            float t = 1f - (recoveryTimer / recoveryDuration);
            accelerationRate = Mathf.Lerp(500f, 3500f, t);
            maxSpeed = Mathf.Lerp(10f, topMaxSpeed, t);

            Vector3 direction = (followTarget.position - transform.position).normalized;
            velocity += direction * (accelerationRate * Time.fixedDeltaTime);

            if (recoveryTimer <= 0f)
            {
                isRecoveringFromHit = false;
                accelerationRate = 3500f;
                maxSpeed = topMaxSpeed;
            }
        }
        else
        {
            Vector3 avoidForward = transform.forward;
            Vector3 leftRay = Quaternion.AngleAxis(-45, Vector3.up) * avoidForward;
            Vector3 rightRay = Quaternion.AngleAxis(45, Vector3.up) * avoidForward;

            RaycastHit hitCenter, hitLeft, hitRight;

            bool obstacleAhead = Physics.Raycast(transform.position, avoidForward, out hitCenter, obstacleCheckDistance, obstacleLayer);
            bool obstacleLeft = Physics.Raycast(transform.position, leftRay, out hitLeft, obstacleCheckDistance, obstacleLayer);
            bool obstacleRight = Physics.Raycast(transform.position, rightRay, out hitRight, obstacleCheckDistance, obstacleLayer);

            if (obstacleAhead && !avoidingObstacle)
            {
                returningToTarget = false; // Cancel spline return if avoiding obstacle
                correctingEdge = false; // cancel edge correction if obstacle becomes priority
                avoidingObstacle = true;
                avoidanceTimer = avoidanceDuration;

                // Pick a turn direction
                if (!obstacleRight || (obstacleLeft && hitRight.distance > hitLeft.distance))
                {
                    avoidanceDirection = Quaternion.AngleAxis(45, Vector3.up) * avoidForward; // Turn right
                }
                else
                {
                    avoidanceDirection = Quaternion.AngleAxis(-45, Vector3.up) * avoidForward; // Turn left
                }
            }

            if (avoidingObstacle)
            {
                avoidanceTimer -= Time.deltaTime;

                //velocity += avoidanceDirection.normalized * (accelerationRate * Time.fixedDeltaTime);
                velocity = transform.forward * maxSpeed; // move forward in the direction it's facing
                rBody.velocity = velocity;
                Quaternion targetRot = Quaternion.LookRotation(avoidanceDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 3f);

                if (avoidanceTimer <= 0f)
                {
                    avoidingObstacle = false;
                    avoidanceBlendTimer = avoidanceBlendDuration;
                }

                return; // Skip normal movement this frame
            }
            else if (avoidanceBlendTimer > 0f)
            {
                avoidanceBlendTimer -= Time.deltaTime;

                Vector3 blendBackTarget = (followTarget.position - transform.position).normalized;
                Quaternion targetRot = Quaternion.LookRotation(blendBackTarget);

                // Smoother, slower rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 2f);

                // Gradually reapply velocity in correct direction
                velocity = Vector3.Lerp(velocity, blendBackTarget * maxSpeed , Time.deltaTime * 2f);
                rBody.velocity = velocity;

                return; // still blending back to normal
            }
            else if (correctingEdge)
            {
                edgeCorrectTimer -= Time.deltaTime;

                velocity = transform.forward * maxSpeed;
                rBody.velocity = velocity;

                Quaternion steerBack = Quaternion.LookRotation(avoidanceDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, steerBack, Time.deltaTime * steerBackStrength);

                if (edgeCorrectTimer <= 0f)
                {
                    correctingEdge = false;
                }

                return;
            }


            // ---- Edge Detection & Correction ----
            RaycastHit hitLeftEdge, hitRightEdge;
            Vector3 leftCheck = transform.position - transform.right * edgeCheckDistance;
            Vector3 rightCheck = transform.position + transform.right * edgeCheckDistance;

            bool nearLeftEdge = !Physics.Raycast(leftCheck + Vector3.up, Vector3.down, out hitLeftEdge, groundCheckDistance, trackLayer);
            bool nearRightEdge = !Physics.Raycast(rightCheck + Vector3.up, Vector3.down, out hitRightEdge, groundCheckDistance, trackLayer);

            if (isGrounded && !avoidingObstacle && !correctingEdge && (nearLeftEdge || nearRightEdge))
            {
                correctingEdge = true;
                edgeCorrectTimer = edgeCorrectDuration;

                if (nearLeftEdge)
                {
                    avoidanceDirection = Quaternion.AngleAxis(35, Vector3.up) * transform.forward; // Turn right
                }
                else
                {
                    avoidanceDirection = Quaternion.AngleAxis(-35, Vector3.up) * transform.forward; // Turn left
                }
            }


            // Full spline following physics mode
            Vector3 toTarget = followTarget.position - transform.position;
            Vector3 moveDir = toTarget.normalized;

            //accelerationRate = 3500f;
            velocity += moveDir * (accelerationRate * Time.fixedDeltaTime);
            followTarget.GetComponent<SplineAnimate>().enabled = true;
        }

        if (accelerationRate < 2000)
        {
            accelerationRate += 10;
        }

        if (maxSpeed < topMaxSpeed)
        {
            maxSpeed += 50 * Time.fixedDeltaTime;
        }

        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        if (velocity.magnitude < minSpeed) velocity = Vector3.zero;

        


        if (bumpCooldown <= 0f)
        {
            rBody.velocity = velocity;
        }

        rBody.MoveRotation(transform.rotation);

        // Fall faster if in air
        if (!avoidingObstacle && !isGrounded)
        {
            rBody.AddForce(Vector3.down * 200f, ForceMode.Acceleration);
        }

        //Debug.Log(velocity.magnitude);
    }

    public void DisableDriving()
    {
        returningToTarget = true;
        //rBody.velocity = Vector3.zero;
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;

            // Draw the raycast for ground check
            Gizmos.DrawLine(transform.position, transform.position - transform.up * groundCheckDistance);

            // Optional: Draw a small sphere at the raycast hit point (only if grounded)
            if (isGrounded)
            {
                Gizmos.DrawSphere(transform.position - transform.up * groundCheckDistance, 0.2f);
            }
        }

        // Velocity Vector
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + velocity);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * obstacleCheckDistance);
        Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(-45, Vector3.up) * transform.forward * obstacleCheckDistance);
        Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(45, Vector3.up) * transform.forward * obstacleCheckDistance);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position - transform.right * edgeCheckDistance);
        Gizmos.DrawLine(transform.position, transform.position + transform.right * edgeCheckDistance);
    }

    public void StartRecovery()
    {
        isRecoveringFromHit = true;
        recoveryTimer = recoveryDuration;

        // Gently reduce current velocity
        velocity *= 0.2f;

        // Limit acceleration and maxSpeed temporarily
        accelerationRate = 500f;
        maxSpeed = 10f;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Kart"))
        {
            Vector3 bumpDirection = transform.position - collision.transform.position;
            bumpDirection.y = 0f;
            bumpDirection.Normalize();

            float bumpForce = 8f;
            rBody.AddForce(bumpDirection * bumpForce, ForceMode.Impulse);

            bumpCooldown = 0.2f;
            StartRecovery();
        }
    }
}