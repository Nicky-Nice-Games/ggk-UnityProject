using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenuHandler : MonoBehaviour
{
    public GameManager gameManager;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void ReturnToTrackSelect()
    {
        gameManager.PlayerSelected();
    }
    public void ReturnToStart()
    {
        gameManager.LoadStartMenu();
    }
}
