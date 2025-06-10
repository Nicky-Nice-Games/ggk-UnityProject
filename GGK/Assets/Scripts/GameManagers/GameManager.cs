using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening.Core.Easing;

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
    // Enum
    private GameStates curState;

    // Instance
    public static GameObject gameManagerInstance;

    // Options
    private OptionsHandler options;
    private GameObject optionsPanel;

    // Options Data
    public int volume = 100;

    // Character
    public Sprite characterSprite;
    public Color characterColor;
    
    // Non-mouse support
    public GameObject currentSceneFirst;

    void Awake()
    {
        if (gameManagerInstance == null)
        {
            gameManagerInstance = gameObject;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        curState = GameStates.start;

        // Call ConnectOptions on scene load and current scene
        SceneManager.sceneLoaded += ConnectOptions;
        ConnectOptions();

        // Call RefreshSelected on device change, scene load and current scene
        InputSystem.onDeviceChange += RefreshSelected;
        SceneManager.sceneLoaded += RefreshSelected;
        RefreshSelected();
    }

    // Update is called once per frame
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
        SceneManager.LoadScene("MultiSinglePlayerScene");
        curState = GameStates.multiSingle;
    }
    public void SinglePlayer()
    {
        SceneManager.LoadScene("GameModeSelectScene");
        curState = GameStates.gameMode;
    }
    public void MutliPlayer()
    {
        // When there is appropriate mutliplayer support add correct scene
    }
    public void QuickRace()
    {
        SceneManager.LoadScene("PlayerKartScene");
        curState = GameStates.playerKart;
    }
    public void PlayerSelected()
    {
        SceneManager.LoadScene("MapSelectScene");
        curState = GameStates.map;
    }
    public void MapSelected(string mapName)
    {
        SceneManager.LoadScene(mapName.Trim());
        curState = GameStates.game;
    }
    public void GameFinished()
    {
        SceneManager.LoadScene("GameOverScene");
        curState = GameStates.gameOver;
    }
    public void LoadStartMenu()
    {
        SceneManager.LoadScene("StartScene");
        curState = GameStates.start;
    }

    // Options methods
    public void Options()
    {
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
        options = FindAnyObjectByType<OptionsHandler>(FindObjectsInactive.Include);
        optionsPanel = options.gameObject;
        options.gameManager = this;
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
        currentSceneFirst = EventSystem.current.firstSelectedGameObject;

        // Check what kind of interactable it is
        if (currentSceneFirst.GetComponent<Button>() != null)
        {
            currentSceneFirst.GetComponent<Button>().Select();
        }
        else if (currentSceneFirst.GetComponent<Slider>() != null)
        {
            currentSceneFirst.GetComponent<Slider>().Select();
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
