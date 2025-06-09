using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHandeler : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI timer;
    private GameManager gamemanagerObj;
    private float targetTime = 5.0f;
    private float curTime;

    // Start is called before the first frame update
    void Start()
    {
        gamemanagerObj = FindAnyObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        curTime = targetTime -= Time.deltaTime;
        if (targetTime <= 0.0f)
        {
            timerEnded();
        }
        timer.text = curTime.ToString();
    }

    /// <summary>
    /// Tells the manager to load the game over scene
    /// </summary>
    void timerEnded()
    {
        gamemanagerObj.GetComponent<GameManager>().GameFinished();
    }
}
