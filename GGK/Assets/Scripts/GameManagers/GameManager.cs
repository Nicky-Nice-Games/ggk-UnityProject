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
    colorSelect,
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
        if (thisManagerInstance == null)
        {
            thisManagerInstance = this;
            thisManagerObjInstance = gameObject;
            sceneLoader = thisManagerInstance.GetComponent<SceneLoader>();
            DontDestroyOnLoad(this);
        }
        else if (thisManagerInstance != this)
        {
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
        curState = GameStates.multiSingle;
        SafeLoad("MultiSinglePlayerScene");
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
            curState = GameStates.gameMode;
            sceneLoader.LoadScene("GameModeSelectScene");
        }
        else if (GetComponent<ButtonBehavior>().buttonClickedName == "Multi")
        {
            // Connect to multiplayer
            // ...

            // Will most likely be replaced when implimenting the comments above
            curState = GameStates.gameMode;
            sceneLoader.LoadScene("GameModeSelectScene");
        }
    }

    /// <summary>
    /// Called once the game mode is selected and loaded
    /// </summary>
    public void LoadedGameMode()
    {
        curState = GameStates.playerKart;
        sceneLoader.LoadScene("CharSelectMenu");
    }

    /// <summary>
    /// Holds logic for when the player selects their cart
    /// Basic for now but might need to be ediited when
    /// multyplayer is added
    /// </summary>
    public void PlayerSelected()
    {
        curState = GameStates.colorSelect;
        sceneLoader.LoadScene("ColorSelectMenu");
    }

    public void ColorSelected()
    {
        curState = GameStates.map;
        sceneLoader.LoadScene("MapSelectScene");
    }

    /// <summary>
    /// Loads the game scene
    /// </summary>
    public void MapSelected(string map)
    {
        curState = GameStates.game;
        // Loads the race based on the name of the button clicked
        switch (map)
        {
            case "Campus Circuit":
                sceneLoader.LoadScene("GSP_RITOuterLoop");
                break;
            case "Tech House Turnpike":
                sceneLoader.LoadScene("GSP_Golisano");
                break;
            case "Dorm Room Derby":
                sceneLoader.LoadScene("GSP_RITDorm");
                break;
            case "All-Nighter Expressway":
                sceneLoader.LoadScene("GSP_FinalsBrickRoad");
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Triggers when the game finishes
    /// </summary>
    public void GameFinished()
    {
        curState = GameStates.gameOver;
        sceneLoader.LoadScene("GameOverScene");
    }

    public void LoadStartMenu()
    {
        curState = GameStates.start;
        SafeLoad("StartScene");
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

    /// <summary>
    /// Loads the selected scene, re-referencing the SceneLoader variable if it is null.
    /// </summary>
    /// <param name="sceneToLoad"> The scene to load.</param>
    public void SafeLoad(string sceneToLoad)
    {
        //protects against loading the scene if it's null
        if (sceneLoader == null)
        {
            sceneLoader = thisManagerObjInstance.GetComponent<SceneLoader>();
        }
        sceneLoader.LoadScene(sceneToLoad);
    }
}


