using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardController : NetworkBehaviour
{
    // References
    public GameObject leaderboard;
    public GameObject leaderboardItem;
    private List<KartCheckpoint> finishedKarts = new List<KartCheckpoint>();
    [SerializeField]
    private TextMeshProUGUI timeDisplay;
    // Timer
    public float curTime;
    public NetworkVariable<float> networkTime = new NetworkVariable<float>(0);
    // Update is called once per frame
    void Update()
    {

        // If server, update networkTme
        if (IsServer)
        {
            curTime += Time.deltaTime;
            networkTime.Value = curTime;
            Debug.Log(networkTime.Value);
        }
        // Otherwise, update single player time
        else if (!IsSpawned)
        {
            curTime += Time.deltaTime;
        }


        // Format and display time
        float seconds = curTime % 60;
        int minutes = (int)curTime / 60;
        timeDisplay.text = "Time: " + string.Format("{0:00}:{1:00.00}", minutes, seconds);
    }

    public override void OnNetworkSpawn()
    {
        // If a client, set networkTime's OnValueChanged
        if (IsClient)
        {
            networkTime.OnValueChanged += OnTimeChange;
        }
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

    public void Finished(KartCheckpoint kart)
    {
        if (!finishedKarts.Contains(kart))
            finishedKarts.Add(kart);

        // Sort by actual finish time
        finishedKarts.Sort((a, b) => a.finishTime.CompareTo(b.finishTime));

        // Clear old entries (optional, if leaderboard is visual only)
        for (int i = 1; i < leaderboard.transform.childCount; i++)
        {
            Destroy(leaderboard.transform.GetChild(i).gameObject);
        }

        // Add leaderboard entries in correct order
        for (int i = 0; i < finishedKarts.Count; i++)
        {
            KartCheckpoint k = finishedKarts[i];
            k.placement = i + 1;

            GameObject tempItem = Instantiate(leaderboardItem);
            TextMeshProUGUI[] tempArray = tempItem.GetComponentsInChildren<TextMeshProUGUI>();
            tempArray[0].text = k.placement.ToString();
            tempArray[1].text = k.name;
            tempArray[2].text = string.Format("{0:00}:{1:00.00}", (int)k.finishTime / 60, k.finishTime % 60);

            tempItem.transform.SetParent(leaderboard.transform);
            tempItem.transform.localScale = Vector3.one;
        }

        leaderboard.SetActive(true);
    }

    /// <summary>
    /// Event for clients for when times changes on server
    /// Clients recieve the updated time as their current time.
    /// </summary>
    /// <param name="prevTime">The previous time</param>
    /// <param name="presTime">The present time</param>
    public void OnTimeChange(float prevTime, float presTime)
    {
        networkTime.Value = curTime;
    }
}