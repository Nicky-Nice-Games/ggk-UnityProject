using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// MultiplayerManager class by Phillip Brown
/// </summary>
public class MultiplayerManager : NetworkBehaviour
{
    public static MultiplayerManager Instance;

    /// <summary>
    /// Current Gamemode
    /// </summary>
    private NetworkVariable<Gamemode> gamemode = new NetworkVariable<Gamemode>(Gamemode.Unselected); // data type should be however we are representing the game modes

    /// <summary>
    /// Current selected map
    /// </summary>
    private NetworkVariable<int> selectedMap = new NetworkVariable<int>(0); // data type should be however we are representing the maps

    /*
    // variable of everyone's kart decision
    private string[] kartChoices;
    //private NetworkVariable<List<Dictionary<int, string>>> kartChoices = new NetworkVariable<List<Dictionary<int, string>>>();
    

    // variable of everyone's map vote 
    private int[] mapChoices;
    // private NetworkVariable<List<Dictionary<int,string>>> mapChoices = new NetworkVariable<List<Dictionary<int, string>>>();

    // private NetworkVariable<bool> allVotesIn = new NetworkVariable<bool>(false);

    // player list
    public List<Player> Players = new List<Player>();
    private Player[] players;
    */


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

    public override void OnNetworkSpawn()
    {
        gamemode.OnValueChanged += OnGameodeChanged;
        selectedMap.OnValueChanged += OnMapSelected;
    }

    public override void OnNetworkDespawn()
    {
        gamemode.OnValueChanged -= OnGameodeChanged;
        selectedMap.OnValueChanged -= OnMapSelected;
    }

    /// <summary>
    /// Call this going into a new multiplayer game (from lobby to game mode select)
    /// </summary>
    private void Reset()
    {
        gamemode.Value = Gamemode.Unselected;
        kartSelectionTimer = kartSelectionTimerMax;
        mapSelectionTimer = mapSelectionTimerMax;
    }


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


    private void SetPlayerData(ulong clientId, PlayerObjectData playerObject) // this implmentation is wrong im pretty sure, I need to do more research
    {

    }

    // Show Gamemode select to host, show Clients a waiting screen
    private void OnGameodeChanged(Gamemode previous, Gamemode current)
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
