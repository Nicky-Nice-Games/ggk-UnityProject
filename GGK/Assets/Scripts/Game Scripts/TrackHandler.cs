using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrackHandler : MonoBehaviour
{
    // Get Player LapCounter
    public LapCounter player;

    public void Start()
    {
        player = FindObjectOfType<LapCounter>();
        player.lapCountText = GameObject.Find("LapText").GetComponent<TextMeshPro>();
        player.checkpointText = GameObject.Find("CheckText").GetComponent<TextMeshPro>();
        player.placementText = GameObject.Find("PlacementText").GetComponent<TextMeshPro>();
    }
}
