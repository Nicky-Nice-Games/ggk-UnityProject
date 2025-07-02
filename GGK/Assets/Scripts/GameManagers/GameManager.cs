using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Enum to hold the various game states
public enum GameStates
{
    start,
    login,
    multiSingle,
    lobby,
    gameMode,
    playerKart,
    map,
    game,
    gameOver
}

public class GameManager : NetworkBehaviour
{
    public GameStates curState;
    public static GameManager thisManagerInstance;
    public static GameObject thisManagerObjInstance;
    public SceneLoader sceneLoader;
    public List<PlayerInfo> playerList;
    private APIManager apiManager;

    //the first button that should be selected should a controller need input
    public GameObject currentSceneFirst;

    void Awake()
    {
        if (thisManagerInstance == null)
        {
            thisManagerInstance = this;
            thisManagerObjInstance = gameObject;
            DontDestroyOnLoad(thisManagerObjInstance);
        } else if(thisManagerInstance != this){
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        curState = GameStates.start;

        //add functions to device config change and scene loaded events
        InputSystem.onDeviceChange += RefreshSelected;
        SceneManager.sceneLoaded += RefreshSelected;
        RelayManager.Instance.OnRelayStarted += RelayManager_OnRelayStarted;
        RelayManager.Instance.OnRelayJoined += RelayManager_OnRelayJoined;
        //refresh selected for the first scene since it doesn't get called for this scene
        RefreshSelected();

        apiManager = GetComponent<APIManager>();
    }

    /// <summary>
    /// Changes the game state to login scene
    /// </summary>
    public void StartGame()
    {
        sceneLoader.LoadScene("Login");
        curState = GameStates.login;
        playerList = new List<PlayerInfo>();
    }

    /// <summary>
    /// Once the player is logged in the player will be moved to the multi / single player select scene
    /// </summary>
    public void LoggedIn()
    {
        // Turn guest mode on
        if(GetComponent<ButtonBehavior>().buttonClickedName == "Guest Log In")
        {
            playerList[playerList.Count - 1].isGuest = true;
            Debug.Log("Guest mode on");
        }

        // Set validate player info
        else
        {
            ValidatePlayer(playerList[playerList.Count - 1]);
        }
        sceneLoader.LoadScene("MultiSinglePlayerScene");
        curState = GameStates.login;
    }

    /// <summary>
    /// Connects the player to either a single or multiplayer lobby
    /// Could prob be put into 2 functions in needed and even in a script attached
    /// to the new scene like the player/Kart script
    /// </summary>
    public void MultiSingleConnect()
    {
        if (GetComponent<ButtonBehavior>().buttonClickedName == "Single")
        {
            // Proceed with single player
            // ...

            // Will most likely be replaced when implimenting the comments above
            sceneLoader.LoadScene("GameModeSelectScene");
            curState = GameStates.gameMode;
        }
        else if (GetComponent<ButtonBehavior>().buttonClickedName == "Multi")
        {
            // Connect to multiplayer
            // ...

            // Will most likely be replaced when implimenting the comments above
            SceneManager.LoadScene("MultiplayerMenus");
            curState = GameStates.lobby;
        }
    }

    /// <summary>
    /// RelayManager OnRelayStarted EventHandler
    /// written by Phillip Brown
    /// </summary>
    public void RelayManager_OnRelayStarted(object sender, EventArgs e)
    {
        ToGameModeSelectScene();
    }

    public void RelayManager_OnRelayJoined(object sender, EventArgs e)
    {
        ToGameModeSelectScene();
    }
    
    /// <summary>
    /// changes game state to the game mode selection scene
    /// </summary>
    public void ToGameModeSelectScene()
    {
        SceneManager.LoadScene("GameModeSelectScene");
        curState = GameStates.gameMode;
    }

    /// <summary>
    /// Called once the game mode is selected and loaded
    /// </summary>
    public void LoadedGameMode()
    {
        sceneLoader.LoadScene("PlayerKartScene");
        curState = GameStates.playerKart;
        if (MultiplayerManager.Instance.IsMultiplayer)
        {
            MultiplayerManager.Instance.Reset();
            if (IsHost)
            {
                LoadedGameModeRpc();
            }
        }
    }

    [Rpc(SendTo.NotServer)]
    public void LoadedGameModeRpc()
    {
        SceneManager.LoadScene("PlayerKartScene");
        curState = GameStates.playerKart;
    }

    /// <summary>
    /// Holds logic for when the player selects their cart
    /// Basic for now but might need to be ediited when
    /// multiplayer is added
    /// </summary>
    public void PlayerSelected()
    {
        if (MultiplayerManager.Instance.IsMultiplayer)
        {
            MultiplayerManager.Instance.PlayerKartSelectedRpc(NetworkManager.Singleton.LocalClientId, CharacterData.Instance.character, CharacterData.Instance.characterColor);
        }
        else
        {
            ToMapSelectScreen();
        }
    }

    public void ToMapSelectScreen() {
        SceneManager.LoadScene("MapSelectScene");
        curState = GameStates.map;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ToMapSelectScreenRpc()
    {
        SceneManager.LoadScene("MapSelectScene");
        curState = GameStates.map;
    }

    /// <summary>
    /// Loads the game scene
    /// </summary>
    public void MapSelected()
    {
        if (MultiplayerManager.Instance.IsMultiplayer)
        {
            switch (GetComponent<ButtonBehavior>().buttonClickedName)
            {
                case "RIT Outer Loop":
                    MultiplayerManager.Instance.VoteMapRpc(Map.RITOuterLoop);
                    break;
                case "Golisano":
                    MultiplayerManager.Instance.VoteMapRpc(Map.Golisano);
                    break;
                case "RIT Dorm":
                    MultiplayerManager.Instance.VoteMapRpc(Map.RITDorm);
                    break;
                case "RIT Woods Greybox":
                    MultiplayerManager.Instance.VoteMapRpc(Map.RITWoods);
                    break;
                case "RIT Quarter Mile ":
                    MultiplayerManager.Instance.VoteMapRpc(Map.RITQuarterMile);
                    break;
                case "Finals Brick Road ":
                    MultiplayerManager.Instance.VoteMapRpc(Map.FinalsBrickRoad);
                    break;
                default:
                    break;
            }      
        }
        else
        {
           // Loads the race based on the name of the button clicked
            switch (GetComponent<ButtonBehavior>().buttonClickedName)
            {
                case "RIT Outer Loop":
                sceneLoader.LoadScene("V2 RIT Outer Loop Greybox");
                break;
            case "Golisano":
                sceneLoader.LoadScene("LevelDesign_GolisanoGreybox");
                break;
            case "RIT Dorm":
                sceneLoader.LoadScene("Dorm_LevelDesign");
                break;
            case "RIT Quarter Mile":
                sceneLoader.LoadScene("RIT Quarter Mile Greybox V2");
                break;
            case "Finals Brick Road":
                sceneLoader.LoadScene("LevelDesign_Finals Brick Road Greybox");
                break;
            default:
                break;
            }
            curState = GameStates.game; 
        }
        

    }

    [Rpc(SendTo.ClientsAndHost)]
    public void LoadMapRpc(string mapName)
    {
        SceneManager.LoadScene(mapName);
    }
    /// <summary>
    /// Triggers when the game finishes
    /// </summary>
    public void GameFinished()
    {
        sceneLoader.LoadScene("GameOverScene");
        curState = GameStates.gameOver;
    }

    public void LoadStartMenu()
    {
        sceneLoader.LoadScene("StartScene");
        curState = GameStates.start;
    }


    public void RefreshSelected()
    {
        // get the current EventSystem
        EventSystem cur = EventSystem.current;

        // if the current EventSystem has a first object..
        if (cur.firstSelectedGameObject)
        {
            // ..store it in the currentSceneFirst variable
            currentSceneFirst = cur.firstSelectedGameObject;
        }

        // if no controllers detected..
        if (Gamepad.all.Count == 0)
        {
            // deselect all buttons
            cur.firstSelectedGameObject = null;
            cur.SetSelectedGameObject(null);
        }
        // otherwise(aka. if a controller is detected)...
        else
        {
            // if there is not a first selected gameObject and a currently stored firstSelected
            // (aka was it removed previously?)...
            if (!cur.firstSelectedGameObject && currentSceneFirst)
            {
                //set the first selected back
                cur.firstSelectedGameObject = currentSceneFirst;
            }
            //set the current scene's selected game object to the first
            cur.SetSelectedGameObject(cur.firstSelectedGameObject);
        }
    }

    //refresh selected should a new scene be loaded
    public void RefreshSelected(Scene s, LoadSceneMode l)
    {
        RefreshSelected();
    }

    //refresh selected should a device confirguration be changed
    public void RefreshSelected(InputDevice device, InputDeviceChange change)
    {
        RefreshSelected();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void ValidatePlayer(PlayerInfo player)
    {
        // Getting the players data
        apiManager.GetPlayerWithNamePass(player.playerName, player.playerPassword, player);
    }

    /// <summary>
    /// Searches the playerList for a matching player data via client ID
    /// </summary>
    /// <param name="thisClientID"></param>
    /// <returns></returns>
    public PlayerInfo AssignDataToDriver(ulong thisClientID)
    {
        // Finding a player info sheet with the same client ID
        foreach(PlayerInfo player in playerList)
        {
            if(player.clientID == thisClientID)
            {
                return player;
            }
        }

        // None are found
        return null;
    }
}