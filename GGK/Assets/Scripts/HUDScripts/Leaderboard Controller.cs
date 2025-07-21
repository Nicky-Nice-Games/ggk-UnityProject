using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardController : NetworkBehaviour
{
    public static LeaderboardController instance;
    // References
    public GameObject leaderboard;
    public GameObject leaderboardItem;
    private List<KartCheckpoint> finishedKarts = new List<KartCheckpoint>();

    [SerializeField]
    private TextMeshProUGUI timeDisplay;
    // Timer
    public float curTime;
    public NetworkVariable<float> networkTime = new NetworkVariable<float>(0);

    public int numOfPlayerKarts = 0;
    public NetworkVariable<bool> allPlayerKartsFinished = new NetworkVariable<bool>(false);
    public List<KartCheckpoint> finishedPlayerKarts = new List<KartCheckpoint>();


    private void Awake()
    {
        instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            // If server, update networkTme
            curTime += Time.deltaTime;
            networkTime.Value = curTime;
            //Debug.Log(networkTime.Value);
            //updating if all karts are finished
            allPlayerKartsFinished.Value = NetworkManager.ConnectedClients.Count == numOfPlayerKarts;
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
            allPlayerKartsFinished.OnValueChanged += OnPlayersFinished;
        }
        
    }

    private void OnPlayersFinished(bool previousValue, bool newValue)
    {
        leaderboard.SetActive(true);
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
        {
            finishedKarts.Add(kart);
        }

        //if multiplayer, figure out number of clients +1, for each player kart, ++ until matches # of clients(+1)

        //if (kart.GetComponent<NEWDriver>() != null)
        //{
        //leaderboard.SetActive(true);

        if (kart.gameObject.transform.parent.GetChild(0).GetComponent<NEWDriver>() != null)
        {
            numOfPlayerKarts++;
            Debug.Log("Player kart added, total: " + numOfPlayerKarts); 
            if (!finishedPlayerKarts.Contains(kart))
            {
                finishedPlayerKarts.Add(kart);
            }
            if (!IsSpawned)
            {
                leaderboard.SetActive(true);
                allPlayerKartsFinished.Value = true;
            }
        }

        // if(allPlayerKartsFinished.Value)
        // {
        //     leaderboard.SetActive(true);
        // }

        //if (IsClient || IsServer)
        //{
        //    numOfPlayerKarts++;
        //    Debug.Log("Player kart added, total: " + numOfPlayerKarts);
        //}
        //else if (kart.GetComponent<NEWDriver>() != null)
        //{
        //    numOfPlayerKarts = 1;
        //}

        //}


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

            if (!IsSpawned) // single player
            {
                GameObject tempItem = Instantiate(leaderboardItem);
                TextMeshProUGUI[] tempArray = tempItem.GetComponentsInChildren<TextMeshProUGUI>();
                tempArray[0].text = k.placement.ToString();
                tempArray[1].text = k.name;
                tempArray[2].text = string.Format("{0:00}:{1:00.00}", (int)k.finishTime / 60, k.finishTime % 60);

                tempItem.transform.SetParent(leaderboard.transform);
                tempItem.transform.localScale = Vector3.one;
            }
            else if (IsServer)
            {
                int tempPlacement = k.placement;
                string tempName = k.name;
                float tempFinishTime = k.finishTime;

                SendTimeDisplayRpc(new LeaderboardDisplayCard(tempPlacement, tempName, tempFinishTime));
            }          
            else
            {
                Debug.Log("this is client");
            }
            
        }

        
    }

    /// <summary>
    /// Event for clients for when times changes on server
    /// Clients recieve the updated time as their current time.
    /// </summary>
    /// <param name="prevTime">The previous time</param>
    /// <param name="presTime">The present time</param>
    public void OnTimeChange(float prevTime, float presTime)
    {
        curTime = presTime;
    }


    [Rpc(SendTo.ClientsAndHost,RequireOwnership = false)]
    public void SendTimeDisplayRpc(LeaderboardDisplayCard card)
    {
        Debug.Log("SendTimeDisplayRPC called");
        GameObject tempItem = Instantiate(leaderboardItem);
        TextMeshProUGUI[] tempArray = tempItem.GetComponentsInChildren<TextMeshProUGUI>();

        tempArray[0].text = card.Placement.ToString();
        tempArray[1].text = card.Name;
        tempArray[2].text = string.Format("{0:00}:{1:00.00}", (int)card.Time / 60, card.Time % 60);

        tempItem.transform.SetParent(leaderboard.transform);
        tempItem.transform.localScale = Vector3.one;
        Debug.Log("added card to leaderboard");
    }

    public struct LeaderboardDisplayCard : INetworkSerializable
    {
        public int Placement;
        public string Name;
        public float Time;

        public LeaderboardDisplayCard(int placement, string name, float time)
        {
            Placement = placement;
            Name = name;
            Time = time;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Placement);
            serializer.SerializeValue(ref Name);
            serializer.SerializeValue(ref Time);
        }
    }

}