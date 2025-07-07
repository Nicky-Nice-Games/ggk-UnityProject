using Assets.Scripts;
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

    private string textLog = "Welcome to Command Prompt";
    [SerializeField] private TextMeshProUGUI textBox;


    // Start is called before the first frame update
    void Start()
    {
        AddListener(gameObject);
        //gameManager = SceneLoader.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
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

        //check if all parts are correct values

        ////For the game object that the method is called on
        //GameObject go = listeners.Where(obj => obj.name == parts[0]).SingleOrDefault();
        //if (go != null)
        //{
        //    go.SendMessage(parts[1], parts[2]);
        //}


        //if (parts[2] != null)
        //{
        //    //case parts[2] check each enum mode and then load that scene
        //    //gameManager.SafeLoad()
        //}

        switch (parts[0])
        {
            case "LoadMap":
                switch (parts[1])
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
                    default:
                        textLog += "\nError: Param 1 Command Not Found.";
                        break;
                }
                break;
            default:
                textLog += "\nError: Method Command Not Found.";
                break;

        }

    }



    //public void LoadMap(/* MapName mapName? */)
    //{
        
    //}



}
