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

    [SerializeField]
    private int maxCheckpointReachedThisLap = -1;
    private bool hasStartedLapProperly = false;
    private bool hasCrossedStart = false;
    private HashSet<int> checkpointsPassedThisLap = new HashSet<int>();


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
        int checkpointIndex = checkpointList.IndexOf(other.gameObject);
        bool isCheckpoint = other.CompareTag("Checkpoint");
        bool isStartpoint = other.CompareTag("Startpoint");

        if (isCheckpoint && checkpointIndex != -1)
        {
            // If trying to pass a checkpoint we've already passed this lap, ignore it
            if (checkpointsPassedThisLap.Contains(checkpointIndex))
            {
                Debug.Log($"{name}: Already passed checkpoint {checkpointIndex}, ignoring.");
                return;
            }

            // Block backward-cheese: prevent hitting the *last checkpoint* before crossing start
            if (!hasCrossedStart && checkpointIndex >= checkpointList.Count - 3)
            {
                Debug.Log($"{name}: Ignored checkpoint {checkpointIndex} — can't go backward before lap start.");
                return;
            }

            // Valid forward checkpoint hit
            checkpointsPassedThisLap.Add(checkpointIndex);
            maxCheckpointReachedThisLap = Mathf.Max(maxCheckpointReachedThisLap, checkpointIndex);
            checkpointId = checkpointIndex;
        }
        else if (isStartpoint)
        {
            if (!hasCrossedStart)
            {
                hasCrossedStart = true;
                Debug.Log($"{name}: Lap started");
            }
            else
            {
                // Only count lap if you've reached the last checkpoint in this lap
                if (maxCheckpointReachedThisLap >= checkpointList.Count - 1)
                {
                    lap++;
                    checkpointId = 0;
                    maxCheckpointReachedThisLap = -1;
                    hasCrossedStart = false;
                    checkpointsPassedThisLap.Clear();

                    if (this.GetComponent<NPCDriver>() == null)
                    {
                        lapDisplay.text = "Lap: " + (lap + 1);
                    }

                    if (lap == totalLaps)
                    {
                        LeaderboardController leaderboardController = FindAnyObjectByType<LeaderboardController>();
                        finishTime = leaderboardController.curTime;
                        leaderboardController.Finished(this);
                        StartCoroutine(GameOverWait());
                    }
                }
                else
                {
                    Debug.Log($"{name}: Crossed Start but didn’t finish lap properly. MaxCP={maxCheckpointReachedThisLap}");
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