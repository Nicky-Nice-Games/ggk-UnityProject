using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEditor.Experimental.GraphView;
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
            timeDisplay.text = "Time: " + string.Format("{0:00}:{1:00.000}", minutes, seconds);
        }
        else
        {
            curTime = 0;
            float seconds = curTime % 60;
            int minutes = (int)curTime / 60;
            timeDisplay.text = "Time: " + string.Format("{0:00}:{1:00.000}", minutes, seconds);
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

        if (IsServer)
        {
            //Debug.Log("I am the server");
            NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnectHandler;
        }
        else
        {
            //Debug.Log("I am a client");
            NetworkManager.Singleton.OnClientDisconnectCallback += ServerDisconnectHandler;
        }

    }
    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            networkTime.OnValueChanged -= OnTimeChange;
            allPlayerKartsFinished.OnValueChanged -= OnPlayersFinished;
        }
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= ClientDisconnectHandler;
        }
        else
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= ServerDisconnectHandler;
        }
    }

    private void ClientDisconnectHandler(ulong clientId)
    {
        numOfPlayerKarts--;
    }

    private void ServerDisconnectHandler(ulong clientId)
    {

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
        NEWDriver player = kart.gameObject.transform.parent.GetChild(0).GetComponent<NEWDriver>();

        if (!finishedKarts.Contains(kart))
        {
            finishedKarts.Add(kart);
        }

        if (player != null)
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

        // Sort by actual finish time
        finishedKarts.Sort((a, b) => a.finishTime.CompareTo(b.finishTime));

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
            tempArray[2].text = string.Format("{0:00}:{1:00.000}", (int)kart.finishTime / 60, kart.finishTime % 60);



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

            player.playerInfo.raceTime = curTime * 1000f;
            player.AssignPlacement(kart.placement);
        }
        else if (IsServer)
        {
            Debug.Log("multiplayer");
            int tempPlacement = kart.placement;
            string tempName = kart.name;
            float tempFinishTime = kart.finishTime;
            bool isPlayerKart = kart.transform.parent.GetChild(0).GetComponent<NEWDriver>() != null;
            ulong ownerClientId = kart.transform.parent.GetComponent<NetworkObject>().OwnerClientId;

            player.playerInfo.raceTime = curTime * 1000f;
            player.AssignPlacementRpc(kart.placement);

            SendTimeDisplayRpc(new LeaderboardDisplayCard(tempPlacement, tempName, tempFinishTime, ownerClientId, isPlayerKart));
        }
        else
        {
            Debug.Log("this is client");
            player.playerInfo.raceTime = curTime * 1000f;
            player.AssignPlacementRpc(kart.placement);
        }

        if ( player != null && 
            player.OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            player.SendThisPlayerData();
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
        tempArray[2].text = string.Format("{0:00}:{1:00.000}", (int)card.Time / 60, card.Time % 60);

        tempItem.transform.SetParent(leaderboard.transform);
        tempItem.transform.localScale = Vector3.one;

        if (card.IsPlayerKart && card.OwnerClientId == NetworkManager.Singleton.LocalClientId)
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
        public bool IsPlayerKart; 

        public LeaderboardDisplayCard(int placement, string name, float time, ulong ownerClientId, bool isPlayerKart)
        {
            Placement = placement;
            Name = name;
            Time = time;
            OwnerClientId = ownerClientId;
            IsPlayerKart = isPlayerKart;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Placement);
            serializer.SerializeValue(ref Name);
            serializer.SerializeValue(ref Time);
            serializer.SerializeValue(ref OwnerClientId);
            serializer.SerializeValue(ref IsPlayerKart); 
        }
    }

}