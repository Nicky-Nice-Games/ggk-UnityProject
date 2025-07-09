using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


enum MapName 
{ 
    OuterLoop,
    Golisano,
    Dorm,
    FinalsBrickRoad,
    QuarterMile, //Remove?
    TestGrid,
    TestTube
}

enum GameMode
{
    Race,
    GrandPrix,
    TimeTrial,
    Free    //Dev mode, free fly/drive ?
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


    // Start is called before the first frame update
    void Start()
    {
        AddListener(gameObject);

        commandPromptCanvas.enabled = false;
        textLog = "Welcome to Command Prompt\nType ShowMethods for methods " +
                "or [methodName] Options for param options (Note: currently only for LoadMap)";

        //Debug.Log("length " + textLog.Length);
        //gameManager = SceneLoader.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //TODO trying to fix issue where prompt dissapears between menu scenes (this code works
        //to keep it enabled but it still doesn't show until the key is pressed again)

        //Debug.Log("loading " + sceneLoader.loading);
        if (sceneLoader.loading)
        {
            //Debug.Log("enabled " + commandPromptCanvas.enabled);
            //Debug.Log("Length " + textLog.Length);
            //if (textLog.Length > 131)
            //{
            //    commandPromptCanvas.enabled = true;
            //    inputField.ActivateInputField();

            //}
            //else
            //{
                //commandPromptCanvas.enabled = false;
            //}
            sceneLoader.loading = false;
            //Debug.Log("enabled " + commandPromptCanvas.enabled);
        }
      

        //Shows and hides the canvas (prompt) when `/~ is pressed (KeyCode.Tilda not working) !!
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
        //Puts user entered text into the prompt log
        textLog += "\n\t>_ " + input;

        //Splits input into substrings with a space as the delimiter
        string[] parts = input.Split(new char[] { ' ' });

        //Format: MethodName space MapParam space ModeParam?
        //or MethodName param : MethodName param
        //  parts[0] = methodName
        //  parts[1] = param 1 (map name)
        //  parts[2] = param 2 (game mode)


        //TODO Implement GameMode Commands
        //if (parts[2] != null)
        //{
        //    //case parts[2] check each enum mode and then load that scene
        //    //gameManager.SafeLoad()
        //}

        switch (parts[0])
        {
            case "ShowMethods":
                textLog += "\nMethod Options:\nShowMethods\nLoadMap\nGameModeChange\nClearLog";
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

            case "ClearLog":
                textLog = "Welcome to Command Prompt\nType ShowMethods for methods " +
                "or [methodName] Options for param options (Note: currently only for LoadMap)";
                break;

            default:
                textLog += "\nError: Method Command Not Found.";
                break;
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
                textLog += "\nOptions for param 1 [MapName]: ";
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
    /// Handles the GameModeChange command based on the inputted paramater entered after it.
    /// </summary>
    /// <param name="gameMode">The inputted parameter after the GameModeChange command.</param>
    public void GameModeChange(string gameMode)
    {

        switch (gameMode)
        {
            //Displays all of the parameter options for the GameModeChange method
            case "Options":
                textLog += "\nOptions for param 1 [GameMode]: ";
                foreach (GameMode mode in Enum.GetValues(typeof(GameMode)))
                {
                    textLog += "\n" + mode;
                }
                break;

            //Mode cases go here

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
