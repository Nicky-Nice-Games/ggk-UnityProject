using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.UIElements.UxmlAttributeDescription;

//Gina Piccirilli

//OTHER TODOs (not listed elsewhere)

//Fix issue where when clicking a button with the mouse it thinks input has been
//  entered (likely something to do with On End Edit)
//If seen as an issue (which I think it probably is), make it so that the command
//  prompt is hidden when entering a map which happens when the prompt was already open
//  and you go into a track not using a command
//Add other commands (see dev keyboard shortcuts; restart command?)
//Typing letters in the input that are keybinds such as WASD and P will do their actions in game
//Trim beginning of input or ignore if first index of method is null/empty? (for if the player types
//  a space before a method name, prevent having to retype)
//Can't load into another map or scene when paused, it goes to a black screen but works after you
//  unpause - make it auto-unpause/disable pause menu when load command happens
//  FIX DEACTIVATE PAUSE! works to deactivate when loading a new scene but causes the same problem as 
//  activating and deactivating the command prompt, you need to press the keybind to reopen or re-close
//  the prompt or pause panel before it actually registers 
//Fix auto scroll - stops auto scrolling because the scroll view keeps changing the value, need to 
//  figure out how to only get it to stop if the player changes the value
//Ability to adjust player and npc speed
//Debug GiveItem


//Fix issue where you are unable to type while in dev maps (and sometimes other tracks?)
//  Update: can type if you close and reopen prompt but not initially even though cursor is there
//UPDATE: fixed, but only works if the prompt is reactivated, doesnt work if it stays open but works 
//  works when you load into a map and then open the prompt

//Done (?)
//Prompt persists through menu scenes


/// <summary>
/// Enum for each map name keyword/command
/// </summary>
public enum MapName 
{ 
    CampusCircuit,
    TechHouseTurnpike,
    DormRoomDerby,
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
    TimeTrial,
    Free    //Dev mode, free fly/drive ?
}


/// <summary>
/// Enum for each item category keyword/command
/// </summary>
public enum ItemType
{
    Offense,
    Defense,
    Hazard,
    Boost
}


public class DevTools : MonoBehaviour
{
    //Variables and references for setting up the DevTools object
    public static DevTools Instance;
    [SerializeField] public SceneLoader sceneLoader;
    //[SerializeField] public GameObject gameManagerObj;
    //[SerializeField] public GameManager gameManager;
    private List<GameObject> listeners = new List<GameObject>();

    //Variables and references for the visible command prompt game objects
    private string textLog;
    [SerializeField] private Canvas commandPromptCanvas;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Scrollbar scrollBar;
    private bool isScrolling;

    //Variables and references for GiveItem command
    [SerializeField] private List<BaseItem> baseItems = new List<BaseItem>();
    private ItemHolder itemHolder;
    private BaseItem baseItem;


    //[SerializeField] private GameObject pausePanel;


