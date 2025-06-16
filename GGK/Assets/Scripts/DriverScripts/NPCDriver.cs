using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;
using Unity.Mathematics;
using Unity.VisualScripting;

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
    void Start()
    {
        rBody.drag = 0.5f;
        rBody.freezeRotation = true;
        topMaxSpeed = maxSpeed;
        boosted = false;
    }

    void FixedUpdate()
    {
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

        float maxDistance = boosted ? 24000f * boostDistanceMultiplier : 24000f;
        if (distanceSquaredToFollow > maxDistance)
        {
            returningToTarget = true;
        }
        else if (distanceSquaredToFollow < 1500)
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
            rBody.velocity = toTarget.normalized * returnSpeed;
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
        else
        {
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

        //// ROTATION TO MATCH TARGET
        //Quaternion targetRotation = followTarget.rotation;
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationAlignSpeed * Time.fixedDeltaTime);
        //
        //// VELOCITY ALIGNMENT (optional, for smooth forward motion)
        //Vector3 desiredForward = followTarget.forward;
        //velocity = Vector3.Lerp(velocity, desiredForward * velocity.magnitude, Time.fixedDeltaTime * 5f);


        rBody.velocity = velocity;
        rBody.MoveRotation(transform.rotation);

        // Fall faster if in air
        if (!isGrounded)
        {
            rBody.AddForce(Vector3.down * 200f, ForceMode.Acceleration);
        }

        //Debug.Log(velocity.magnitude);
    }

    public void DisableDriving()
    {
        returningToTarget = true;
        rBody.velocity = Vector3.zero;
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
    }
}