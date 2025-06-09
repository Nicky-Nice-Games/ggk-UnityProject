using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;

public class LapCounter : MonoBehaviour
{
    // References
    public TextMeshProUGUI lapCountText;
    public TextMeshProUGUI checkpointText;
    public TextMeshProUGUI placementText;
    [HideInInspector] public List<CheckPoint> checkpoints;

    // Values
    public int currentCheckPoint = 0;
    public int lap = 1;
    public int placement;
    public float distanceSquaredToNextCP = 0;

    void Update()
    {
        distanceSquaredToNextCP = math.distancesq(gameObject.transform.position, checkpoints[(currentCheckPoint + 1) % checkpoints.Count].transform.position);
    }

    public void CheckCheckPoint(CheckPoint checkpoint)
    {
        // Get index of other checkpoint
        int otherCheckPoint = checkpoints.IndexOf(checkpoint);

        // Compare checkpoints
        if (otherCheckPoint == (currentCheckPoint + 1) % checkpoints.Count)
        {
            currentCheckPoint = otherCheckPoint; // Not currentCheckPoint++ because need to loop around

            // Went around the track
            if (currentCheckPoint == 0)
            {
                lap++;
                if (lapCountText != null) lapCountText.text = "LAP " + lap + "/3";
            }

            if (checkpointText != null) checkpointText.text = "Check " + (currentCheckPoint + 1) + "/" + checkpoints.Count; 
        }
    }
}
