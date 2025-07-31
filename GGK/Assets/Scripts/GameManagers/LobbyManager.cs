using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// LobbyManager Class by Phillip Brown
/// </summary>
public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    /// <summary>
    /// events for other scripts to be able to tell what's going on;
    /// </summary>
    public event EventHandler OnLeftLobby;

    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public event EventHandler<LobbyEventArgs> OnKickedFromLobby;

    private GameManager gameManager;
    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }

    /// <summary>
    /// Constant player data keys that get passed around in the lobby
    /// </summary>
    public const string KEY_PLAYER_NAME = "PlayerName";
    //public const string KEY_PLAYER_CHARACTER = "Character";
    public const string KEY_START_GAME = "StartGame_RelayCode";

    /// <summary>
    /// variables to help the functions within the manager run
    /// </summary>
    private Lobby joinedLobby;
    private float refreshLobbyListTimer;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;
    private string playerName;

    /// <summary>
    /// variables to differentiate between arcade mode and general play mode
    /// </summary>
    [SerializeField] private bool arcadeToggle;
    [SerializeField] private string arcadeCode = "arcade";

    #region initalization functions
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

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        gameManager = FindAnyObjectByType<GameManager>();

        AuthenticationService.Instance.SignedIn += () =>
        {
            //Debug.Log("Signed in" + AuthenticationService.Instance.PlayerId);
        };
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            playerName = $"RIT:{UnityEngine.Random.Range(10, 999)}"; 
        }
        //Debug.Log(playerName);
        //Authenticate(playerName);
    }

    private async void Authenticate(string playerName)
    {
        this.playerName = playerName;
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () =>
        {
            //do nothing
            //Debug.Log("Signed in" + AuthenticationService.Instance.PlayerId);

            //RefreshLobbyList();
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
    #endregion

    #region looping functions
    private void Update()
    {
        //HandleRefreshLobbyList(); // Disabled for testing so that we dont hit rate limit within the same IP
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();
    }

    /// <summary>
    /// refreshes the lobby list every 5s
    /// </summary>
    private void HandleRefreshLobbyList()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn)
        {
            refreshLobbyListTimer -= Time.deltaTime;
            if (refreshLobbyListTimer < 0f)
            {
                float refreshLobbyListTimerMax = 5f;
                refreshLobbyListTimer = refreshLobbyListTimerMax;

                RefreshLobbyList();
            }
        }
    }

    /// <summary>
    /// Keeps a lobby from deleting itself from lack of input.
    /// Unity has a default timer of 30s. This pings the lobby every 15s.
    /// </summary>
    private async void HandleLobbyHeartbeat()
    {
        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    /// <summary>
    /// Lobbies dont update in real time so this grabs/updates the information on the client every 1.1s.
    /// 1s per client is the rate limit so we go 1.1s to make sure we dont hit it.
    /// </summary>
    private async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer <= 0f)
            {
                float lobbyUpdateTimerMax = 1.1f;
                lobbyUpdateTimer = lobbyUpdateTimerMax;

                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });

                if (!IsPlayerInLobby())
                {
                    // player was kicked
                    //Debug.Log("Kicked from Lobby!");

                    joinedLobby = null;

                    OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = joinedLobby });
                }

                if (joinedLobby.Data[KEY_START_GAME].Value != "0")
                {
                    // game started
                    if (!IsLobbyHost())
                    {
                        RelayManager.Instance.JoinRelay(joinedLobby.Data[KEY_START_GAME].Value);
                    }
                    joinedLobby = null;
                }
            }
        }
    }
    #endregion

    #region creating and deleting lobbies
    /// <summary>
    /// Creates a lobby on Unity Gaming Services.
    /// </summary>
    public async void CreateLobby(string lobbyName = "", int maxPlayers = 4, bool isPrivate = false)
    {
        try
        {
            if (lobbyName == "") lobbyName = playerName + "\'s MyLobby";
            Player player = GetPlayer();
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                Player = player,
                IsPrivate = isPrivate,
                Data = new Dictionary<string, DataObject>
                {
                    {KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0")}
                }
            };

            // extra information if this client is on an arcade machine
            // if (/*arcadeToggle*/ false) // for debugging purposes
            // {
            //     options.Data.Add(
            //             "Arcade", new DataObject(
            //                 visibility: DataObject.VisibilityOptions.Public,
            //                 value: arcadeCode,
            //                 index: DataObject.IndexOptions.S1
            //                 )
            //     );
            // };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            joinedLobby = lobby;
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });

            //Debug.Log("Created Lobby! " + lobby.Name + " " + lobby.MaxPlayers);
            //PrintPlayers(lobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Allows the host to delete the lobby
    /// </summary>
    private async void DeleteLobby()
    {
        try
        {
            if (!IsLobbyHost()) return;
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    #endregion

    /// <summary>
    /// updates the list of lobbies seen on the client. 
    /// And raises an event that tells all listeners the new list.
    /// privated for now until needed
    /// </summary>
    public async void RefreshLobbyList()
    {
        int numOfLobbies = 25;
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = numOfLobbies;

            // filters for open lobbies
            options.Filters = new List<QueryFilter>
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            options.Order = new List<QueryOrder>
            {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// For now Prints the list of findable lobbies to the console.
    /// Will be converted to return a list once it is needed for UI.
    /// </summary>
    private async void ListLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    #region joining lobbies

    /// <summary>
    /// Allows for joining of lobbies through an ID (can be changed to a code later). Right now set to join the first available lobby.
    /// Can be converted to have password protection in addition if needed.
    /// </summary>
    public async void JoinLobbyById(string id)
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions()
            {
                Player = GetPlayer()
            };

            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(id, options);
            joinedLobby = lobby;

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
            //PrintPlayers(lobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {

            Player player = GetPlayer();

            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions
            {
                Player = player
            });

            joinedLobby = lobby;

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Allows for Quick joining of lobbies without having to search and know a specific code. 
    /// Auto joins the first found avalible lobby that fits the search criteria.
    /// Right now it's set for arcade machines to join a lobby with a specific arcade code. And fall back to creating one if not found.
    /// Can be changed for actual Quickplay reasons.
    /// </summary>
    public async void QuickJoin()
    {
        try
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions()
            {
                Player = GetPlayer()
            };
            options.Filter = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.S1,
                    op: QueryFilter.OpOptions.EQ,
                    value: arcadeCode
                )
            };
            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            joinedLobby = lobby;
            //PrintPlayers(lobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            CreateLobby();
        }
    }

    #endregion

    #region exiting lobbies
    /// <summary>
    /// Actually removes the player from the lobby.
    /// Closing the game doesnt update the lobby on the UGS that the player has left.
    /// </summary>
    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            joinedLobby = null;
            OnLeftLobby?.Invoke(this, EventArgs.Empty);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Allows the host to kick a player.
    /// The same as leaving except they remove an index other than themselves
    /// </summary>
    public void KickPlayer()
    {
        if (!IsLobbyHost()) return;
        KickPlayer(1);
    }

    private async void KickPlayer(int index)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[index].Id);
            joinedLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        catch (IndexOutOfRangeException e)
        {
            Debug.Log(e);
        }
    }

    #endregion

    /// <summary>
    /// Prints and Lists Lobby info to console.
    /// </summary>
    public void PrintPlayers()
    {
        if (joinedLobby == null)
        {
            Debug.Log("Not in a lobby yet");
            return;
        }
        PrintPlayers(joinedLobby);
    }

    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log(lobby.Players.Count + " Players in Lobby " + lobby.Name);
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }

    #region extra lobby utilities
    /// <summary>
    /// returns player information formatted for lobby and query options
    /// </summary>
    public Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                {
                    {KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
                }
        };
    }

    public Lobby GetJoinedLobby()
    {
        return joinedLobby;
    }

    /// <summary>
    /// returns if this client is the lobby host
    /// </summary>
    public bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    /// <summary>
    /// returns if this player/client is still in the lobby they joined
    /// </summary>
    private bool IsPlayerInLobby()
    {
        if (joinedLobby != null && joinedLobby.Players != null)
        {
            foreach (Player player in joinedLobby.Players)
            {
                if (player.Id == AuthenticationService.Instance.PlayerId)
                {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion

    /// <summary>
    /// starts a relay and passes the code around through the lobby
    /// </summary>
    public async void StartGame()
    {
        if (IsLobbyHost())
        {
            try
            {
                //Debug.Log("StartGame");
                string relayCode = await RelayManager.Instance.CreateRelay();

                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                    }
                });
                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public void AssignPlayerName(string name)
    {
        if(name != "")
        {
            playerName = name;
        }
    }
}
