using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseHandler : NetworkBehaviour
{
    // References
    public GameManager gameManager;
    public SceneLoader sceneLoader;
    public Countdown countdown;

    public GameObject pausePanel;
    public GameObject restartBtn;
    public GameObject mapBtn;
    public GameObject startBtn;

    public Vector3 initalScale;

    // Multiplayer Pausing
    public bool isHostPaused;
    public GameObject hostPausedText;
    public static PauseHandler instance;

    private void Awake()
    {
        instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        initalScale = restartBtn.transform.localScale;
        gameManager = FindAnyObjectByType<GameManager>();
        sceneLoader = FindAnyObjectByType<SceneLoader>();

        isHostPaused = false;

        // Client cannot use pause panel buttons
        if (!MultiplayerManager.Instance.IsHost && MultiplayerManager.Instance.IsMultiplayer)
        {
            restartBtn.GetComponent<Button>().enabled = false;
            mapBtn.GetComponent<Button>().enabled = false;
            startBtn.GetComponent<Button>().enabled = false;
        }
    }

    // I assume this works together with the button visual script someone added to the pause panel prefab
    public void OnDisable()
    {
        restartBtn.transform.localScale = initalScale;
        startBtn.transform.localScale = initalScale;
        mapBtn.transform.localScale = initalScale;
    }

    // Add this to player input
    public void TogglePause(InputAction.CallbackContext context)
    {
        // Checks if function is called and that countdown has finished
        if (context.started && countdown.finished)
        {
            if (MultiplayerManager.Instance.IsMultiplayer)
            {
                if (MultiplayerManager.Instance.IsHost)
                {
                    if (pausePanel.activeSelf)
                    {
                        pausePanel.SetActive(false);
                        Time.timeScale = 1;
                    }
                    else
                    {
                        pausePanel.SetActive(true);
                        restartBtn.SetActive(false);
                        mapBtn.SetActive(true);
                        startBtn.SetActive(true);
                        Time.timeScale = 0;
                    }

                    ToggleMultiplayerPauseRpc();
                }
                else // Not host
                {
                    if (pausePanel.activeSelf)
                    {
                        pausePanel.SetActive(false);

                        // If host not paused resume game, else wait for host to unpause
                        if (!isHostPaused)
                        {
                            Time.timeScale = 1;
                        }
                    }
                    else
                    {
                        pausePanel.SetActive(true);
                        restartBtn.SetActive(false);
                        mapBtn.SetActive(false);
                        startBtn.SetActive(true);
                    }
                }
            }
            else
            {
                if (pausePanel.activeSelf)
                {
                    pausePanel.SetActive(false);
                    Time.timeScale = 1;
                }
                else
                {
                    pausePanel.SetActive(true);
                    restartBtn.SetActive(true);
                    mapBtn.SetActive(true);
                    startBtn.SetActive(true);
                    Time.timeScale = 0;
                }
            }
        }
    }

    // Restart the track
    public void Restart()
    {
        DisableButtons();

        sceneLoader.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    // Return to start menu
    public void ReturnToStart()
    {
        DisableButtons();
        if (IsSpawned)
        {
            DisconnectHandler.instance.SafeDisconnect();
        }
        else
        {
            gameManager.LoadStartMenu();
        }
        Time.timeScale = 1;
    }

    // Return to map select menu
    public void ReturnToMapSelect()
    {
        DisableButtons();
        if (IsSpawned)
        {
            MultiplayerSceneManager.Instance.ToMapSelectScene();
        }
        else
        {
            gameManager.PlayerSelected();
        }
        Time.timeScale = 1;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ToggleMultiplayerPauseRpc()
    {
        Debug.Log("ToggleMultiplayerPauseRpc called");

        if (isHostPaused)
        {
            // Host unpauses and pause panel closes
            isHostPaused = false;

            // Client paused
            if (pausePanel.activeSelf)
            {
                hostPausedText.SetActive(false);
            }
            // Client not paused
            else
            {
                hostPausedText.SetActive(false);
                Time.timeScale = 1;
            }
        }
        else
        {
            // Host pauses and pause panel opens
            isHostPaused = true;

            // Client paused
            if (pausePanel.activeSelf)
            {
                hostPausedText.SetActive(true);
            }
            // Client not paused
            else
            {
                hostPausedText.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }

    // Disables buttons to avoid spammed clicks
    private void DisableButtons()
    {
        restartBtn.GetComponent<Button>().enabled = false;
        mapBtn.GetComponent<Button>().enabled = false;
        startBtn.GetComponent<Button>().enabled = false;
    }
}