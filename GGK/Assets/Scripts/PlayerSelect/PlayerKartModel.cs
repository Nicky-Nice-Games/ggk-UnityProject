using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKartModel : MonoBehaviour
{
    // Screen Resolution
    public Vector2 Resolution;

    // References
    public GameObject kartModel;
    private int rotation = 140;
    private float ratio = 1101f / 514f;
    private float newRatio;
    // public float testMultiplier = 0.5f;

    public GameObject optionsPanel;

    // Start is called before the first frame update
    void Start()
    {
        Resolution = new Vector2 (Screen.width, Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        // If receive input Q or E rotate that direction
        if (Input.GetKey(KeyCode.Q))
        {
            RotateLeft();
        }
        else if (Input.GetKey(KeyCode.E))
        {
            RotateRight();
        }

        // If optionsPanel is active turn off kartModel
        if (optionsPanel.activeSelf)
        {
            kartModel.SetActive(false);
        }
        else
        {
            kartModel.SetActive(true);
        }

        // Get screen resolution changes and more appropriately move with it?
        if (Resolution.x != Screen.width || Resolution.y != Screen.height)
        {
            Resolution.x = Screen.width;
            Resolution.y = Screen.height;
            RePosition();
        }
    }

    // Kart Rotation
    public void RotateLeft()
    {
        rotation--;
        kartModel.transform.rotation = Quaternion.Euler(0, rotation, 0);
    }
    public void RotateRight()
    {
        rotation++;
        kartModel.transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

    // Kart Positioning
    public void RePosition()
    {
        newRatio = (float)Screen.width / (float)Screen.height;
        float difference = ratio - newRatio;
        difference = Mathf.Abs(difference);

        if (ratio < newRatio)
        {
            kartModel.transform.position = new Vector3(-6 - difference * 5, kartModel.transform.position.y, kartModel.transform.position.z);
        }
        else if (ratio == newRatio)
        {
            kartModel.transform.position = new Vector3(-6, kartModel.transform.position.y, kartModel.transform.position.z);
        }
        else
        {
            kartModel.transform.position = new Vector3(-6 + difference * 6, kartModel.transform.position.y, kartModel.transform.position.z);
        }
    }
}
