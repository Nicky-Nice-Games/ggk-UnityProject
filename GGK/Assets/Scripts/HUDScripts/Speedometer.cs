// Joshua Chisholm
// 7/2/25
// Speedometer - Display speed in a speedometer format

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    // Reference to needle to change rotation of.
    [SerializeField]
    private Transform needle;

    // Reference to the player/kart for speed
    [SerializeField]
    NEWDriver player;

    // Restriction for rotation of needle
    private int maxSpeedAngle, minSpeedAngle;
    private int maxRotations;
    private int maxSpeed;

    // Set initial values
    void Start()
    {
        maxSpeedAngle = 210;
        minSpeedAngle = -20;
        maxSpeed = 80;
        maxRotations = maxSpeedAngle - minSpeedAngle;
    }

    // Update is called once per frame
    void Update()
    {
        needle.eulerAngles = new Vector3(0, 0, GetRotation());
    }

    /// <summary>
    /// Gets the rotation for the needle to be set to
    /// 
    /// </summary>
    /// <returns></returns>
    public float GetRotation()
    {
        return maxSpeedAngle - (player.sphere.velocity.magnitude/maxSpeed) * maxRotations;
    }
}
