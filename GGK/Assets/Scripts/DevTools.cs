using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static ItemHolder;
using static UnityEngine.UIElements.UxmlAttributeDescription;

//Gina Piccirilli

//Additional/stretch tasks
//  Fix GiveItem in multiplayer (maybe make not work or all players can, etc.) - Disabled currently
//  Other possible commands: show debug messages, remove NPCs, toggle checkpoints, restart game
//  Fix Bugs:
//      Fix deactivate pause/pause issue: Can't load into another map or scene when paused, it goes
//          to a black screen but works after you unpause
//      Typing letters in the input that are keybinds will do their actions in game
//      Trim beginning of input or ignore if first index of method is null/empty? (for if the
//          player types a space before a method name on accident, prevents having to retype)
//      Fix issue where when clicking a button with the mouse it thinks input has been
//          entered (likely something to do with On End Edit), and pressing enter after the prompt is
//          closed still enters a line (even after adding additional statement to if)
//      Sometimes it will still type in the input field while driving while the prompt is closed


#region Enums
/// <summary>
/// Enum for every method command to simplify adding additional methods
/// </summary>
public enum MethodName 
{
    ShowMethods,
    LoadMap,
    LoadScene,
    GameModeChange,
    GiveItem,
    ChangeSpeed,
    ClearLog
}


/// <summary>
/// Enum for each map name keyword/command
/// </summary>
public enum MapName 
{ 
    CampusCircuit,    
    DormRoomDerby,
    TechHouseTurnpike,
    AllNighterExpressway,
    QuarterMile,
    TestGrid,
    TestTube
}


/// <summary>
/// Enum for each scene name keyword/command
/// </summary>
public enum SceneName 
{ 
    Start,
    MultiSingle,
    ModeSelect,
    PlayerKart,
    MapSelect
}


/// <summary>
/// Enum for each game mode keyword/command
/// </summary>
public enum GameMode
{
    Race,
    GrandPrix,
    TimeTrial
    //FreeDrive    //Dev mode, free fly/drive ?
}


//  Replaced with Enum from ItemHolder
///// <summary>
///// Enum for each item category keyword/command
///// </summary>
//public enum ItemType
//{
//    Offense,
//    Defense,
//    Hazard,
//    Boost
//}
#endregion

public class DevTools : MonoBehaviour
{
    #region Fields
    //Variables and references for setting up the DevTools object and references to other scripts
    public static DevTools Instance;
    [SerializeField] public SceneLoader sceneLoader;
    [SerializeField] public MultiplayerSceneManager multiSceneLoader;    
    [SerializeField] public CharacterData characterData;
    private PlacementManager placementManager;
    private AppearanceSettings appearanceSettings;
    [SerializeField] public GameManager gameManager;
    private List<GameObject> listeners = new List<GameObject>();

    //Variables for password protection
    public GameObject optionsPanel;
    private bool devToolsKeyEnabled = false;
    private int keyCount = 0;
    private KeyCode lastKey = KeyCode.Backspace;
    private KeyCode[] enablePass = 
        {KeyCode.E, KeyCode.N, KeyCode.A, KeyCode.B, KeyCode.L, KeyCode.E, KeyCode.D, KeyCode.E, 
        KeyCode.V, KeyCode.P, KeyCode.R, KeyCode.O, KeyCode.M, KeyCode.P, KeyCode.T};
    private KeyCode[] disablePass =
        {KeyCode.D, KeyCode.I, KeyCode.S, KeyCode.A, KeyCode.B, KeyCode.L, KeyCode.E, KeyCode.D, KeyCode.E,
        KeyCode.V, KeyCode.P, KeyCode.R, KeyCode.O, KeyCode.M, KeyCode.P, KeyCode.T};

    //Variables and references for the visible command prompt game objects
    private string textLog;
    private string defaultText;
    [SerializeField] private Canvas commandPromptCanvas;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Scrollbar scrollBar;
    private bool isScrolling;

    //Variable for GiveItem command
    private ItemHolder itemHolder;

