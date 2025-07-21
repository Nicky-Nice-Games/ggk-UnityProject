using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    // References
    public GameObject leaderboard;
    public GameObject leaderboardItem;

    // Timer
    public float curTime = 0;

    // Update is called once per frame
    void Update()
    {
        curTime += Time.deltaTime;

        //if (Input.GetKeyDown(KeyCode.Tab))
        //{
        //    ToggleLeaderBoard();
        //}
    }

    public void ToggleLeaderBoard()
    {
        if (leaderboard.activeSelf)
        {
            leaderboard.SetActive(false);
        }
        else
        {
            leaderboard.SetActive(true);
        }
    }

//This method has been commented out due to causing issues with the assembly definition references
    //public void Finished(KartCheckpoint kart)
    //{
    // if player kart finishes opens up leaderboard (how will this work in multiplayer?)
    //    if (this.GetComponent<NPCDriver>() == null)
    //    {
    //        leaderboard.gameObject.SetActive(true);
    //    }

    // Otherwise add time to leaderboad like normal
    //    GameObject tempItem = Instantiate(leaderboardItem);
    //    TextMeshProUGUI[] tempArray = tempItem.GetComponentsInChildren<TextMeshProUGUI>();
    //    tempArray[0].text = kart.placement.ToString();
    //    tempArray[1].text = kart.name;
    //    tempArray[2].text = string.Format("{0:00}:{1:00.00}", (int)kart.finishTime / 60 , kart.finishTime % 60);

    //    tempItem.transform.SetParent(leaderboard.transform);
    //    tempItem.transform.localScale = Vector3.one;
    //}
}
