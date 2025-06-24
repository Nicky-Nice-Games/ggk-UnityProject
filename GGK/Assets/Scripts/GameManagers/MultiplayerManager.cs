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
    private Dictionary<ulong, Map> playerMapSelections = new Dictionary<ulong, Map>();
    public Dictionary<ulong, PlayerData> players = new Dictionary<ulong, PlayerData>();


    // timer variables for kart select and map select scenes
    [SerializeField] private const float kartSelectionTimerMax = 30f;
    private float kartSelectionTimer = 30f;
    [SerializeField] private bool runKartSelectionTimer = false;
    [SerializeField] private const float mapSelectionTimerMax = 60f;
    private float mapSelectionTimer = 60f;
    [SerializeField] private bool runMapSelectionTimer = false;

    private System.Random random = new System.Random();

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
    public void Reset()
    {
        //gamemode.Value = Gamemode.Unselected;
        kartSelectionTimer = kartSelectionTimerMax;
        mapSelectionTimer = mapSelectionTimerMax;
        ResetDictionaries();
    }

    public void ResetDictionaries()
    {
        //clear dictionaries
        playerKartSelectionChecks.Clear();
        playerMapSelectionChecks.Clear();
        playerMapSelections.Clear();
        players.Clear();
        InitPlayerDataRpc();
        Debug.Log($"number of connected clients {NetworkManager.ConnectedClientsIds.Count}");
        foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            playerKartSelectionChecks.Add(clientId, false);
            playerMapSelectionChecks.Add(clientId, false);
            playerMapSelections.Add(clientId, Map.RITOuterLoop);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void InitPlayerDataRpc()
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
    public void SetPlayerKartRpc(ulong clientId)
    {
        
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = true)]
    public void ForcePlayerKartSelectRpc()
    {
        // Auto picks the kart they are hovering
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void PlayerKartSelectedRpc(ulong clientId, PlayerKart playerCharacter, UnityEngine.Color playerColor) // colors are being sent correctly, just need to change how the player character is written to the character data script
    {
        Debug.Log($"Rpc called by clientid: {clientId}");
        playerKartSelectionChecks[clientId] = true;
        PlayerData player = players[clientId];
        player.PlayerCharacter = playerCharacter;
        player.PlayerColor = playerColor;
        players[clientId] = player;
        Debug.Log($"Client {clientId} chose {playerCharacter} in color {playerColor}");
        if (AllPlayerKartsSelected())
        {
            GameManager.thisManagerInstance.ToMapSelectScreenRpc();
        }
    }

    public bool AllPlayerKartsSelected()
    {
        bool allSelected = true;
        foreach (KeyValuePair<ulong, bool> playerChoice in playerKartSelectionChecks)
        {
            if (!playerChoice.Value) allSelected = false;
        }
        return allSelected;
    }
    #endregion

    #region map votes
    // After a selection has been made show the Map Selection Scene to everyone (after a timer auto select their last hovered option)
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void VoteMapRpc(ulong clientId, Map map)
    {
        Debug.Log($"Rpc called by clientid: {clientId}");
        playerMapSelectionChecks[clientId] = true;
        playerMapSelections[clientId] = map;
        if (AllPlayerMapVotesIn())
        {
            // pick random map
            List<Map> mapOptions = new List<Map>(playerMapSelections.Values);
            Map votedMap = mapOptions[random.Next(mapOptions.Count)];
            switch (votedMap)
            {
                case Map.RITOuterLoop:
                    GameManager.thisManagerInstance.LoadMapRpc("Netcode RIT Outer Loop Greybox");
                    break;
                case Map.Golisano:
                    GameManager.thisManagerInstance.LoadMapRpc("Golisano Greybox");
                    break;
                case Map.RITDorm:
                    GameManager.thisManagerInstance.LoadMapRpc("RIT Dorm Greybox");
                    break;
                case Map.RITWoods:
                    GameManager.thisManagerInstance.LoadMapRpc("RIT Woods Greybox");
                    break;
                default:
                    break;
            }
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void RescindVoteMapRpc(ulong clientId)
    {

    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = true)]
    public void ForceVoteMapRpc()
    {
        // Auto picks the map they are hovering
        VoteMapRpc(NetworkManager.Singleton.LocalClientId, (Map)random.Next((int)Map.RITWoods)); // random pick for now
    }

    public bool AllPlayerMapVotesIn()
    {
        bool allVoted = true;
        foreach (KeyValuePair<ulong, bool> playerChoice in playerMapSelectionChecks)
        {
            if (!playerChoice.Value) allVoted = false;
        }
        return allVoted;
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