    //Variables for default character model
    [SerializeField] public Sprite defaultSprite;
    [SerializeField] public Color defaultColor;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        AddListener(gameObject);

        isScrolling = false;

        commandPromptCanvas.enabled = false;
        defaultText = "Welcome to Command Prompt\nType ShowMethods for methods " +
                "or \n[MethodName] Options for Param options";
        textLog = defaultText;
    }


    // Update is called once per frame
    void Update()
    {
        //Sets the options panel variable to the currently open options panel
        optionsPanel = GameObject.FindGameObjectWithTag("Options");

        //Makes command prompt password protected for final build
        if (optionsPanel != null && optionsPanel.activeSelf == true)
        {
            if (!devToolsKeyEnabled)
            {
                if (keyCount < enablePass.Length)
                {
                    if (Input.GetKeyDown(enablePass[keyCount]))
                    {
                        Debug.Log("key pressed");
                        if (lastKey != KeyCode.Backspace)
                        {
                            Debug.Log("here");
                            if (lastKey == enablePass[keyCount - 1])
                            {
                                lastKey = enablePass[keyCount];

                                keyCount++;
                            }
                        }
                        else
                        {
                            lastKey = enablePass[keyCount];
                            keyCount++;
                        }
                    }
                }
                else if (keyCount == enablePass.Length)
                {
                    Debug.Log("enabled!");
                    devToolsKeyEnabled = true;
                    keyCount = 0;
                    lastKey = KeyCode.Backspace;
                }
            }
            else if (devToolsKeyEnabled)
            {
                if (keyCount < disablePass.Length)
                {
                    if (Input.GetKeyDown(disablePass[keyCount]))
                    {
                        Debug.Log("key pressed");
                        if (lastKey != KeyCode.Backspace)
                        {
                            Debug.Log("here");
                            if (lastKey == disablePass[keyCount - 1])
                            {
                                lastKey = disablePass[keyCount];

                                keyCount++;
                            }
                        }
                        else
                        {
                            lastKey = disablePass[keyCount];
                            keyCount++;
                        }
                    }
                }
                else if (keyCount == disablePass.Length)
                {
                    Debug.Log("disabled!");
                    devToolsKeyEnabled = false;
                    keyCount = 0;
                    lastKey = KeyCode.Backspace;
                }
            }
        }


        if (devToolsKeyEnabled) 
        {
            //Shows and hides the canvas (prompt) when `/~ is pressed and prompt is enabled
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                if (commandPromptCanvas.enabled == false)
                {
                    commandPromptCanvas.enabled = true;
                    inputField.DeactivateInputField();
                    inputField.ActivateInputField();
                }
                else
                {
                    inputField.text = "";
                    commandPromptCanvas.enabled = false;
                    inputField.DeactivateInputField();
                }
            }

        }

        //Clears input field, returns cursor to field, and turns on auto-scroll when the user
        //enters new input
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) 
            && commandPromptCanvas.enabled == true)
        {
            inputField.text = "";
            inputField.ActivateInputField();
            isScrolling = false;
            AutoScroll(scrollRect);
        }

        //Turns on auto - scroll when the user isn't scrolling
        if (!isScrolling)
        {
            AutoScroll(scrollRect);
        }

        //Sets text of command prompt equal to the textLog variable that is added to
        textBox.text = textLog;
    }


    /// <summary>
    /// Singleton functionality for prompt to remain across scenes.
    /// </summary>
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


    /// <summary>
    /// Adds a game object as a listner if it is not already in the list.
    /// </summary>
    /// <param name="listener">The game object that listens for the commands.</param>
    public void AddListener(GameObject listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }


    /// <summary>
    /// Command method is run On End Edit (String) of the UI input field and controls 
    /// what happens to the input.
    /// </summary>
    /// <param name="input">The user entered text.</param>
    public void Command(string input)
    {
        string[] parts = null;

        //Puts user entered text into the prompt log
        textLog += "\n\t>_ " + input;

        //Splits input into substrings with a colon as the delimiter
        string[] methods = input.Split(new char[] { ':' });

        /*Format:
            MethodName Param:MethodName Param
            methods[0] = Method1 Param1
            methods[1] = Method2 Param2
            parts[0] = MethodName
            parts[1] = Param */

        //For each method inputted, checks method name and parameters
        for (int i = 0; i < methods.Length; i++)
        {
            parts = methods[i].Split(new char[] { ' ' });

            if (parts != null)
            {
                switch (parts[0])
                {
                    case "ShowMethods":
                        textLog += "\nMethod Command Options: ";
                        foreach (MethodName method in Enum.GetValues(typeof(MethodName)))
                        {
                            textLog += "\n" + method;
                        }
                        break;

                    case "LoadMap":

                        if (parts.Length > 1)
                        {
                            LoadMap(parts[1]);
                        }
                        else
                        {
                            textLog += "\nError: No Param 1 [MapName] was Entered.";
                        }

                        break;

                    case "LoadScene":

                        if (parts.Length > 1)
                        {
                            LoadScene(parts[1]);
                        }
                        else
                        {
                            textLog += "\nError: No Param 1 [SceneName] was Entered.";
                        }
                        break;

                    case "ChangeGameMode":
                        if (parts.Length > 1)
                        {
                            ChangeGameMode(parts[1]);
                        }
                        else
                        {
                            textLog += "\nError: No Param 1 [GameMode] was Entered.";
                        }
                        break;

                    case "GiveItem":
                        if (parts.Length > 2)
                        {
                            GiveItem(parts[1], parts[2]);
                        }
                        else if (parts.Length > 1 && parts[1] == "Options")
                        {
                            textLog += "\nOptions for Param 1 [ItemType]: ";
                            foreach (ItemHolder.ItemTypeEnum type in Enum.GetValues(typeof(ItemHolder.ItemTypeEnum)))
                            {
                                textLog += "\n" + type;
                            }
                            textLog += "\nOptions for Param 2 [ItemTier]: " +
                                "\nEnter a number 1-4";
                            break;
                        }             
                        else
                        {
                            textLog += "\nError: No or Invalid Param 1 [ItemType] \nor Param 2 [ItemTier] was Entered.";
                        }
                        break;

                    case "ChangeSpeed":
                        if (parts.Length > 2)
                        {
                            ChangeSpeed(parts[1], parts[2]);
                        }
                        else if (parts.Length > 1 && parts[1] == "Options")
                        {
                            textLog += "\nOptions for Param 1 [KartType]: \nPlayer\nNPC\nAll";
                            
                            textLog += "\nOptions for Param 2 [Speed]: " +
                                "\nEnter a number or Reset";  
                            break;
                        }
                        else
                        {
                            textLog += "\nError: No or Invalid Param 1 [KartType] \nor Param 2 [Speed] was Entered.";
                        }
                        break;

                    case "ClearLog":
                        textLog = defaultText;
                        break;

                    default:
                        textLog += "\nError: Method Command Not Found.";
                        break;
                }
            }
        }

    }


    /// <summary>
    /// <summary>
    /// Handles the LoadMap command based on the inputted paramater entered after it. 
    /// </summary>
    /// <param name="mapName">The inputted parameter after the LoadMap command to 
    /// specify the map/track that the user wants to go to.</param>
    public void LoadMap(string mapName)
    {
        bool multiplayer = false;

        if (MultiplayerManager.Instance.IsMultiplayer) { multiplayer = true; }

        //Sets default character when loading into a map if one hasn't already been chosen
        if (characterData.characterSprite == null)
        {
            characterData.characterName = "Gizmo";
            characterData.characterColor = defaultColor;
            characterData.characterSprite = defaultSprite;             
        }

        //DeactivatePause();
        switch (mapName)
        {
            //Displays all of the parameter options for the LoadMap method
            case "Options":
                textLog += "\nOptions for Param 1 [MapName]: ";
                foreach (MapName name in Enum.GetValues(typeof(MapName)))
                {
                    textLog += "\n" + name;
                }
                break;

            case "CampusCircuit":                
                if (!multiplayer) { sceneLoader.LoadScene("LD_RITOuterLoop"); }
                else { multiSceneLoader.LoadScene("LD_RITOuterLoop"); }                
                commandPromptCanvas.enabled = false;
                inputField.DeactivateInputField();                
                break;

            case "DormRoomDerby":
                if (!multiplayer) { sceneLoader.LoadScene("LD_RITDorm"); }
                else { multiSceneLoader.LoadScene("LD_RITDorm"); }                
                commandPromptCanvas.enabled = false;
                inputField.DeactivateInputField();
                break;

            case "TechHouseTurnpike":
                if (!multiplayer) { sceneLoader.LoadScene("GSP_Golisano"); }
                else { multiSceneLoader.LoadScene("GSP_Golisano"); }
                commandPromptCanvas.enabled = false;
                inputField.DeactivateInputField();
                break;

            case "AllNighterExpressway":
                if (!multiplayer) { sceneLoader.LoadScene("GSP_FinalsBrickRoad"); }
                else { multiSceneLoader.LoadScene("GSP_FinalsBrickRoad"); }
                commandPromptCanvas.enabled = false;
                inputField.DeactivateInputField();
                break;

            case "QuarterMile":
                if (!multiplayer) { sceneLoader.LoadScene("GSP_RITQuarterMile"); }
                else { multiSceneLoader.LoadScene("GSP_RITQuarterMile"); }
                commandPromptCanvas.enabled = false;
                inputField.DeactivateInputField();
                break;

            case "TestGrid":
                if (!multiplayer) { sceneLoader.LoadScene("Testing_Grid"); }
                else { multiSceneLoader.LoadScene("Testing_Grid"); }
                commandPromptCanvas.enabled = false;
                inputField.DeactivateInputField();
                break;

            case "TestTube":
                if (!multiplayer) { sceneLoader.LoadScene("Testing_Tube"); }
                else { multiSceneLoader.LoadScene("Testing_Tube"); }
                commandPromptCanvas.enabled = false;
                inputField.DeactivateInputField();
                break;

            case "":
                textLog += "\nError: No Param 1 [MapName] was Entered.";
                break;
            case null:
                textLog += "\nError: No Param 1 [MapName] was Entered.";
                break;
            default:
                textLog += "\nError: Param 1 [MapName] Command Not Found.";
                break;
        }

    }


    /// <summary>
    /// Handles the LoadScene command based on the inputted paramater entered after it.
    /// </summary>
    /// <param name="sceneName">The inputted parameter after the LoadScene command to 
    /// specify the scene that the user wants to go to.</param>
    public void LoadScene(string sceneName)
    {

        bool multiplayer = false;

        if (MultiplayerManager.Instance.IsMultiplayer) { multiplayer = true; }

        //DeactivatePause();
        switch (sceneName)
        {
            //Displays all of the parameter options for the LoadScene method
            case "Options":
                textLog += "\nOptions for Param 1 [SceneName]: ";
                foreach (SceneName name in Enum.GetValues(typeof(SceneName)))
                {
                    textLog += "\n" + name;
                }
                break;

            case "Start":
                if (!multiplayer) { sceneLoader.LoadScene("StartScene"); }
                else { multiSceneLoader.LoadScene("StartScene"); }
                commandPromptCanvas.enabled = true;
                inputField.DeactivateInputField();
                inputField.ActivateInputField();
                break;

            case "MultiSingle":
                if (!multiplayer) { sceneLoader.LoadScene("MultiSinglePlayerScene"); }
                else { multiSceneLoader.LoadScene("MultiSinglePlayerScene"); }
                commandPromptCanvas.enabled = true;
                inputField.DeactivateInputField();
                inputField.ActivateInputField();
                break;

            case "ModeSelect":
                if (!multiplayer) { sceneLoader.LoadScene("GameModeSelectScene"); }
                else { multiSceneLoader.LoadScene("GameModeSelectScene"); }
                commandPromptCanvas.enabled = true;
                inputField.DeactivateInputField();
                inputField.ActivateInputField();
                break;

            case "PlayerKart":
                if (!multiplayer) { sceneLoader.LoadScene("PlayerKartScene"); }
                else { multiSceneLoader.LoadScene("PlayerKartScene"); }
                commandPromptCanvas.enabled = true;
                inputField.DeactivateInputField();
                inputField.ActivateInputField();
                break;

            case "MapSelect":
                if (!multiplayer) { sceneLoader.LoadScene("MapSelectScene"); }
                else { multiSceneLoader.LoadScene("MapSelectScene"); }
                commandPromptCanvas.enabled = true;
                inputField.DeactivateInputField();
                inputField.ActivateInputField();
                break;

            case "":
                textLog += "\nError: No Param 1 [SceneName] was Entered.";
                break;
            case null:
                textLog += "\nError: No Param 1 [SceneName] was Entered.";
                break;
            default:
                textLog += "\nError: Param 1 [SceneName] Command Not Found.";
                break;
        }

    }


    /// <summary>
    /// Handles the ChangeGameMode command based on the inputted paramater entered after it.
    /// </summary>
    /// <param name="gameMode">The inputted parameter after the ChangeGameMode command to 
    /// specify the game mode that the user wants to change to.</param>
    public void ChangeGameMode(string gameMode)
    {
        bool multiplayer = false;

        if (MultiplayerManager.Instance.IsMultiplayer) { multiplayer = true; }

        switch (gameMode)
        {
            //Displays all of the parameter options for the ChangeGameMode method
            case "Options":
                textLog += "\nOptions for Param 1 [GameMode]: ";
                foreach (GameMode mode in Enum.GetValues(typeof(GameMode)))
                {
                    textLog += "\n" + mode;
                }
                break;

            case "Race":
                gameManager.curGameMode = GameModes.quickRace;
                break;

            case "GrandPrix":
                //Not availible in multiplayer
                if (!multiplayer) 
                {
                    gameManager.ChangeGameMode(GameModes.grandPrix); ;
                    gameManager.GrandPrixSelected(new List<string> 
                    { "LD_RITOuterLoop", "LD_RITDorm", "GSP_Golisano", "GSP_FinalsBrickRoad" });
                }
                else { textLog += "\nError: Grand Prix not availible \nin multiplayer."; return; }                
                break;

            case "TimeTrial":
                //Not availible in multiplayer                
                if (!multiplayer) 
                { 
                    gameManager.ChangeGameMode(GameModes.timeTrial);
                    string scene = SceneManager.GetActiveScene().name;
                    LoadTimeTrialMap(scene);
                }
                else { textLog += "\nError: Time Trial not availible \nin multiplayer."; return; }
                break;

            //case "FreeDrive":
            //    //Disable NPCs, countdown/timer/leaderboard/placement?
            //    //In multiplayer, enable give item
            //    break;

            case "":
                textLog += "\nError: No Param 1 [GameMode] was Entered.";
                break;
            case null:
                textLog += "\nError: No Param 1 [GameMode] was Entered.";
                break;
            default:
                textLog += "\nError: No Param 1 [GameMode] was Entered.";
                break;
        }

    }


    /// <summary>
    /// Handles the GiveItem command based on the inputted paramaters entered after it.
    /// </summary>
    /// <param name="itemType">The first inputted parameter after the GiveItem command 
    /// to specify the item type that the user wants.</param>
    /// <param name="itemTier">The second inputted parameter after the GiveItem command 
    /// to specify the item tier that the user wants.</param>
    public void GiveItem(string itemType, string itemTier)
    {
        int tier;
        bool multiplayer = false;

        if (MultiplayerManager.Instance.IsMultiplayer) 
        { 
            multiplayer = true;
            textLog += "\nError: Command not currently supported in \nmultiplayer.";
            return;
        }

        if (gameManager.curGameMode == GameModes.timeTrial)
        {
            textLog += "\nError: Command not allowed in Time Trial.";
            return;
        }


        //Checks if you are in a track scene or not (such as a menu)
        GameObject kart = GameObject.Find("Kart 1(Clone)");
        if (kart != null)
        {
            itemHolder = kart.GetComponentInChildren<ItemHolder>();
        }
        else
        {
            textLog += "\nError: No Kart found in scene, cannot use command.";
            return;
        }


        //Checks if the second parameter inputted is a number 1-4
        if (Int32.TryParse(itemTier, out tier))
        {
            if(tier >= 1 && tier <= 4)
            {
                itemHolder.ItemTier = tier - 1;
            }
            else
            {
                textLog += "\nError: Invalid Param 2 [ItemTier] was Entered.";
                return;
            }
        }
        else
        {
            textLog += "\nError: Invalid Param 2 [ItemTier] was Entered.";
            return;
        }

        switch (itemType)
        {           
            case "Puck":             
                itemHolder.ItemType = ItemHolder.ItemTypeEnum.Puck;                
                break;

            case "Shield":
                itemHolder.ItemType = ItemHolder.ItemTypeEnum.Shield;
                break;

            case "Hazard":
                itemHolder.ItemType = ItemHolder.ItemTypeEnum.Hazard;
                break;

            case "Boost":
                itemHolder.ItemType = ItemHolder.ItemTypeEnum.Boost;
                break;

            //Removes item but keeps tier that is entered
            case "NoItem":
                itemHolder.ItemType = ItemHolder.ItemTypeEnum.NoItem;
                break;

            case "":
                textLog += "\nError: No Param 1 [ItemType] was Entered.";
                return;

            case null:
                textLog += "\nError: No Param 1 [ItemType] was Entered.";
                return;

            default:
                textLog += "\nError: No Param 1 [ItemType] was Entered.";
                return;
        }

    }


    /// <summary>
    /// Handles the ChangeSpeed command based on the inputted paramaters entered after it.
    /// </summary>
    /// <param name="kartType">The first inputted parameter after the ChangeSpeed command 
    /// to specify if the player wants to change the speed of the player or NPC karts.</param>
    /// <param name="speed">The second inputted parameter after the ChangeSpeed command 
    /// to specify the speed that the user wants to change the kart speed to.</param>
    public void ChangeSpeed(string kartType, string speed)
    {
        float speedFloat;

        if (gameManager.curGameMode == GameModes.timeTrial)
        {
            textLog += "\nError: Command not allowed in Time Trial.";
            return;
        }

        GameObject kartCheck = GameObject.FindGameObjectWithTag("Kart");
        if (!kartCheck)
        {
            textLog += "\nError: No Kart found in scene, cannot \nuse command.";
            return;
        }

        if (GameObject.Find("PlacementManager").GetComponent<PlacementManager>() != null)
        {
            placementManager = GameObject.Find("PlacementManager").GetComponent<PlacementManager>();
        }       

        //Checks if the second parameter inputted is a valid number, then sets karts speed
        if (!float.TryParse(speed, out speedFloat))
        {
            if (speed != "Reset")
            {
                textLog += "\nError: Invalid Param 2 [Speed] was Entered.";
                return;
            }
        }


        switch (kartType) 
        {
            case ("Player"):
                if(speed == "Reset")
                {
                    foreach (GameObject kart in placementManager.kartsList)
                    {
                        if (kart.GetComponent<NEWDriver>() != null)
                        {
                            kart.GetComponent<NEWDriver>().maxSpeed = 38;
                            kart.GetComponent<NEWDriver>().accelerationRate = 2700;
                        }                      
                    }
                }
                else
                {
                    foreach (GameObject kart in placementManager.kartsList)
                    {
                        if (kart.GetComponent<NEWDriver>() != null)
                        {
                            kart.GetComponent<NEWDriver>().maxSpeed = speedFloat;
                            kart.GetComponent<NEWDriver>().accelerationRate *= speedFloat / (speedFloat / 2);
                        }
                    }
                }                             
                break;

            case ("NPC"):
                if (speed == "Reset")
                {
                    foreach (GameObject kart in placementManager.kartsList)
                    {
                        if (kart.GetComponent<NPCPhysics>() != null)
                        {
                            kart.GetComponent<NPCPhysics>().maxSpeed = 38;
                            kart.GetComponent<NPCPhysics>().accelerationRate = 2300;
                        }
                    }
                }
                else
                {
                    foreach (GameObject kart in placementManager.kartsList)
                    {
                        if (kart.GetComponent<NPCPhysics>() != null)
                        {
                            kart.GetComponent<NPCPhysics>().maxSpeed = speedFloat;
                            kart.GetComponent<NPCPhysics>().accelerationRate *= speedFloat / (speedFloat / 2);
                        }
                    }
                }                
                break;

            case ("All"):
                if (speed == "Reset")
                {
                    foreach (GameObject kart in placementManager.kartsList)
                    {
                        if (kart.GetComponent<NEWDriver>() != null)
                        {
                            kart.GetComponent<NEWDriver>().maxSpeed = 38;
                            kart.GetComponent<NEWDriver>().accelerationRate = 2700;
                        }
                        if (kart.GetComponent<NPCPhysics>() != null)
                        {
                            kart.GetComponent<NPCPhysics>().maxSpeed = 38;
                            kart.GetComponent<NPCPhysics>().accelerationRate = 2300;
                        }
                    }
                }
                else
                {
                    foreach (GameObject kart in placementManager.kartsList)
                    {
                        if (kart.GetComponent<NEWDriver>() != null)
                        {
                            kart.GetComponent<NEWDriver>().maxSpeed = speedFloat;
                            kart.GetComponent<NEWDriver>().accelerationRate *= speedFloat / (speedFloat / 2);
                        }
                        if (kart.GetComponent<NPCPhysics>() != null)
                        {
                            kart.GetComponent<NPCPhysics>().maxSpeed = speedFloat;
                            kart.GetComponent<NPCPhysics>().accelerationRate *= speedFloat / (speedFloat / 2);
                        }
                    }
                }
                break;

            case "":
                textLog += "\nError: No Param 1 [KartType] was Entered.";
                return;

            case null:
                textLog += "\nError: No Param 1 [KartType] was Entered.";
                return;

            default:
                textLog += "\nError: No Param 1 [KartType] was Entered.";
                return;
        }

    }


