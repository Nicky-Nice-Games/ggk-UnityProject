using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpeedCameraEffect : MonoBehaviour
{
    //public Transform target;               // The kart
    //public Rigidbody targetRigidbody;      // Rigidbody of the kart
    //
    //[Header("FOV Settings")]
    //public float minFOV = 55f;
    //public float maxFOV = 75f;
    //public float maxSpeed = 40f;
    //public float fovSmoothSpeed = 5f;
    //
    //[Header("Position Settings")]
    //public Vector3 baseOffset = new Vector3(0f, 2.5f, -5f);
    //public float maxZoomOutZ = -7f;       // How far back at max speed
    //public float positionSmoothTime = 0.1f;
    //
    //private Camera cam;
    //private Vector3 velocityRef;
    //
    //private Vector3 smoothedTargetPosition;
    //
    //void Start()
    //{
    //    cam = GetComponent<Camera>();
    //    smoothedTargetPosition = target.position;
    //}
    //
    //void FixedUpdate()
    //{
    //
    //    float forwardSpeed = Vector3.Dot(targetRigidbody.velocity, target.forward);
    //    float speedPercent = Mathf.Clamp01(forwardSpeed / maxSpeed);
    //    float distanceToTarget = Vector3.Distance(transform.position, target.position);
    //    
    //
    //    //Smoothing position updates
    //    smoothedTargetPosition = Vector3.SmoothDamp(
    //        smoothedTargetPosition,
    //        target.position,
    //        ref velocityRef,
    //        positionSmoothTime
    //    );
    //
    //    //Fov adjust
    //    float targetFOV = Mathf.Lerp(minFOV, maxFOV, speedPercent);
    //    cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSmoothSpeed);
    //
    //    //adjusting camera positions
    //    float dynamicZ = Mathf.Lerp(baseOffset.z, maxZoomOutZ, speedPercent);
    //    Vector3 dynamicOffset = new Vector3(baseOffset.x, baseOffset.y, dynamicZ);
    //    Vector3 desiredPosition = target.TransformPoint(dynamicOffset);
    //
    //
    //    desiredPosition = desiredPosition + (smoothedTargetPosition - target.position);
    //
    //    
    //
    //    //if (distanceToTarget < 9.5f)
    //    //{
    //    //    // If the kart is very close, adjust the camera to avoid clipping
    //    //    transform.position = target.position + baseOffset;
    //    //    
    //    //    
    //    //}
    //
    //    transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10f);
    //    
    //    
    //    //Look at the kart
    //    Vector3 lookPoint = smoothedTargetPosition + Vector3.up * 1f;
    //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPoint - transform.position), Time.deltaTime * 8f);
    //    
    //}

    public Transform target;               // The kart
    public Rigidbody targetRigidbody;      // Rigidbody of the kart
    public Transform lookBackTarget;       // Object in front of kart

    [Header("FOV Settings")]
    public float minFOV = 55f;
    public float maxFOV = 75f;
    public float maxSpeed = 40f;
    public float fovSmoothSpeed = 8f;

    [Header("Camera Position")]
    public Vector3 baseOffset = new Vector3(0f, 2.5f, -5f);
    public float maxZoomOutZ = -7f;
    public float followSpeed = 15f;

    private Camera cam;

    private bool isHoldingTab;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    public void FollowKart(Transform kart)
    {
        target = kart.Find("CameraFollowFront");
        targetRigidbody = kart.GetComponentInChildren<Rigidbody>();
        lookBackTarget = kart.Find("CameraFollowBack");
    }

    void FixedUpdate()
    {
        if (!target || !targetRigidbody) return;

        // 1. Forward speed only
        float forwardSpeed = Vector3.Dot(targetRigidbody.velocity, target.forward);
        float speedPercent = Mathf.Clamp01(forwardSpeed / maxSpeed);

        // 2. Adjust FOV based on forward speed
        float targetFOV = Mathf.Lerp(minFOV, maxFOV, speedPercent);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSmoothSpeed);

        // 3. Adjust camera position
        float dynamicZ = Mathf.Lerp(baseOffset.z, maxZoomOutZ, speedPercent);
        Vector3 dynamicOffset = new Vector3(baseOffset.x, baseOffset.y, dynamicZ);
        Vector3 desiredPosition = target.TransformPoint(dynamicOffset);

        // Tighter follow with faster Lerp
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);

        // 4. Always look in the target's forward direction (not backward)
        Vector3 lookPoint = target.position + target.forward * 5f + Vector3.up * 1f;
        //Vector3 lookPoint = target.position + Vector3.up * 1f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPoint - transform.position), Time.deltaTime * 10f);

        // looks behind kart while tab is being hold
        if (isHoldingTab)
        {
            transform.position = lookBackTarget.position;
            Vector3 direction = target.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }
    }

    /// <summary>
    /// hold tab to view kart from front
    /// </summary>
    /// <param name="context">context of input action</param>
    public void OnLookBack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isHoldingTab = true;
        }

        if (context.canceled)
        {
            isHoldingTab = false;
        }
    }
}

