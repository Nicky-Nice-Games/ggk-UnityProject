using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.VFX;

public class KartVisual : MonoBehaviour
{
    [Header("References")]
    public Rigidbody sphereRigidbody;
    public Transform visualRoot;      // Handles ground alignment and vertical compression
    public Transform leanContainer;   // Handles pitch/roll for acceleration leaning
    public Transform[] suspensionPoints;
    public NEWDriver driver; 


    [Header("Suspension Settings")]
    public float suspensionLength = 0.9f;
    public float suspensionOffset = 0.2f;
    public LayerMask groundLayer;
    public float rotationFactor = 10f;

    [Header("Lean Settings")]
    public float maxPitch = 10f;
    public float maxRoll = 15f;
    public float leanSmoothing = 2f;

    [Header("Bounce Settings")]
    public float landingVelocityThreshold = -6f;
    public float squashAmount = 2f;
    public float squashSpeed = 11f;

    [Header("Wheel Suspension Settings")]
    public Transform[] wheelMeshes;            // Match order with suspensionPoints
    public float wheelRestDistance = 0.61f;     // Default distance from suspension point to wheel
    public float wheelMaxCompression = 1.32f;   // Max how far it can push into chassis
    public float wheelSmoothTime = 0.05f;

    [Header("Wheel Rotation Settings")]
    public float wheelRadius = 0.3f; //Set based on tire mesh size

    [Header("Visual Effects")]
    public VisualEffect landingEffect; 


    private Vector3[] wheelVelocity;           //For SmoothDamp per wheel

    private Vector3 lastVelocity;
    private Vector3 visualVelocity;
    private Vector3 squashScaleVelocity;
    private bool isSquashing = false;
    private bool wasFlying = false;

    void Start()
    {
        lastVelocity = sphereRigidbody.velocity;
        wheelVelocity = new Vector3[wheelMeshes.Length];
        landingEffect.Stop();
    }

    void LateUpdate()
    {
        ApplySuspension();
        ApplyLeaning();
        CheckLanding();
        AnimateSquash();
        RotateWheels();

        wasFlying = !driver.isGrounded;
    }

    void RotateWheels()
    {
        //Project velocity onto the forward direction of the visual root
        Vector3 forward = visualRoot.forward;
        float forwardSpeed = Vector3.Dot(sphereRigidbody.velocity, forward);

        //Calculate how much to rotate this frame
        float rotationAmount = (forwardSpeed / (2f * Mathf.PI * wheelRadius)) * 360f * Time.deltaTime;

        foreach (var wheel in wheelMeshes)
        {
            // Rotate around local X axis (assumes wheel rolls around X)
            wheel.Rotate(Vector3.right, rotationAmount, Space.Self);
        }
    }


    void ApplySuspension()
    {
        //Vector3 avgNormal = Vector3.up;
        //float totalCompression = 0f;
        //int hitCount = 0;
        //
        //foreach (var point in suspensionPoints)
        //{
        //    if (Physics.Raycast(point.position, Vector3.down, out RaycastHit hit, suspensionLength, groundLayer))
        //    {
        //        avgNormal += hit.normal;
        //        totalCompression += 1f - (hit.distance / suspensionLength);
        //        hitCount++;
        //    }
        //}
        //
        //if (hitCount > 0)
        //{
        //    avgNormal.Normalize();
        //    totalCompression /= hitCount;
        //    
        //    // Compress visualRoot vertically
        //    Vector3 offset = -avgNormal * totalCompression * suspensionOffset;
        //    visualRoot.localPosition = Vector3.SmoothDamp(visualRoot.localPosition, offset, ref visualVelocity, 0.1f);
        //}

        // Update wheel positions
        for (int i = 0; i < suspensionPoints.Length; i++)
        {
            var rayOrigin = suspensionPoints[i].position;
            var wheel = wheelMeshes[i];

            float targetOffset = wheelRestDistance;

            if (Physics.Raycast(rayOrigin, -visualRoot.transform.up, out RaycastHit hit, suspensionLength, groundLayer))
            {
                float compression = 1f - (hit.distance / suspensionLength);
                targetOffset -= compression * wheelMaxCompression;
            }

            //Convert world-space target position back to local relative to suspension point
            Vector3 localTarget = suspensionPoints[i].InverseTransformPoint(rayOrigin - Vector3.up * targetOffset);
            Vector3 localCurrent = suspensionPoints[i].InverseTransformPoint(wheel.position);

            Vector3 newLocal = Vector3.SmoothDamp(localCurrent, localTarget, ref wheelVelocity[i], wheelSmoothTime);
            wheel.position = suspensionPoints[i].TransformPoint(newLocal);
        }
    }

    void ApplyLeaning()
    {
        Vector3 acceleration = (sphereRigidbody.velocity - lastVelocity) / Time.deltaTime;
        Vector3 localAccel = visualRoot.InverseTransformDirection(acceleration);

        float targetPitch = Mathf.Clamp(-localAccel.z * maxPitch, -maxPitch, maxPitch);
        float targetRoll = Mathf.Clamp(-localAccel.x * maxRoll, -maxRoll, maxRoll);

        Quaternion currentRot = leanContainer.localRotation;
        Quaternion targetRot = Quaternion.Euler(targetPitch, 0f, targetRoll);

        leanContainer.localRotation = Quaternion.Slerp(currentRot, targetRot, Time.deltaTime * leanSmoothing);

        lastVelocity = sphereRigidbody.velocity;
    }

    void CheckLanding()
    {
        float verticalVel = sphereRigidbody.velocity.y;
        float lastYVel = lastVelocity.y;

        if (driver.isGrounded && wasFlying && !driver.attemptingDrift)
        {
            TriggerLandingSquash();
            landingEffect.Play();
        }
    }

    void TriggerLandingSquash()
    {
        isSquashing = true;
    }

    void AnimateSquash()
    {
        if (isSquashing)
        {
            Vector3 targetScale = new Vector3(1f, squashAmount, 1f);
            leanContainer.localScale = Vector3.SmoothDamp(leanContainer.localScale, targetScale, ref squashScaleVelocity, 0.05f);

            if (Vector3.Distance(leanContainer.localScale, targetScale) < 0.01f)
            {
                isSquashing = false;
            }
        }
        else
        {
            Vector3 targetScale = Vector3.one;
            leanContainer.localScale = Vector3.SmoothDamp(leanContainer.localScale, targetScale, ref squashScaleVelocity, 0.05f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(suspensionPoints[0].position, -visualRoot.transform.up * suspensionLength);
        Gizmos.DrawRay(suspensionPoints[1].position, -visualRoot.transform.up * suspensionLength);
        Gizmos.DrawRay(suspensionPoints[2].position, -visualRoot.transform.up * suspensionLength);
        Gizmos.DrawRay(suspensionPoints[3].position, -visualRoot.transform.up * suspensionLength);

    }
}
