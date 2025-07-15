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
//Fix issue where you are unable to type while in dev maps (and sometimes other tracks?)
//  Update: can type if you close and reopen prompt but not initially even though cursor is there
//Fix issue where when clicking a button with the mouse it thinks input has been
//  entered (likely something to do with On End Edit)
//If seen as an issue (which I think it probably is), make it so that the command
//  prompt is hidden when entering a map which happens when the prompt was already open
//  and you go into a track not using a command (Update: now not working with command either)
//  (This mildly relates to issue of prompt only appearing in some scenes when it persists)
//Add other commands (see dev keyboard shortcuts; restart command?)
//Typing letters in the input that are keybinds such as WASD and P will do their actions in game
//Trim beginning of input or ignore if first index of method is null/empty? (for if the player types
//  a space before a method name, prevent having to retype)
//Can't load into another map (or scene?) when paused, it goes to a black screen but works after you
//  unpause - make it auto-unpause/disable pause menu when load command happens
//Change map names to updated names


/// <summary>
/// Enum for each map name keyword/command
/// </summary>
public enum MapName 
{ 
    OuterLoop,
    Golisano,
    Dorm,
    FinalsBrickRoad,
    QuarterMile, //Remove?
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

    public static DevTools Instance;
    [SerializeField] public SceneLoader sceneLoader;
    //[SerializeField] public GameObject gameManagerObj;
    //[SerializeField] public GameManager gameManager;

    private List<GameObject> listeners = new List<GameObject>();

    private string textLog;
    [SerializeField] private Canvas commandPromptCanvas;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private ScrollRect scrollRect;

    private bool isScrolling;

    [SerializeField] private List<BaseItem> baseItems = new List<BaseItem>();
    private ItemHolder itemHolder;
    private BaseItem baseItem;


