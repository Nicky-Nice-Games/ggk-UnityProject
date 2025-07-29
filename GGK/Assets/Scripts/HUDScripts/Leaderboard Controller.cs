using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        if (Countdown.instance.finished)
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
        else
        {
            curTime = 0;
            float seconds = curTime % 60;
            int minutes = (int)curTime / 60;
            timeDisplay.text = "Time: " + string.Format("{0:00}:{1:00.00}", minutes, seconds);
        }
    }

    /// <summary>
    /// Set up OnValueChanged functions for network variables client side
    /// </summary>
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            networkTime.OnValueChanged += OnTimeChange;
            allPlayerKartsFinished.OnValueChanged += OnPlayersFinished;
        }

    }

    /// <summary>
    /// Set leaderboard active to true once all players
    /// finish the race
    /// </summary>
    /// <param name="previousValue"></param>
    /// <param name="newValue"></param>
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

        //// Clear old entries (optional, if leaderboard is visual only)
        //for (int i = 1; i < leaderboard.transform.childCount; i++)
        //{
        //    Destroy(leaderboard.transform.GetChild(i).gameObject);
        //}

        // Add leaderboard entries in correct order
        for (int i = 0; i < finishedKarts.Count; i++)
        {
            KartCheckpoint k = finishedKarts[i];
            k.placement = i + 1;
        }

        if (!IsSpawned) // single player
        {
            Debug.Log("single player");
            GameObject tempItem = Instantiate(leaderboardItem);
            TextMeshProUGUI[] tempArray = tempItem.GetComponentsInChildren<TextMeshProUGUI>();

            tempArray[0].text = kart.placement.ToString();
            tempArray[1].text = kart.name;
            tempArray[2].text = string.Format("{0:00}:{1:00.00}", (int)kart.finishTime / 60, kart.finishTime % 60);



            tempItem.transform.SetParent(leaderboard.transform);
            tempItem.transform.localScale = Vector3.one;



            if (kart.transform.parent.GetChild(0).GetComponent<NEWDriver>() != null) // local player
            {
                for (int i = 0; i < tempArray.Length; i++)
                {
                    tempArray[i].color = Color.red;
                }
            }
            else
            {
                for (int i = 0; i < tempArray.Length; i++)
                {
                    tempArray[i].color = Color.white;
                }
            }
        }
        else if (IsServer)
        {
            Debug.Log("multiplayer");
            int tempPlacement = kart.placement;
            string tempName = kart.name;
            float tempFinishTime = kart.finishTime;
            ulong ownerClientId = kart.transform.parent.GetComponent<NetworkObject>().OwnerClientId;
            SendTimeDisplayRpc(new LeaderboardDisplayCard(tempPlacement, tempName, tempFinishTime, ownerClientId));
        }
        else
        {
            Debug.Log("this is client");
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

    /// <summary>
    /// Sends leaderboard info to clients to display server information about the race
    /// </summary>
    /// <param name="card"></param>
    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
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

        if (card.OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            for (int i = 0; i < tempArray.Length; i++)
            {
                tempArray[i].color = Color.red;
            }
        }
        else
        {
            for (int i = 0; i < tempArray.Length; i++)
            {
                tempArray[i].color = Color.white;
            }
        }

        Debug.Log("added card to leaderboard");
    }

    public struct LeaderboardDisplayCard : INetworkSerializable
    {
        public int Placement;
        public string Name;
        public float Time;
        public ulong OwnerClientId;

        public LeaderboardDisplayCard(int placement, string name, float time, ulong ownerClientId)
        {
            Placement = placement;
            Name = name;
            Time = time;
            OwnerClientId = ownerClientId;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Placement);
            serializer.SerializeValue(ref Name);
            serializer.SerializeValue(ref Time);
            serializer.SerializeValue(ref OwnerClientId);
        }
    }

}