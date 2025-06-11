using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;

public class PlacementManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<KartCheckpoint> checkpointList = new List<KartCheckpoint>();
    public List<KartCheckpoint> sortedList;
    void Start()
    {
        GameObject[] karts = GameObject.FindGameObjectsWithTag("Kart");

        checkpointList.Clear();

        foreach (GameObject kart in karts)
        {
            KartCheckpoint cp = kart.GetComponent<KartCheckpoint>();
            if (cp != null)
            {
                checkpointList.Add(cp);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        checkPlacement();
        
    }

    private void checkPlacement()
    {
        // Sort by lap first, then checkpointId
        sortedList = checkpointList.OrderByDescending(kart => kart.lap)
                                       .ThenByDescending(kart => kart.checkpointId)
                                       .ThenBy(kart => kart.distanceSquaredToNextCP)
                                       .ToList();

        // Assign placements
        for (int i = 0; i < sortedList.Count; i++)
        {
            sortedList[i].placement = i + 1; // 1st place is index 0
        }
    }
}