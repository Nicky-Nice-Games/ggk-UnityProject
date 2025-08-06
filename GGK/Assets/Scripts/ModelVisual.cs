using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelVisual : MonoBehaviour
{
    [Header("Wheel & Input")]
    public Transform steeringWheel;
    public float maxSteeringAngle = 90f;
    public float turnSpeed = 5f;

    [Header("IK Targets")]
    public Transform leftHandTarget;
    public Transform rightHandTarget;
    public Transform leftElbowHint;
    public Transform rightElbowHint;
    public float gripAngle;

    [Header("Body Lean")]
    public Transform spineBone;
    public float maxLeanAngle = 10f;

    [Header("Looking")]
    public Transform effector; // The sphere used as the multi-aim target
    public float lookRadius = 15f;
    public float lookSpeed = 5f;
    private Transform nearestKart;
    public Vector3 effectorOffset;
    public GameObject baseEffectorPos;

    [Header("Input")]
    public NEWDriver driverScript;

    //Other references
    PlacementManager PlacementManager;

    private void Start()
    {
        PlacementManager = FindObjectOfType<PlacementManager>();
        
    }
    void Update()
    {
        FindNearestKart();

        if (nearestKart != null)
        {
            Vector3 targetPosition = nearestKart.position;
            effector.position = Vector3.Lerp(
                effector.position,
                targetPosition,
                Time.deltaTime * lookSpeed
            );
        }
        else
        {
            effector.position = Vector3.Lerp(
                effector.position,
                baseEffectorPos.transform.position,
                Time.deltaTime * lookSpeed
            );
        }

        //// Wheel Rotation
        //float targetAngle = turnInput * maxSteeringAngle;
        //Quaternion targetRot = Quaternion.Euler(0, -targetAngle, 0);
        //steeringWheel.localRotation = Quaternion.Slerp(steeringWheel.localRotation, targetRot, Time.deltaTime * turnSpeed);

        // Hand Targets (rotate around wheel pivot)
        //UpdateHandTarget(leftHandTarget, -gripAngle);
        //UpdateHandTarget(rightHandTarget, gripAngle);

        // Elbow Hints (offsets from shoulder toward back)
        UpdateElbowHint(leftHandTarget, leftElbowHint, Vector3.left);
        UpdateElbowHint(rightHandTarget, rightElbowHint, Vector3.right);
        
        // Torso Lean
        float leanAmount = driverScript.movementDirection.x * maxLeanAngle;
        Debug.Log("Lean Amount: " + leanAmount);
        Vector3 currentEuler = spineBone.localEulerAngles;
        spineBone.localEulerAngles = new Vector3(
            -leanAmount,
            currentEuler.y,
            currentEuler.z
        );
    }

    void UpdateHandTarget(Transform handTarget, float angleOffset)
    {
        Quaternion handRot = Quaternion.Euler(0, 0, angleOffset);
        handTarget.position = steeringWheel.position + (steeringWheel.rotation * handRot * Vector3.up * 0.2f); // adjust radius
        handTarget.rotation = steeringWheel.rotation * Quaternion.Euler(90, 0, 0); // tweak this
    }

    void UpdateElbowHint(Transform hand, Transform hint, Vector3 sideDir)
    {
        Vector3 elbowOffset = sideDir * 0.3f + Vector3.back * 0.3f;
        hint.position = hand.position + elbowOffset;
    }

    void FindNearestKart()
    {
        List<GameObject> allKarts = PlacementManager.kartsList;
        float minDistance = Mathf.Infinity;
        nearestKart = null;

        foreach (GameObject kart in allKarts)
        {
            if (kart == driverScript.sphere.gameObject) continue; // Skip self

            float dist = Vector3.Distance(transform.position, kart.transform.position);
            if (dist < minDistance && dist < lookRadius)
            {
                minDistance = dist;
                nearestKart = kart.transform;
            }
        }
    }
}