/// <summary>
/// Helper method used in ChangeGameMode to load into the correct time trial map based 
/// on the current map.
/// </summary>
/// <param name="name">The name of the current scene.</param>
    public void LoadTimeTrialMap(string name)
    {
        switch (name) 
        {
            case "LD_RITOuterLoop":
                sceneLoader.LoadScene("TT_RITOuterLoop");
                break;

            case "LD_RITDorm":
                sceneLoader.LoadScene("TT_RITDorm");
                break;

            case "GSP_Golisano":
                sceneLoader.LoadScene("TT_Golisano");
                break;

            case "GSP_FinalsBrickRoad":
                sceneLoader.LoadScene("TT_FinalsBrickRoad");
                break;

            default:
                return;
        }
    }


    /// <summary>
    /// Keeps scroll bar at bottom of the prompt to show the most recent inputs and outputs.
    /// </summary>
    /// <param name="scrollRect">The rect transform of the scroll view bar game object.</param>
    public void AutoScroll(ScrollRect scrollRect)
    {
        isScrolling = false;
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }


    /// <summary>
    /// Keeps the scroll bar from trying to stay at the bottom when the user is trying to scroll 
    /// up to view previous text. Auto-scrolling continues when either the bar is scrolled back to 
    /// the bottom or the user enters another input. Called OnEndEdit of input field.
    /// </summary>
    public void StopAutoScroll()
    {
        isScrolling = true;
        scrollRect.verticalNormalizedPosition = scrollRect.verticalNormalizedPosition;
    }


    //Attempt at solving issue where screen turns black when trying to load somewhere else while
    //paused, just need to press escape again to fix but wanted an automatic fix
    //public void DeactivatePause()
    //{
    //    //Checks if pause panel is active and deactivates if so
    //    //GameObject pausePanel = GameObject.Find("PausePanel");
    //    if (pausePanel != null)
    //    {
    //        pausePanel.SetActive(false);
    //        Time.timeScale = 1;
    //    }

    //    //if (pausePanel != null)
    //    //{
    //    //    //pausePanel.SetActive(true);
    //    //    pausePanel.SetActive(false);
    //    //}
    //}

}