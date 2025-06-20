using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

/// <summary>
/// MultiplayerManager class by Phillip Brown
/// </summary>
public class MultiplayerManager : NetworkBehaviour
{
    public static MultiplayerManager Instance;

    public bool IsMultiplayer { get; set; }

    public PlayerData playerData { get; set; }
    /// <summary>
    /// Current Gamemode
    /// </summary>
    private NetworkVariable<Gamemode> gamemode = new NetworkVariable<Gamemode>(Gamemode.Unselected); // data type should be however we are representing the game modes

    /// <summary>
    /// Current selected map
    /// </summary>
    private NetworkVariable<int> selectedMap = new NetworkVariable<int>(0); // data type should be however we are representing the maps

    private Dictionary<ulong,bool> playerKartSelectionChecks = new Dictionary<ulong, bool>();
    private Dictionary<ulong,bool> playerMapSelectionChecks = new Dictionary<ulong, bool>();
    private Dictionary<ulong, PlayerData> players = new Dictionary<ulong, PlayerData>();


    // timer variables for kart select and map select scenes
    [SerializeField] private const float kartSelectionTimerMax = 30f;
    private float kartSelectionTimer = 30f;
    [SerializeField] private bool runKartSelectionTimer = false;
    [SerializeField] private const float mapSelectionTimerMax = 60f;
    private float mapSelectionTimer = 60f;
    [SerializeField] private bool runMapSelectionTimer = false;


    enum Gamemode
    {
        Unselected,
        Race
    }

    private void Awake()
    {
        IsMultiplayer = false;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        gamemode.OnValueChanged += OnGamemodeChanged;
        selectedMap.OnValueChanged += OnMapSelected;
        RelayManager.Instance.OnRelayStarted += MultiplayerStarted;
        RelayManager.Instance.OnRelayJoined += MultiplayerStarted;
    }

    private void MultiplayerStarted(object sender, EventArgs e)
    {
        IsMultiplayer = true;
    }
    private void MultiplayerEnded(object sender, EventArgs e)
    {
        IsMultiplayer = false;
    }

    #region initalization functions
    /// <summary>
    /// Call this going into a new multiplayer game (from lobby to game mode select)
    /// </summary>
    private void Reset()
    {
        gamemode.Value = Gamemode.Unselected;
        kartSelectionTimer = kartSelectionTimerMax;
        mapSelectionTimer = mapSelectionTimerMax;
        ResetDictionaries();
    }

    public void ResetDictionaries()
    {
        //clear dictionaries
        playerKartSelectionChecks.Clear();
        playerMapSelectionChecks.Clear();
        players.Clear();
        GetPlayerDataRpc();
        Debug.Log($"number of connected clients {NetworkManager.ConnectedClientsIds.Count}");
        foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            playerKartSelectionChecks.Add(clientId, false);
            playerMapSelectionChecks.Add(clientId, false);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void GetPlayerDataRpc()
    {
        SendPlayerDataRpc(NetworkManager.LocalClientId, playerData.PlayerName);
    }

    [Rpc(SendTo.Server)]
    public void SendPlayerDataRpc(ulong clientId, string name) {
        players.Add(clientId, new PlayerData(name, players.Count));
    }

    #endregion

    private void Update()
    {
        if (runKartSelectionTimer)
        {
            kartSelectionTimer -= Time.deltaTime;
            if (kartSelectionTimer <= 0f)
            {
                runKartSelectionTimer = false;
                OnKartSelectionTimerEnd();
            }
        }

        if (runMapSelectionTimer)
        {
            mapSelectionTimer -= Time.deltaTime;
            if (mapSelectionTimer <= 0f)
            {
                runMapSelectionTimer = false;
                OnMapSelectionTimerEnd();
            }
        }

        if (IsHost)
        {
            //if (allVotesIn.Value == false)
            {
                // calculate if all the votes are in
            }
        }
    }

    // Show Gamemode select to host, show Clients a waiting screen
    private void OnGamemodeChanged(Gamemode previous, Gamemode current)
    {
        if (gamemode.Value == Gamemode.Unselected) return;
        // transition all players to the kart select scene
        
        Debug.Log(IsHost ? "I chose Race gamemode" : "the Host chose Race gamemode");
    }

    #region timer events
    private void OnKartSelectionTimerEnd()
    {
        // if the client hasnt made a choice
        // force pick the option they were hovering
    }

    private void OnMapSelectionTimerEnd()
    {
        // if the client doesnt have a map vote casted
        // force pick the map they are hovering
    }
    #endregion 

    #region kart selection
    // Once the game mode is selected, Show Player Kart Selection Scene to everyone (after a timer auto select their last hovered option)
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void SetPlayerKartRpc(/*parameters for the player kart data and who the data should be set on*/)
    {
        
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = true)]
    public void ForcePlayerKartSelectRpc()
    {
        // Auto picks the kart they are hovering
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void PlayerKartSelectedRpc(ulong clientId)
    {
        Debug.Log($"Rpc called by clientid: {clientId}");
        playerKartSelectionChecks[clientId] = true;
        if (AllPlayerKartsSelected())
        {
            GameManager.thisManagerInstance.ToMapSelectScreenRpc();
        }
    }

    public bool AllPlayerKartsSelected()
    {
        bool allSelected = true;
        string str = "here are player choices\n";
        foreach (KeyValuePair<ulong, bool> playerChoice in playerKartSelectionChecks)
        {
            str += $"client: {playerChoice.Key}, decision: {playerChoice.Value}\n";
            if (!playerChoice.Value) allSelected = false;
        }
        Debug.Log(str);
        return allSelected;
    }
    #endregion

    #region map votes
    // After a selection has been made show the Map Selection Scene to everyone (after a timer auto select their last hovered option)
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void VoteMapRpc(/*parameters for the Map choice and who the data should be set on*/)
    {

    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void RescindVoteMapRpc(/*parameters for who wants their vote rescinded*/)
    {

    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = true)]
    public void ForceVoteMapRpc()
    {
        // Auto picks the map they are hovering
    }
    #endregion

    #region map selection
    // Once everyone has selected a map, randomly choose one of the maps out of the selected options and continue onto the race with their options passed along
    public void SelectMap()
    {
        // randomly pick a map from the pool of maps voted for, maps with more votes have a higher chance of being picked
        // set selected map to a value that all other clients can see
        selectedMap.Value = 0;
    }

    private void OnMapSelected(int previous, int current)
    {
        // when the map is selected this event will fire and let the clients know what the map is
        // transition everyone to the race scene
    }
    #endregion 
}
