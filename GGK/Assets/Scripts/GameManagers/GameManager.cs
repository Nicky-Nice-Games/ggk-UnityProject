using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Enum to hold the various game states
public enum GameStates
{
    start,
    multiSingle,
    gameMode,
    playerKart,
    map,
    game,
    gameOver
}

public class GameManager : MonoBehaviour
{
    public GameStates curState;
    public static GameManager thisManagerInstance;
    public static GameObject thisManagerObjInstance;
    public SceneLoader sceneLoader;
    //the first button that should be selected should a controller need input
    public GameObject currentSceneFirst;
    void Awake()
    {
        thisManagerInstance = this;
        thisManagerObjInstance = gameObject;
        DontDestroyOnLoad(thisManagerObjInstance);
    }

    // Start is called before the first frame update
    void Start()
    {
        curState = GameStates.start;

        //add functions to device config change and scene loaded events
        InputSystem.onDeviceChange += RefreshSelected;
        SceneManager.sceneLoaded += RefreshSelected;

        //refresh selected for the first scene since it doesn't get called for this scene
        RefreshSelected();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Changes the game state to multi /single player select
    /// </summary>
    public void StartGame()
    {
        sceneLoader.LoadScene("MultiSinglePlayerScene");
        curState = GameStates.multiSingle;
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
            sceneLoader.LoadScene("GameModeSelectScene");
            curState = GameStates.gameMode;
        }
    }

    /// <summary>
    /// Called once the game mode is selected and loaded
    /// </summary>
    public void LoadedGameMode()
    {
        sceneLoader.LoadScene("PlayerKartScene");
        curState = GameStates.playerKart;
    }

    /// <summary>
    /// Holds logic for when the player selects their cart
    /// Basic for now but might need to be ediited when
    /// multyplayer is added
    /// </summary>
    public void PlayerSelected()
    {
        sceneLoader.LoadScene("MapSelectScene");
        curState = GameStates.map;
    }

    /// <summary>
    /// Loads the game scene
    /// </summary>
    public void MapSelected()
    {
        // Loads the race based on the name of the button clicked
        switch (GetComponent<ButtonBehavior>().buttonClickedName)
        {
            case "RIT Outer Loop Greybox":
                sceneLoader.LoadScene("RIT Outer Loop Greybox");
                break;
            case "Golisano Greybox":
                sceneLoader.LoadScene("Golisano Greybox");
                break;
            case "RIT Dorm Greybox":
                sceneLoader.LoadScene("RIT Dorm Greybox");
                break;
            case "RIT Woods Greybox":
                sceneLoader.LoadScene("RIT Woods Greybox");
                break;
            case "RIT Quarter Mile Greybox":
                sceneLoader.LoadScene("RIT Quarter Mile Greybox V2");
                break;
            case "Finals Brick Road Greybox":
                sceneLoader.LoadScene("Finals Brick Road Greybox");
                break;
            default:
                break;
        }
        curState = GameStates.game;
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

}