    // Start is called before the first frame update
    void Start()
    {
        AddListener(gameObject);

        isScrolling = false;

        commandPromptCanvas.enabled = false;
        textLog = "Welcome to Command Prompt\nType ShowMethods for methods " +
                "or \n[MethodName] Options for Param options";

        //Debug.Log("length " + textLog.Length);
        //gameManager = SceneLoader.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //trying to fix issue where prompt dissapears between menu scenes (this code works
        //to keep it enabled but it still doesn't show until the key is pressed again)
        #region Commented out testing
        //Debug.Log("loading " + sceneLoader.loading);
        //if (sceneLoader.loading)
        //{
        //inputField.ActivateInputField();
        //Debug.Log("enabled " + commandPromptCanvas.enabled);
        //Debug.Log("Length " + textLog.Length);

        //TODO FIX! won't work if log has been cleared, run secondary check?
        //  ALSO overrides disabling when entering a map
        //if (textLog.Length > 131)   
        //{
        //    commandPromptCanvas.enabled = true;
        //    inputField.ActivateInputField();
        //}
        //else
        //{
        //    commandPromptCanvas.enabled = false;
        //}
        //sceneLoader.loading = false;
        //Debug.Log("enabled " + commandPromptCanvas.enabled);
        //}
        #endregion

        //Shows and hides the canvas (prompt) when `/~ is pressed (KeyCode.Tilda not working)
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
                commandPromptCanvas.enabled = false;
                inputField.DeactivateInputField();
            }
        }

        //Clears input field, returns cursor to field, and turns on auto-scroll when the user
        //enters new input
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            inputField.text = "";
            inputField.ActivateInputField();
            isScrolling = false;
            //AutoScroll(scrollRect);
        }

        //Turns on auto-scroll when the user isn't scrolling
        if(!isScrolling)
        {
            AutoScroll(scrollRect);
        }
        //else
        //{
        //    StopAutoScroll();
        //}
        
        //Sets text of command prompt equal to the textLog variable that is added to
        textBox.text = textLog;
    }


    /// <summary>
    /// Singleton functionality for promt to remain across scenes.
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

        //Splits input into substrings with a space as the delimiter (Pre-method chaining)
        //string[] parts = input.Split(new char[] { ' ' });

        //Splits input into substrings with a colon as the delimiter
        string[] methods = input.Split(new char[] { ':' });

        //Format:
        //or MethodName Param:MethodName Param
        //  methods[0] = Method1 Param1
        //  methods[1] = Method2 Param2
        //  parts[0] = MethodName
        //  parts[1] = Param

        //For each method inputted, checks method name and parameters
        for (int i = 0; i < methods.Length; i++)
        {
            parts = methods[i].Split(new char[] { ' ' });

            if (parts != null)
            {

                switch (parts[0])
                {
                    case "ShowMethods":
                        textLog += "\nMethod Options:\nShowMethods\nLoadMap" +
                            "\nLoadScene\nGameModeChange\nGiveItem\nClearLog";
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

                    case "GameModeChange":
                        //TODO set up gamemode method command to change game mode without changing map
                        if (parts.Length > 1)
                        {
                            GameModeChange(parts[1]);
                        }
                        else
                        {
                            textLog += "\nError: No Param 1 [GameMode] was Entered.";
                        }
                        textLog += "\nMethod not yet set up :)";
                        break;

                    case "GiveItem":
                        if (parts.Length > 2)
                        {
                            GiveItem(parts[1], parts[2]);
                        }
                        else if (parts[1] == "Options")
                        {
                            textLog += "\nOptions for Param 1 [ItemType]: ";
                            foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
                            {
                                textLog += "\n" + type;
                            }
                            textLog += "\nOptions for Param 2 [ItemTier]: " +
                                "\nEnter a number 1-4";
                            break;
                        }             
                        else
                        {
                            textLog += "\nError: No or Invalid Param 1 [ItemType] or Param 2 [ItemTier] was Entered.";
                        }
                        break;

                    case "ClearLog":
                        textLog = "Welcome to Command Prompt\nType ShowMethods for methods " +
                        "or \n[MethodName] Options for Param options";
                        break;

                    default:
                        textLog += "\nError: Method Command Not Found.";
                        break;
                }
            }
        }

    }


    /// <summary>
    /// Handles the LoadMap command based on the inputted paramater entered after it. 
    /// </summary>
    /// <param name="mapName">The inputted parameter after the LoadMap command to 
    /// specify the map/track that the user wants to go to.</param>
    public void LoadMap(string mapName)
    {
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
                sceneLoader.LoadScene("GSP_RITOuterLoop");
                //Can be removed if prefered, maybe when in certain modes?
                commandPromptCanvas.enabled = false;
                inputField.DeactivateInputField();
                break;

            case "TechHouseTurnpike":
                sceneLoader.LoadScene("GSP_Golisano");
                commandPromptCanvas.enabled = false;
                inputField.DeactivateInputField();
                break;

            case "DormRoomDerby":
                sceneLoader.LoadScene("GSP_RITDorm");
                commandPromptCanvas.enabled = false;
                inputField.DeactivateInputField();
                break;

            case "AllNighterExpressway":
                sceneLoader.LoadScene("GSP_FinalsBrickRoad");
                commandPromptCanvas.enabled = false;
                inputField.DeactivateInputField();
                break;

            case "QuarterMile":
                sceneLoader.LoadScene("GSP_RITQuarterMile");
                commandPromptCanvas.enabled = false;
                inputField.DeactivateInputField();
                break;

            case "TestGrid":
                sceneLoader.LoadScene("Testing_Grid");
                commandPromptCanvas.enabled = false;
                inputField.DeactivateInputField();
                break;

            case "TestTube":
                sceneLoader.LoadScene("Testing_Tube");
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
                sceneLoader.LoadScene("StartScene");
                //Can be removed if prefered, maybe when in certain modes?
                commandPromptCanvas.enabled = true;
                inputField.DeactivateInputField();
                inputField.ActivateInputField();
                break;

            case "MultiSingle":
                sceneLoader.LoadScene("MultiSinglePlayerScene");
                commandPromptCanvas.enabled = true;
                inputField.DeactivateInputField();
                inputField.ActivateInputField();
                break;

            case "ModeSelect":
                sceneLoader.LoadScene("GameModeSelectScene");
                commandPromptCanvas.enabled = true;
                inputField.DeactivateInputField();
                inputField.ActivateInputField();
                break;

            case "PlayerKart":
                sceneLoader.LoadScene("PlayerKartScene");
                commandPromptCanvas.enabled = true;
                inputField.DeactivateInputField();
                inputField.ActivateInputField();
                break;

            case "MapSelect":
                sceneLoader.LoadScene("MapSelectScene");
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
    /// Handles the GameModeChange command based on the inputted paramater entered after it.
    /// </summary>
    /// <param name="gameMode">The inputted parameter after the GameModeChange command to 
    /// specify the game mode that the user wants to change to.</param>
    public void GameModeChange(string gameMode)
    {

        switch (gameMode)
        {
            //Displays all of the parameter options for the GameModeChange method
            case "Options":
                textLog += "\nOptions for Param 1 [GameMode]: ";
                foreach (GameMode mode in Enum.GetValues(typeof(GameMode)))
                {
                    textLog += "\n" + mode;
                }
                break;

            //TODO Mode cases go here

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

        //TODO remove? check moved to command; Check for options before tier to prevent check for tier
        if (itemType == "Options")
        {
            textLog += "\nOptions for Param 1 [ItemType]: ";
            foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
            {
                textLog += "\n" + type;
            }
            textLog += "\nOptions for Param 2 [ItemTier]: " +
                "\nEnter a number 1-4";
            return;
        }
        else
        {
            //Checks if you are in a track scene or not (such as a menu)
            GameObject kart = GameObject.Find("Kart 1/Kart");
            if (kart != null)
            {
                itemHolder = kart.GetComponent<ItemHolder>();
            }
            else
            {
                textLog += "\nError: No Kart found in scene, cannot use command.";
                return;
            }
        }

        

        //Checks if the second parameter inputted is a number 1-4
        if (Int32.TryParse(itemTier, out tier))
        {
            if(tier >= 1 && tier <= 4)
            {
                itemHolder.DriverItemTier = tier;
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
            //Displays all of the parameter options for the GiveItem method
            case "Options":
                textLog += "\nOptions for Param 1 [ItemType]: ";
                foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
                {
                    textLog += "\n" + type;
                }
                textLog += "\nOptions for Param 2 [ItemTier]: " +
                    "\nEnter a number 1-4";
                break;

            case "Offense":
                itemHolder.HeldItem = baseItems[0];                
                break;

            case "Defense":
                itemHolder.HeldItem = baseItems[1];
                break;

            case "Hazard":
                itemHolder.HeldItem = baseItems[2];              
                break;

            case "Boost":
                itemHolder.HeldItem = baseItems[3];
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

        //Setting appropriate variables for the held item, regardless of item type
        //Note: Does not run if a parameter is invalid because of return statements
        itemHolder.HoldingItem = true;
        itemHolder.HeldItem.ItemTier = tier;
        itemHolder.HeldItem.OnLevelUp(tier);
        itemHolder.ApplyItemTween(itemHolder.HeldItem.itemIcon);
        itemHolder.uses = itemHolder.HeldItem.UseCount;
    }


    /// <summary>
    /// Keeps scroll bar at bottom of the prompt to show the most recent inputs and outputs.
    /// </summary>
    /// <param name="scrollRect">The rect transform of the scroll view bar game object.</param>
    public void AutoScroll(ScrollRect scrollRect)
    {
        scrollRect.verticalNormalizedPosition = 0;
        //scrollBar.value = 0;
    }


    /// <summary>
    /// Keeps the scroll bar from trying to stay at the bottom when the user is trying to scroll 
    /// up to view previous text. Auto-scrolling continues when either the bar is scrolled back to 
    /// the bottom or the user enters another input. 
    /// </summary>
    public void StopAutoScroll()
    {
        isScrolling = true;
        scrollRect.verticalNormalizedPosition = scrollRect.verticalNormalizedPosition;

        if(scrollRect.verticalNormalizedPosition == 0)
        {
            isScrolling = false;
        }
    }


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