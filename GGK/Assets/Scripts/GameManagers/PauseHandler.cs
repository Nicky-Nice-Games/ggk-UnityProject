using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseHandler : MonoBehaviour
{
    // References
    public GameManager gameManager;
    public SceneLoader sceneLoader;
    public Countdown countdown;
    public GameObject restartBtn;
    public GameObject startButton;
    public GameObject mapBtn;

    public Vector3 initalScale;

    // Start is called before the first frame update
    void Start()
    {
        initalScale = restartBtn.transform.localScale;
        gameManager = FindAnyObjectByType<GameManager>();
        sceneLoader = FindAnyObjectByType<SceneLoader>();
    }

    // I assume this works together with the button visual script someone added to the pause panel prefab
    public void OnDisable()
    {
        restartBtn.transform.localScale = initalScale;
        startButton.transform.localScale = initalScale;
        mapBtn.transform.localScale = initalScale;
    }

    // Add this to player input
    public void TogglePause(InputAction.CallbackContext context)
    {
        // Checks if function is called and that countdown has finished
        if(context.started && countdown.finished)
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                gameObject.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }

    // Restart the track
    public void Restart()
    {
        sceneLoader.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    // Return to start menu
    public void ReturnToStart()
    {
        gameManager.LoadStartMenu();
        Time.timeScale = 1;
    }

    // Return to map select menu
    public void ReturnToMapSelect()
    {
        gameManager.PlayerSelected();
        Time.timeScale = 1;
    }
}
