using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeHandler : MonoBehaviour
{
    public GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindAnyObjectByType<GameManager>();
    }

    // Taking the relevant buttons from Game Manager
    public void QuickRace()
    {
        gameManager.QuickRace();
    }
}
