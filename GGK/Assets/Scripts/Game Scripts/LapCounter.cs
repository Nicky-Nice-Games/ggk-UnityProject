using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;

public class LapCounter : MonoBehaviour
{
    // References
    public TextMeshPro lapCountText;
    public TextMeshPro checkpointText;
    public TextMeshPro placementText;
    [HideInInspector] public List<CheckPoint> checkPoints;

    // Values
    public int currentCheckPoint = 0;
    public int lap = 1;
    public int placement;
    public float distanceSquaredToNextCP = 0;

    void Update()
    {
        distanceSquaredToNextCP = math.distancesq(gameObject.transform.position, checkPoints[(currentCheckPoint + 1) % checkPoints.Count].transform.position);
    }

    public void CheckCheckPoint(CheckPoint checkpoint)
    {
        // Get index of other checkpoint
        int otherCheckPoint = checkPoints.IndexOf(checkpoint);

        // Compare checkpoints
        if (otherCheckPoint == (currentCheckPoint + 1) % checkPoints.Count)
        {
            currentCheckPoint = otherCheckPoint; // Not currentCheckPoint++ because need to loop around

            // Went around the track
            if (currentCheckPoint == 0)
            {
                lap++;
                if (lapCountText != null) lapCountText.text = "LAP " + lap + "/3";
            }

            if (checkpointText != null) checkpointText.text = "Check " + (currentCheckPoint + 1) + "/" + checkPoints.Count; 
        }
    }
}
