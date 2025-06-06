using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public List<LapCounter> karts;
    public List<CheckPoint> checkPoints;
    private List<LapCounter> sortedList;

    void Start()
    {
        karts = FindObjectsOfType<LapCounter>().ToList();
        
        foreach (LapCounter kart in karts)
        {
            kart.checkPoints = checkPoints;
        }
    }

    void Update()
    {
        CheckPlacement();
    }

    public void CheckPlacement()
    {
        // Sort by lap, then checkpoint ID, then next checkpoint distance
        sortedList = karts.OrderByDescending(kart => kart.lap)
            .ThenByDescending(kart => kart.currentCheckPoint)
            .ThenBy(kart => kart.distanceSquaredToNextCP)
            .ToList();

        // Assign Placements
        for (int i = 0; i < sortedList.Count; i++)
        {
            if (i == 0)
            {
                sortedList[i].placement = 1;
                if (sortedList[i].placementText != null) sortedList[i].placementText.text = "1st";
            }
            else if (i == 1)
            {
                sortedList[i].placement = 2;
                if (sortedList[i].placementText != null) sortedList[i].placementText.text = "2nd";
            }
            else if (i == 2)
            {
                sortedList[i].placement = 3;
                if (sortedList[i].placementText != null) sortedList[i].placementText.text = "3rd";
            }
            else
            {
                sortedList[i].placement = i + 1;
                if (sortedList[i].placementText != null) sortedList[i].placementText.text = sortedList[i].placement + "th";
            }
        }
    }
}
