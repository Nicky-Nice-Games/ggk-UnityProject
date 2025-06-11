using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class KartVisual : MonoBehaviour
{
    [Header("References")]
    public Rigidbody sphereRigidbody;
    public Transform visualRoot;      // Handles ground alignment and vertical compression
    public Transform leanContainer;   // Handles pitch/roll for acceleration leaning
    public Transform[] suspensionPoints;

    [Header("Suspension Settings")]
    public float suspensionLength = 0.5f;
    public float suspensionOffset = 0.2f;
    public LayerMask groundLayer;
    public float rotationFactor = 10f;

    [Header("Lean Settings")]
    public float maxPitch = 10f;
    public float maxRoll = 15f;
    public float leanSmoothing = 5f;

    [Header("Bounce Settings")]
    public float landingVelocityThreshold = -6f;
    public float squashAmount = 0.9f;
    public float squashSpeed = 8f;

    private Vector3 lastVelocity;
    private Vector3 visualVelocity;
    private Vector3 squashScaleVelocity;
    private bool isSquashing = false;

    void Start()
    {
        lastVelocity = sphereRigidbody.velocity;
    }

    void LateUpdate()
    {
        ApplySuspension();
        ApplyLeaning();
        CheckLanding();
        AnimateSquash();
    }

    void ApplySuspension()
    {
        Vector3 avgNormal = Vector3.up;
        float totalCompression = 0f;
        int hitCount = 0;

        foreach (var point in suspensionPoints)
        {
            if (Physics.Raycast(point.position, Vector3.down, out RaycastHit hit, suspensionLength, groundLayer))
            {
                avgNormal += hit.normal;
                totalCompression += 1f - (hit.distance / suspensionLength);
                hitCount++;
            }
        }

        if (hitCount > 0)
        {
            avgNormal.Normalize();
            totalCompression /= hitCount;

            // Rotate visualRoot to match ground
            //Quaternion targetRot = Quaternion.FromToRotation(visualRoot.up, avgNormal) * visualRoot.rotation;
            //visualRoot.rotation = Quaternion.Slerp(visualRoot.rotation, targetRot, Time.deltaTime * rotationFactor);

            // Compress visualRoot vertically
            Vector3 offset = -avgNormal * totalCompression * suspensionOffset;
            visualRoot.localPosition = Vector3.SmoothDamp(visualRoot.localPosition, offset, ref visualVelocity, 0.1f);
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

        if (!isSquashing && lastYVel < landingVelocityThreshold && Mathf.Abs(verticalVel) < 0.1f)
        {
            TriggerLandingSquash();
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
        Gizmos.DrawRay(suspensionPoints[0].position, Vector3.down * suspensionLength);
        Gizmos.DrawRay(suspensionPoints[1].position, Vector3.down * suspensionLength);
        Gizmos.DrawRay(suspensionPoints[2].position, Vector3.down * suspensionLength);
        Gizmos.DrawRay(suspensionPoints[3].position, Vector3.down * suspensionLength);

    }
}
