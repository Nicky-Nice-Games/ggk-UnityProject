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

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        sceneLoader = FindAnyObjectByType<SceneLoader>();
    }

    // Add this to player input
    public void TogglePause(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }
    }

    public void Restart()
    {
        sceneLoader.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToStart()
    {
        gameManager.LoadStartMenu();
    }

    public void ReturnToMapSelect()
    {
        gameManager.PlayerSelected();
    }
}
