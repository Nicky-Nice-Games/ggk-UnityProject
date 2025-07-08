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
                "or [methodName] options for param options (Note: currently only for LoadMap)";

        //Debug.Log("length " + textLog.Length);
        //gameManager = SceneLoader.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("loading " + sceneLoader.loading);
        if (sceneLoader.loading)
        {
            //Debug.Log("enabled " + commandPromptCanvas.enabled);
            //Debug.Log("Length " + textLog.Length);
            if (textLog.Length > 131)
            {
                commandPromptCanvas.enabled = true;
                inputField.ActivateInputField();

            }
            //else
            //{
                //commandPromptCanvas.enabled = false;
            //}
            sceneLoader.loading = false;
            //Debug.Log("enabled " + commandPromptCanvas.enabled);
        }
      

        //Shows and hides the canvas (prompt) when ~ is pressed (KeyCode.Tilda not working) !!
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            //Debug.Log("Pressed");
            if (commandPromptCanvas.enabled == false)
            {
                //Debug.Log("false");
                commandPromptCanvas.enabled = true;
                inputField.ActivateInputField();
            }
            else
            {
                //Debug.Log("true");
                commandPromptCanvas.enabled = false;
                inputField.DeactivateInputField();
            }
            
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            inputField.text = "";
            inputField.ActivateInputField();
            isScrolling = false;
        }

        if(!isScrolling)
        {
            AutoScroll(scrollRect);
        }
        
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
    /// <param name="listener"></param>
    public void AddListener(GameObject listener)
    {

        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }


    public void Command(string input)
    {
        textLog += "\n\t>_ " + input;

        //Splits input into substrings with a space as the delimiter
        string[] parts = input.Split(new char[] { ' ' });

        //Format: methodName space mapParam space modeParam
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
                textLog += "\nMethod not yet set up :)";
                break;

            case "ClearLog":
                textLog = "Welcome to Command Prompt\nType ShowMethods for options";
                break;

            default:
                textLog += "\nError: Method Command Not Found.";
                break;

        }


    }

    //textLog +=  "\nOptions: ";
    //foreach (MapName mapName in Enum.GetValues(typeof(MapName)))
    //{
    //    textLog += "\n" + mapName;
    //}

    public void LoadMap(string mapName)
    {
        switch (mapName)
        {
            case "options":
                textLog += "\nOptions for param 1 [mapName]: ";
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


    public void AutoScroll(ScrollRect scrollRect)
    {
        //Keeps scroll bar at bottom to show most recent text
        scrollRect.verticalNormalizedPosition = 0;
    }

    public void StopAutoScroll()
    {
        isScrolling = true;
        //Keeps scroll bar at bottom to show most recent text
        scrollRect.verticalNormalizedPosition = scrollRect.verticalNormalizedPosition;

        if(scrollRect.verticalNormalizedPosition == 0)
        {
            isScrolling = false;
        }

    }


}
