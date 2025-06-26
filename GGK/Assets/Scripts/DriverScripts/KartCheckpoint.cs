using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class KartCheckpoint : MonoBehaviour
{
    // Start is called before the first frame update
    public int lap = 0;
    public int checkpointId = 0;
    public float distanceSquaredToNextCP;
    public float finishTime = float.MaxValue;
    public int placement;
    public string name;
    [SerializeField] public List<GameObject> checkpointList;
    [SerializeField]
    private GameObject checkPointParent;
    GameManager gameManager;

    [SerializeField]
    TextMeshProUGUI placementDisplay;
    [SerializeField]
    TextMeshProUGUI lapDisplay;

    [SerializeField]
    private int totalLaps;

    // check if the player finished the lap/race with the warp
    private bool passedWithWarp = false;

    public bool PassedWithWarp {  get { return passedWithWarp; } set { passedWithWarp = value; } }

    void Start()
    {
        totalLaps = 1;
        checkpointId = 0;
        foreach (Transform child in checkPointParent.GetComponentsInChildren<Transform>(true))
        {
            if (child != checkPointParent.transform) // Avoid adding the parent itself
                checkpointList.Add(child.gameObject);
        }
        if(this.GetComponent<NPCDriver>() == null)
        {
            lapDisplay.text = "Lap: " + (lap + 1);
        }

        GameObject gameManagerGO = GameObject.FindGameObjectWithTag("GameManager");

        if (gameManagerGO == null)
        {
            Debug.LogError("GameManager not found in the scene. Please ensure it is present.");
            return;
        }
        else
        {
            gameManager = gameManagerGO.GetComponent<GameManager>();


        }
    }

    // Update is called once per frame
    void Update()
    {
        distanceSquaredToNextCP = Mathf.Pow(transform.position.x - checkpointList[(checkpointId + 1) % checkpointList.Count].transform.position.x, 2) +
            Mathf.Pow(transform.position.z - checkpointList[(checkpointId + 1) % checkpointList.Count].transform.position.z, 2);
        if (this.GetComponent<NPCDriver>()== null)
        {
            placementDisplay.text = "Placement: " + placement;
        }

        if(passedWithWarp && lap == totalLaps)
        {
            LeaderboardController leaderboardController = FindAnyObjectByType<LeaderboardController>();
            finishTime = leaderboardController.curTime;
            leaderboardController.Finished(this);
            if (this.GetComponent<NPCDriver>() == null)
            {
                lapDisplay.text = "Lap: " + (lap + 1);
            }
            StartCoroutine(GameOverWait());
            passedWithWarp = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bool canPass = false;

        if (other.gameObject == checkpointList[(checkpointId + 1) % checkpointList.Count])
        {
            canPass = true;
        }

        if (other.CompareTag("Checkpoint") && canPass)
        {
            checkpointId++;
        }
        else if (other.CompareTag("Startpoint"))
        {
            if (checkpointId == checkpointList.Count - 1)
            {
                lap++;
                checkpointId = 0;
                if (this.GetComponent<NPCDriver>() == null)
                {
                    lapDisplay.text = "Lap: " + (lap + 1);
                }
                

                // 3 laps finished assuming we start on lap 0
                if (lap == totalLaps)
                {
                    LeaderboardController leaderboardController = FindAnyObjectByType<LeaderboardController>();
                    finishTime = leaderboardController.curTime;
                    leaderboardController.Finished(this);
                    
                    StartCoroutine(GameOverWait());
                }
            }
        }
    }  
    
    IEnumerator GameOverWait()
    {
        yield return new WaitForSeconds(10.5f);
        gameManager.GameFinished();
    }
}