    // Start is called before the first frame update
    void Start()
    {
        AddListener(gameObject);

        commandPromptCanvas.enabled = false;
        textLog = "Welcome to Command Prompt\nType ShowMethods for methods " +
                "or [methodName] Options for Param options";

        //Debug.Log("length " + textLog.Length);
        //gameManager = SceneLoader.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //TODO trying to fix issue where prompt dissapears between menu scenes (this code works
        //to keep it enabled but it still doesn't show until the key is pressed again)

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
      

        //Shows and hides the canvas (prompt) when `/~ is pressed (KeyCode.Tilda not working)
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (commandPromptCanvas.enabled == false)
            {
                commandPromptCanvas.enabled = true;
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
        }

        //Turns on auto-scroll when the user isn't scrolling
        if(!isScrolling)
        {
            AutoScroll(scrollRect);
        }
        
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
                            "\nLoadScene\nGameModeChange\nClearLog";
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
                        else if (parts.Length > 1 && parts.Length < 3)
                        {
                            textLog += "\nError: No Param 2 [ItemTier] was Entered.";
                        }
                        else
                        {
                            textLog += "\nError: No Param 1 [ItemType] or Param 2 [ItemTier] were Entered.";
                        }
                        break;

                    case "ClearLog":
                        textLog = "Welcome to Command Prompt\nType ShowMethods for methods " +
                        "or [methodName] Options for Param options";
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
    /// <param name="mapName">The inputted parameter after the LoadMap command.</param>
    public void LoadMap(string mapName)
    {
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
            case "OuterLoop":
                sceneLoader.LoadScene("GSP_RITOuterLoop");
                //Can be removed if prefered, maybe when in certain modes?
                commandPromptCanvas.enabled = false;
                inputField.DeactivateInputField();
                break;
            case "Golisano":
                sceneLoader.LoadScene("GSP_Golisano");
                commandPromptCanvas.enabled = false;
                inputField.DeactivateInputField();
                break;
            case "Dorm":
                sceneLoader.LoadScene("GSP_RITDorm");
                commandPromptCanvas.enabled = false;
                inputField.DeactivateInputField();
                break;
            case "FinalsBrickRoad":
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
    /// <param name="sceneName">The inputted parameter after the LoadScene command.</param>
    public void LoadScene(string sceneName)
    {
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
                inputField.ActivateInputField();
                break;

            case "MultiSingle":
                sceneLoader.LoadScene("MultiSinglePlayerScene");
                commandPromptCanvas.enabled = true;
                inputField.ActivateInputField();
                break;

            case "ModeSelect":
                sceneLoader.LoadScene("GameModeSelectScene");
                commandPromptCanvas.enabled = true;
                inputField.ActivateInputField();
                break;

            case "PlayerKart":
                sceneLoader.LoadScene("PlayerKartScene");
                commandPromptCanvas.enabled = true;
                inputField.ActivateInputField();
                break;

            case "MapSelect":
                sceneLoader.LoadScene("MapSelectScene");
                commandPromptCanvas.enabled = true;
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
    /// <param name="gameMode">The inputted parameter after the GameModeChange command.</param>
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


    public void GiveItem(string itemType, string itemTier)
    {

        GameObject kart = GameObject.Find("Kart 1/Kart");
        if (kart != null)
        {
            itemHolder = kart.GetComponent<ItemHolder>();
        }
        else
        {
            textLog += "\nError: No Kart found in scene.";
            return;
        }

        if (Int32.TryParse(itemTier, out int tier))
        {
            itemHolder.DriverItemTier = tier;
        }
        else
        {
            textLog += "\nError: Invalid Param 2 [ItemTier] was Entered.";
            return;
        }

        switch (itemType)
        {
            //Displays all of the parameter options for the GameModeChange method
            case "Options":
                textLog += "\nOptions for Param 1 [ItemType]: ";
                foreach (GameMode type in Enum.GetValues(typeof(ItemType)))
                {
                    textLog += "\n" + type;
                }
                textLog += "\nOptions for Param 2 [ItemTier]: " +
                    "\nEnter a number 1-4";
                break;

            case "Offense":
                itemHolder.HeldItem = baseItems[0];
                itemHolder.HoldingItem = true;
                itemHolder.ApplyItemTween(itemHolder.HeldItem.itemIcon);
                itemHolder.uses = itemHolder.HeldItem.UseCount;
                baseItems[0].OnLevelUp(itemHolder.DriverItemTier);
                break;

            case "Defense":
                itemHolder.HeldItem = baseItems[1];
                itemHolder.HoldingItem = true;
                itemHolder.ApplyItemTween(itemHolder.HeldItem.itemIcon);
                itemHolder.uses = itemHolder.HeldItem.UseCount;
                break;

            case "Hazard":
                itemHolder.HeldItem = baseItems[2];
                itemHolder.HoldingItem = true;
                itemHolder.ApplyItemTween(itemHolder.HeldItem.itemIcon);
                itemHolder.uses = itemHolder.HeldItem.UseCount;
                break;

            case "Boost":
                itemHolder.HeldItem = baseItems[3];
                itemHolder.HoldingItem = true;
                itemHolder.ApplyItemTween(itemHolder.HeldItem.itemIcon);
                itemHolder.uses = itemHolder.HeldItem.UseCount;
                break;

            case "":
                textLog += "\nError: No Param 1 [ItemType] was Entered.";
                break;
            case null:
                textLog += "\nError: No Param 1 [ItemType] was Entered.";
                break;
            default:
                textLog += "\nError: No Param 1 [ItemType] was Entered.";
                break;
        }

        Debug.Log(itemHolder.DriverItemTier);
        Debug.Log(itemHolder.HeldItem);

    }



    /// <summary>
    /// Keeps scroll bar at bottom of the prompt to show the most recent inputs and outputs.
    /// </summary>
    /// <param name="scrollRect">The rect transform of the scroll view bar game object.</param>
    public void AutoScroll(ScrollRect scrollRect)
    {
        scrollRect.verticalNormalizedPosition = 0;
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

}
