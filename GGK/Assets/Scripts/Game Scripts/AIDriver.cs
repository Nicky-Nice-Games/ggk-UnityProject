using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class AIDriver : MonoBehaviour
{
    //Reference
    private LapCounter lapCounter;
    private Driver driver;

    // Test variable
    public float angleToStartX = 15;
    public float angleToStartDrift = 25;

    // Start is called before the first frame update
    void Start()
    {
        lapCounter = gameObject.GetComponent<LapCounter>();
        driver = gameObject.GetComponent<Driver>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 desiredLocation = lapCounter.checkPoints[(lapCounter.currentCheckPoint + 1) % lapCounter.checkPoints.Count].transform.position;
        Vector3 desiredDirection = (desiredLocation - gameObject.transform.position).normalized;

        // Test if direction is in front, back, left or right or kart
        float forwardDot = Vector3.Dot(transform.forward, desiredDirection);
        float rightDot = Vector3.Dot(transform.right, desiredDirection);

        if (forwardDot >= 0)
        {
            driver.movementDirection.z = 1;
        }
        else if (forwardDot < 0)
        {
            driver.movementDirection.z = -1;
        }

        // Smaller angle between kart direction and desired direction
        float angle = Vector3.Angle(transform.forward, desiredDirection);

        // If angle above angleToStartX start rotating, if angle furthur above angleToStartDrift start drifting
        if (angle > angleToStartX)
        {
            if (angle > angleToStartDrift)
            {
                driver.AttemptDrift();
            }
            else
            {
                driver.EndDrift();
            }
            if (rightDot > 0)
            {
                driver.movementDirection.x = 1;
            }
            else
            {
                driver.movementDirection.x = -1;
            }
        }
        else
        {
            driver.movementDirection.x = 0;
            driver.EndDrift();
        }
    }
}
