using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHandeler : MonoBehaviour
{
    private GameManager gamemanagerObj;
    private float targetTime = 5.0f;
    private float curTime;

    private float tooBadTime = 8.0f;
    [SerializeField]
    private TextMeshProUGUI tooBad;

    [SerializeField]
    private Image leaderboard;

    // Start is called before the first frame update
    void Start()
    {
        gamemanagerObj = FindAnyObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(leaderboard.gameObject.activeSelf)
        {
            NEWDriver[] karts = GameObject.FindObjectsOfType<NEWDriver>();
            curTime = targetTime -= Time.deltaTime;
            if (targetTime <= 0.0f)
            {
                curTime += Time.deltaTime;
                foreach(NEWDriver driver in karts)
                {
                    GameObject gameobj = driver.transform.parent.gameObject;
                    KartCheckpoint kartCheck = gameobj.GetComponentInChildren<KartCheckpoint>();
                    if(kartCheck.finishTime == 0)
                    {
                        tooBad.gameObject.SetActive(true);
                    }
                }
                if (curTime >= tooBadTime)
                {
                    timerEnded();
                }
            }
        }
    }

    /// <summary>
    /// Tells the manager to load the game over scene
    /// </summary>
    void timerEnded()
    {
        gamemanagerObj.GetComponent<GameManager>().GameFinished();
    }
}
