using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


enum MapName 
{ 
    OuterLoop,
    Golisano,
    Dorm,
    FinalsBrickRoad,
    QuarterMile //Remove?
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
    //[SerializeField] public static GameObject gameManagerObj;
    //public static GameManager gameManager;

    private List<GameObject> listeners = new List<GameObject>();

    private string textLog = "Welcome to Command Prompt\nType ShowMethods for options";
    [SerializeField] private Canvas commandPromptCanvas;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI textBox;


    // Start is called before the first frame update
    void Start()
    {
        AddListener(gameObject);
        commandPromptCanvas.enabled = false;
        //gameManager = SceneLoader.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
        //Shows and hides the canvas (prompt) when ~ is pressed (KeyCode.Tilda not working) !!
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            //Debug.Log("Pressed");
            if (commandPromptCanvas.enabled == false)
            {
                //Debug.Log("false");
                commandPromptCanvas.enabled = true;
            }
            else
            {
                //Debug.Log("true");
                commandPromptCanvas.enabled = false;
            }
            
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            inputField.text = "";

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
        textLog += "\n" + input;

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
                //textLog +=  "\nOptions: ";
                //foreach (MapName mapName in Enum.GetValues(typeof(MapName)))
                //{
                //    textLog += "\n" + mapName;
                //}
                LoadMap(parts[1]);
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



    public void LoadMap(string mapName)
    {
        switch (mapName)
        {
            case "OuterLoop":
                sceneLoader.LoadScene("GSP_RITOuterLoop");
                break;
            case "Golisano":
                sceneLoader.LoadScene("GSP_Golisano");
                break;
            case "Dorm":
                sceneLoader.LoadScene("GSP_RITDorm");
                break;
            case "FinalsBrickRoad":
                sceneLoader.LoadScene("GSP_FinalsBrickRoad");
                break;
            case "QuarterMile":
                sceneLoader.LoadScene("GSP_RITQuarterMile");
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



}
