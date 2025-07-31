using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsData : MonoBehaviour
{
    // Reference
    private OptionsHandler options;
    private GameObject optionsPanel;
    private GameManager gameManager;
    public GameManager GameManager { get { return gameManager; } }

    // Volume Data
    public float masterVolume = 100;
    public float dialougeVolume = 100;
    public float sfxVolume = 100;
    public float musicVolume = 100;

    // Resolution Data
    public bool IsFullScreen = true;
    public Vector2 resolution;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameManager>();

        SceneManager.sceneLoaded += ConnectOptions;
        ConnectOptions();

        // Force full screen max resolution because certain settings are saved even if you closed the application)
        // Resolution cannot go beyond your display resolution so I took the numbers of the largest computer resolution,
        // but setting them to any sufficiently large numbers is fine
        resolution.x = 3840;
        resolution.y = 2160;
        Screen.SetResolution((int)resolution.x, (int)resolution.y, IsFullScreen);
    }

    // Options methods
    public void Options()
    {
        if (!optionsPanel.activeSelf)
        {
            optionsPanel.SetActive(true);
            options.masterVolumeSlider.Select();
        }
    }
    public void ConnectOptions()
    {
        Button optionsButton;

        // Grabbing options panel
        options = FindAnyObjectByType<OptionsHandler>(FindObjectsInactive.Include);
        if (options != null)
        {
            options.optionsData = this;
            optionsPanel = options.gameObject;

            optionsButton = GameObject.Find("Options").GetComponent<Button>();
            optionsButton.onClick.AddListener(() => gameManager.GetComponent<ButtonBehavior>().OnClick());
            optionsButton.onClick.AddListener(() => Options());
        }               
    }
    public void ConnectOptions(Scene s, LoadSceneMode l)
    {
        ConnectOptions();
    }
}
