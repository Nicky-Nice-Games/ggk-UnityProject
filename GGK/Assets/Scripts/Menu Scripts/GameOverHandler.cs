using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    public GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindAnyObjectByType<GameManager>();
    }

    // Taking the relevant buttons from Game Manager
    public void ReturnToTrackSelect()
    {
        gameManager.PlayerSelected();
    }
    public void ReturnToStart()
    {
        gameManager.LoadStartMenu();
    }
}
