using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiSingleHandler : MonoBehaviour
{
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void SinglePlayer()
    {
        gameManager.SinglePlayer();
    }

    public void MultiPlayer()
    {
        gameManager.MutliPlayer();
    }
}
