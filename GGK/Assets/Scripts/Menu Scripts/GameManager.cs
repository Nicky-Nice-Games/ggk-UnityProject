using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Instance
    public static GameObject gameManagerInstance;

    // Options
    public OptionsHandler options;
    public GameObject optionsPanel;
    public string previousScene;

    // Character
    public Sprite characterSprite;
    public Color characterColor;

    // Options
    public int volume = 100;

    // Non-mouse support
    public GameObject firstInteractable;
    
    void Awake()
    {
        if (gameManagerInstance == null)
        {
            gameManagerInstance = gameObject;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        OptionsHandler options = gameObject.GetComponent<OptionsHandler>();

        InputSystem.onDeviceChange += RefreshSelected;
        SceneManager.sceneLoaded += RefreshSelected;
        SceneManager.sceneLoaded += ConnectOptions;

        RefreshSelected();
        ConnectOptions();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Options();
        }
    }

    // Scene Transition Methods
    public void StartGame()
    {
        SceneManager.LoadScene("PlayerModeMenu");
    }
    public void SinglePlayer()
    {
        SceneManager.LoadScene("GameModeMenu");
    }
    public void QuickRace()
    {
        SceneManager.LoadScene("PlayerSelectMenu");
    }
    public void PlayerSelected()
    {
        SceneManager.LoadScene("TrackSelectMenu");
    }
    public void LoadTrack(string trackName)
    {
        SceneManager.LoadScene(trackName.Trim());
    }
    public void GameFinished()
    {
        SceneManager.LoadScene("GameOverMenu");
    }
    public void LoadStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    // Options methods
    public void Options()
    {
        /*
        if (SceneManager.GetActiveScene().name != "OptionsMenu")
        {
            previousScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene("OptionsMenu");
        }
        else
        {
            SceneManager.LoadScene(previousScene);
        }
        */

        if (!optionsPanel.activeSelf)
        {
            optionsPanel.SetActive(true);
            options.volumeSlider.Select();
        }
        else
        {
            optionsPanel.SetActive(false);
            RefreshSelected();
        }
    }

    public void ConnectOptions()
    {
        // Grabbing options panel
        GameObject canvas = GameObject.FindAnyObjectByType<Canvas>().gameObject;
        optionsPanel = canvas.transform.GetChild(canvas.transform.childCount-1).gameObject;
        
        // Grabbing options panel refrences
        options.volumeSlider = optionsPanel.GetComponentInChildren<Slider>();
        options.volumeSlider.onValueChanged.AddListener(options.VolumeChange);
        options.volumeValue = optionsPanel.GetComponentsInChildren<TextMeshProUGUI>()[2];

        // Intializing option values
        options.volumeSlider.value = volume;
        options.volumeValue.text = volume.ToString();
    }
    public void ConnectOptions(Scene s, LoadSceneMode l)
    {
        ConnectOptions();
    }

    // Non-mouse support methods
    public void RefreshSelected()
    {
        // Grab first interactable game object
        firstInteractable = EventSystem.current.firstSelectedGameObject;

        // Check what kind of interactable it is
        if (firstInteractable.GetComponent<Button>() != null)
        {
            firstInteractable.GetComponent<Button>().Select();
        }
        else if (firstInteractable.GetComponent<Slider>() != null)
        {
            firstInteractable.GetComponent<Slider>().Select();
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
}
