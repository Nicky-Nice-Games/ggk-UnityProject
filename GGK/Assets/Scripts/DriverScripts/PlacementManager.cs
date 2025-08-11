using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlacementManager : NetworkBehaviour
{
    public static PlacementManager instance;
    // Start is called before the first frame update
    [Header("Checkpoint settings")]
    [SerializeField] private int maxSkip = 10;

    [Header("In Scene Object References")]
    [SerializeField] public GameObject checkpointManager;
    [SerializeField] private TextMeshProUGUI placementDisplay;
    [SerializeField] private TextMeshProUGUI lapDisplay;
    [SerializeField] private KartCheckpoint trackedKart;

    [Header("List References")]
    private List<GameObject> checkpointList = new List<GameObject>();
    [SerializeField] List<KartCheckpoint> kartCheckpointList = new List<KartCheckpoint>();
    public List<KartCheckpoint> sortedList;
    public List<GameObject> kartsList;

    private void Awake()
    {
        instance = this;
        checkpointList.Clear();
        kartsList.Clear();
        kartCheckpointList.Clear();
    }

    public override void OnNetworkSpawn()
    {
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
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= ClientDisconnectHandler;
        }
        else
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= ServerDisconnectHandler;
        }
    }

    /// <summary>
    /// this is what happens when the server disconnects as seen from the client's perspective
    /// </summary>
    /// <param name="clientId">your own client id</param>
    private void ServerDisconnectHandler(ulong clientId)
    {

    }

    /// <summary>
    /// this is what happens when a client disconnects as seen from the server's perspective
    /// </summary>
    /// <param name="clientId">the client id of the player who disconnects</param>
    private void ClientDisconnectHandler(ulong clientId)
    {
        int kartIndex = FindKartIndex(clientId);
        kartCheckpointList.RemoveAt(kartIndex);
        kartsList.RemoveAt(kartIndex);
    }

    void Start()
    {
        GameObject[] karts = GameObject.FindGameObjectsWithTag("Kart");
        // checkpointList.Clear();
        // kartsList.Clear();
        // kartCheckpointList.Clear();
        LoadCheckpoints();

        foreach (GameObject kart in karts)
        {
            KartCheckpoint kartCheckpoint = kart.GetComponent<KartCheckpoint>();
            // add npcs to list for now player script added themselves
            if (kartCheckpoint != null && kart.GetComponent<NPCDriver>())
            {
                kartsList.Add(kart);
                kartCheckpointList.Add(kartCheckpoint);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        checkPlacement();
        placementDisplay.text = $"Placement: {trackedKart.placement}";
        lapDisplay.text = $"Lap: {trackedKart.lap + 1}";
    }

    private void checkPlacement()
    {
        // Sort by lap first, then checkpointId
        sortedList = kartCheckpointList.OrderByDescending(kart => kart.lap)
                                       .ThenByDescending(kart => kart.checkpointId)
                                       .ThenBy(kart => kart.distanceSquaredToNextCP)
                                       .ThenBy(kart => kart.finishTime)
                                       .ToList();

        // Assign placements
        for (int i = 0; i < sortedList.Count; i++)
        {
            if (sortedList[i] == trackedKart)
            {
                trackedKart.OnPlacementChange(i + 1);
            }

            sortedList[i].placement = i + 1; // 1st place is index 0
        }
    }

    private void LoadCheckpoints()
    {
        // imagine this grabs the reference of all the checkpoints that may or may not already exist
        foreach (Transform child in checkpointManager.GetComponentsInChildren<Transform>(true))
        {
            if (child != checkpointManager.transform) // Avoid adding the parent itself
                checkpointList.Add(child.gameObject);
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void CheckpointCrossedRpc(int checkpointId, RpcParams rpcParams = default)
    {

    }

    public void CheckpointCrossed(int checkpointId, GameObject kart)
    {

    }
    public void AddKart(GameObject kart, KartCheckpoint kartCheckpoint)
    {
        kartsList.Add(kart);
        kartCheckpointList.Add(kartCheckpoint);
    }

    public void TrackKart(KartCheckpoint kart)
    {
        trackedKart = kart;
    }
    
    /// <summary>
    /// returns kart index from user clientId
    /// </summary>
    private int FindKartIndex(ulong clientId)
    {
        for (int index = 0; index < kartCheckpointList.Count; index++)
        {
            if (kartCheckpointList[index].OwnerClientId == clientId)
            {
                return index;
            }
        }
        return -1;
    }

    /// <summary>
    /// returns kart from user clientId
    /// </summary>
    public GameObject FindKart(ulong clientId)
    {
        for (int index = 0; index < kartCheckpointList.Count; index++)
        {
            if (kartCheckpointList[index].OwnerClientId == clientId)
            {
                return kartsList[index];
            }
        }
        return null;
    }
}