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
    public GameObject restartBtn;
    public GameObject startButton;
    public GameObject mapBtn;

    public bool countdownDone = false;

    public Vector3 initalScale;

    // Start is called before the first frame update
    void Start()
    {
        initalScale = restartBtn.transform.localScale;
        gameManager = FindAnyObjectByType<GameManager>();
        sceneLoader = FindAnyObjectByType<SceneLoader>();
    }

    // I don't know why this is here, who added this? I think someone else added the button visual script to the pause panel prefab such that
    // buttons pop up if you hover over them and this activates on disable??? What is this for?
    public void OnDisable()
    {
        restartBtn.transform.localScale = initalScale;
        startButton.transform.localScale = initalScale;
        mapBtn.transform.localScale = initalScale;
    }

    // Add this to player input
    public void TogglePause(InputAction.CallbackContext context)
    {
        if(context.started && countdownDone)
        {
            if (gameObject.activeSelf)
            {
                // Exit pause
                gameObject.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                // Enter pause
                gameObject.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }

    // Restart track
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
