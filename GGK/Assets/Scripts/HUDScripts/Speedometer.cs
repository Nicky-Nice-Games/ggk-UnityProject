// Joshua Chisholm
// 7/2/25
// Speedometer - Display speed in a speedometer format

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField]
    private GameObject numTextDisplay;
    int labelNum;

    // Set initial values
    void Start()
    {
        maxSpeedAngle = 210;
        minSpeedAngle = -20;
        maxSpeed = 100;
        maxRotations = maxSpeedAngle - minSpeedAngle;
        labelNum = (maxSpeed / 10) + 1;

        // Create labels for soeeds
        for (int i = 0; i < labelNum; i++)
        {
            // Instatiate the label
            GameObject label = Instantiate(numTextDisplay, gameObject.transform.GetChild(1).transform);
            float labelSpeedNormalized = (float)i / labelNum;

            // Rotate it according to speed
            label.transform.eulerAngles = new Vector3(0, 0, maxSpeedAngle - labelSpeedNormalized * maxRotations - 5);

            // Get text, assign values for speed and set rotation to Vector3.zero
            TextMeshProUGUI myText = label.GetComponentInChildren<TextMeshProUGUI>();
            myText.text = (i * 10).ToString();
            myText.gameObject.transform.eulerAngles = Vector3.zero;

            // Set label to active
            label.SetActive(true);
        }
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
