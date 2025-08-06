using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

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

    private Dictionary<ulong, bool> playerKartSelectionChecks = new Dictionary<ulong, bool>();
    private Dictionary<ulong, bool> playerMapSelectionChecks = new Dictionary<ulong, bool>();
    private Dictionary<ulong, Map> playerMapSelections = new Dictionary<ulong, Map>();
    public Dictionary<ulong, PlayerData> players = new Dictionary<ulong, PlayerData>();

    // timer variables for kart select and map select scenes
    [SerializeField] private const float kartSelectionTimerMax = 30f;
    private float kartSelectionTimer = 30f;
    [SerializeField] private bool runKartSelectionTimer = false;
    [SerializeField] private const float mapSelectionTimerMax = 60f;
    private float mapSelectionTimer = 60f;
    [SerializeField] private bool runMapSelectionTimer = false;

    // timer visual
    private GameObject timer;
    private TextMeshProUGUI countdown;
    private Image fill;

    // Multiplayer Tracking
    [SerializeField] GameObject multiplayerPanel;
    [SerializeField] GameObject playerPanelItemTemplate;
    private Dictionary<ulong, GameObject> playerPanelItems = new Dictionary<ulong, GameObject>();

    private System.Random random = new System.Random();

    enum Gamemode
    {
        Unselected,
        Race
    }

    #region initalization functions
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
        SceneManager.sceneLoaded += FindTimer;
        SceneManager.sceneLoaded += SceneTransitionPanelUpdate;
        multiplayerPanel.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        IsMultiplayer = true;
    }

    public override void OnNetworkDespawn()
    {
        IsMultiplayer = false;
    }

    /// <summary>
    /// Call this going into a new multiplayer game (from lobby to game mode select)
    /// </summary>
    public void Reset()
    {
        //gamemode.Value = Gamemode.Unselected;
        kartSelectionTimer = kartSelectionTimerMax;
        mapSelectionTimer = mapSelectionTimerMax;
        if (IsServer)
        {
            ResetDictionaries();
        }
    }

    public void ResetDictionaries()
    {
        // clear dictionaries
        playerKartSelectionChecks.Clear();
        playerMapSelectionChecks.Clear();
        playerMapSelections.Clear();
        playerPanelItems.Clear();
        players.Clear();

        // clear multiplayer panel
        int childCount = multiplayerPanel.transform.childCount;
        for (int i = childCount - 1; i > 0; i++)
        {
            Destroy(multiplayerPanel.transform.GetChild(i).gameObject);
        }

        // Initilization
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
        if (!IsMultiplayer) return;

        // Timers
        if (runKartSelectionTimer)
        {
            kartSelectionTimer -= Time.deltaTime;
            countdown.text = ((int)kartSelectionTimer).ToString();
            fill.fillAmount = kartSelectionTimer / kartSelectionTimerMax;

            if (kartSelectionTimer <= 0f)
            {
                runKartSelectionTimer = false;
                OnKartSelectionTimerEnd();
            }
        }
        if (runMapSelectionTimer)
        {
            mapSelectionTimer -= Time.deltaTime;
            countdown.text = ((int)mapSelectionTimer).ToString();
            fill.fillAmount = mapSelectionTimer / mapSelectionTimerMax;
            if (mapSelectionTimer <= 0f)
            {
                runMapSelectionTimer = false;
                OnMapSelectionTimerEnd();
            }
        }

        // Toggle multiplayer panel
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (multiplayerPanel.activeSelf)
            {
                multiplayerPanel.SetActive(false);
            }
            else
            {
                multiplayerPanel.SetActive(true);
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

    // player panel stuff
    [Rpc(SendTo.Server)]
    public void AddPlayerToPanelRpc()
    {
        foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            AddPlayerToPanelRpc(clientId, players[clientId].PlayerName);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void AddPlayerToPanelRpc(ulong clientId, FixedString128Bytes name)
    {
        if (!playerPanelItems.ContainsKey(clientId))
        {
            GameObject tempPanelItem = Instantiate(playerPanelItemTemplate);

            tempPanelItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name.ToString();

            tempPanelItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Selecting";

            /* Turn this on if multiplayer panel should be built during game mode select
            if (clientId == 0)
            {
                tempPanelItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Choosing GameMode";
            }
            else
            {
                tempPanelItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Waiting";
            }
            */

            tempPanelItem.transform.parent = multiplayerPanel.transform;
            tempPanelItem.transform.localScale = Vector3.one;

            playerPanelItems[clientId] = tempPanelItem;
        }
    }

    private void SceneTransitionPanelUpdate(Scene s, LoadSceneMode l)
    {
        SceneTransitionPanelUpdate();
    }

    private void SceneTransitionPanelUpdate()
    {
        if (!IsMultiplayer) return;

        /*
        if (SceneManager.GetActiveScene().name == "GameModeSelectScene" || SceneManager.GetActiveScene().name == "GameOverScene")
        {
            multiplayerPanel.SetActive(true);
        }
        */
        if (SceneManager.GetActiveScene().name == "PlayerKartScene" || SceneManager.GetActiveScene().name == "MapSelectScene")
        {
            multiplayerPanel.SetActive(true);

            int childCount = multiplayerPanel.transform.childCount;
            for (int i = childCount - 1; i > 0; i--)
            {
               multiplayerPanel.transform.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text = "Selecting";
            }
        }
        else
        {
            multiplayerPanel.SetActive(false);
        }
    }

    #region timer events
    private void OnKartSelectionTimerEnd()
    {
        // if the client hasnt made a choice
        // force pick the option they were hovering
        ForcePlayerKartSelectRpc();
    }

    private void OnMapSelectionTimerEnd()
    {
        // if the client doesnt have a map vote casted
        // force pick the map they are hovering
        ForceVoteMapRpc();
    }

    private void FindTimer(Scene s, LoadSceneMode l)
    {
        FindTimer();
    }

    private void FindTimer()
    {
        // set run timer false when entering new scene
        runKartSelectionTimer = false;
        runMapSelectionTimer = false;

        // If in PlayerKartScene set timer to kartSelectionTimerMax and make it run
        if (SceneManager.GetActiveScene().name == "PlayerKartScene")
        {
            timer = GameObject.Find("Timer");

            if (IsMultiplayer)
            {
                countdown = timer.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                fill = timer.transform.GetChild(2).GetComponent<Image>();
                
                runKartSelectionTimer = true;
                kartSelectionTimer = kartSelectionTimerMax;
                countdown.text = kartSelectionTimerMax.ToString();
                fill.fillAmount = 1;
            }
            else
            {
                timer.SetActive(false);
            }
        }
        // If in MapSelectScene set timer to mapSelectionTimerMax and make it run
        else if (SceneManager.GetActiveScene().name == "MapSelectScene")
        {
            timer = GameObject.Find("Timer");

            if (IsMultiplayer)
            {
                countdown = timer.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                fill = timer.transform.GetChild(2).GetComponent<Image>();

                runMapSelectionTimer = true;
                mapSelectionTimer = mapSelectionTimerMax;
                countdown.text = mapSelectionTimerMax.ToString();
                fill.fillAmount = 1;
            }
            else
            {
                timer.SetActive(false);
            }
        }
    }


    #endregion

    #region kart selection
    // Once the game mode is selected, Show Player Kart Selection Scene to everyone (after a timer auto select their last hovered option)
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void PlayerKartSelectedRpc(String characterName, UnityEngine.Color characterColor, RpcParams rpcParams = default) // colors are being sent correctly, just need to change how the player character is written to the character data script
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        Debug.Log($"Rpc called by clientid: {senderClientId}");
        playerKartSelectionChecks[senderClientId] = true;
        PlayerData player = players[senderClientId];
        //player.PlayerCharacter = playerCharacter;
        player.CharacterColor = characterColor;
        player.CharacterName = characterName; //DELETE IF NOT WORKY
        players[senderClientId] = player;
        UpdatePlayerPanelRpc(senderClientId);
        Debug.Log($"Client {senderClientId} chose {characterName} in color {characterColor}");

        //checking if all players have made a selection
        if (AllPlayerKartsSelected())
        {
            MultiplayerSceneManager.Instance.ToMapSelectScene();
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

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = true)]
    public void ForcePlayerKartSelectRpc()
    {
        // Auto picks the kart they are hovering
        PlayerKartSelectedRpc(CharacterData.Instance.characterName, CharacterData.Instance.characterColor); // Most recently selected character and color for now
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void UpdatePlayerPanelRpc(ulong clientId)
    {
        playerPanelItems[clientId].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Confirmed";
    }
    #endregion

    #region map votes
    // After a selection has been made show the Map Selection Scene to everyone (after a timer auto select their last hovered option)
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void VoteMapRpc(Map map, RpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        Debug.Log($"Rpc called by clientid: {senderClientId}");
        playerMapSelectionChecks[senderClientId] = true;
        playerMapSelections[senderClientId] = map;
        UpdateMapPanelRpc(senderClientId, map);
        if (AllPlayerMapVotesIn())
        {
            // pick random map
            List<Map> mapOptions = new List<Map>(playerMapSelections.Values);
            Map votedMap = mapOptions[random.Next(mapOptions.Count)];
            switch (votedMap)
            {
                case Map.RITOuterLoop:
                    //GameManager.thisManagerInstance.LoadMapRpc("Netcode RIT Outer Loop Greybox");
                    MultiplayerSceneManager.Instance.ToRITOuterLoop();
                    break;
                case Map.Golisano:
                    //GameManager.thisManagerInstance.LoadMapRpc("Golisano Greybox");
                    MultiplayerSceneManager.Instance.ToGolisano();
                    break;
                case Map.RITDorm:
                    //GameManager.thisManagerInstance.LoadMapRpc("RIT Dorm Greybox");
                    MultiplayerSceneManager.Instance.ToRITDorm();
                    break;
                case Map.RITWoods:
                    //GameManager.thisManagerInstance.LoadMapRpc("RIT Woods Greybox");
                    break;
                case Map.RITQuarterMile:
                    //GameManager.thisManagerInstance.LoadMapRpc("RIT Quarter Mile Greybox V2");
                    //MultiplayerSceneManager.Instance.ToRITQuarterMile();
                    break;
                case Map.FinalsBrickRoad:
                    //GameManager.thisManagerInstance.LoadMapRpc("Finals Brick Road Greybox");
                    MultiplayerSceneManager.Instance.ToFinalsBrickRoad();
                    break;
                default:
                    break;
            }

            //clear the selections so they can vote again next time
            playerMapSelections.Clear();
            playerMapSelectionChecks.Clear();

            foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
            {
                playerKartSelectionChecks.Add(clientId, false);
                playerMapSelectionChecks.Add(clientId, false);
                playerMapSelections.Add(clientId, Map.RITOuterLoop);
            }
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void RescindVoteMapRpc(RpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        playerMapSelectionChecks[senderClientId] = false;
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = true)]
    public void ForceVoteMapRpc()
    {
        // Auto picks the map they are hovering
        VoteMapRpc((Map)random.Next((int)Map.RITWoods)); // random pick for now
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

    [Rpc(SendTo.ClientsAndHost)]
    public void UpdateMapPanelRpc(ulong clientId, Map map)
    {
        playerPanelItems[clientId].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = map.ToString();
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
