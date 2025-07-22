using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class KartCheckpoint : NetworkBehaviour
{
    // Start is called before the first frame update
    public int lap = 0;
    public int checkpointId = 0;
    public float distanceSquaredToNextCP;
    public float finishTime = float.MaxValue;
    public int placement;
    public string name;
    [SerializeField] public List<GameObject> checkpointList;
    [SerializeField] private GameObject checkPointParent;
    GameManager gameManager;
    public GameObject parent;
    public NPCPhysics physicsNPC;

    [SerializeField]
    TextMeshProUGUI placementDisplay;
    [SerializeField]
    TextMeshProUGUI lapDisplay;

    [SerializeField]
    private int totalLaps;

    // check if the player finished the lap/race with the warp
    private bool passedWithWarp = false;

    public bool PassedWithWarp { get { return passedWithWarp; } set { passedWithWarp = value; } }

    void Start()
    {
        totalLaps = 1;
        checkpointId = 0;
        checkPointParent = PlacementManager.instance.checkpointManager;
        Transform childTransform = parent.transform.GetChild(0);
        physicsNPC = childTransform.GetComponent<NPCPhysics>();
        foreach (Transform child in checkPointParent.GetComponentsInChildren<Transform>(true))
        {
            if (child != checkPointParent.transform) // Avoid adding the parent itself
                checkpointList.Add(child.gameObject);
        }
        if (this.GetComponent<NPCDriver>() == null && physicsNPC == null)
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
        if (this.GetComponent<NPCDriver>() == null && physicsNPC == null)
        {
            //placementDisplay.text = "Placement: " + placement;
        }

        if (passedWithWarp && lap == totalLaps)
        {
            finishTime = IsSpawned ? LeaderboardController.instance.networkTime.Value : LeaderboardController.instance.curTime;
            StartCoroutine(FinalizeFinish());
            if (this.GetComponent<NPCDriver>() == null && physicsNPC == null)
            {
                //lapDisplay.text = "Lap: " + (lap + 1);
            }
            passedWithWarp = false;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        bool canPass = false;
        int maxSkip = 15; // how many checkpoints can be skipped ahead
        int nextValidCheckpointIndex = -1;

        // Loop through the next `maxSkip` checkpoints
        for (int i = 1; i <= maxSkip; i++)
        {
            int index = (checkpointId + i) % checkpointList.Count;
            if (other.gameObject == checkpointList[index])
            {
                canPass = true;
                nextValidCheckpointIndex = index;
                break;
            }
        }

        if (other.CompareTag("Checkpoint") && canPass)
        {
            checkpointId = nextValidCheckpointIndex;
        }
        else if (other.CompareTag("Startpoint"))
        {
            if (checkpointId >= checkpointList.Count - 10 && checkpointId < checkpointList.Count)
            {
                lap++;
                checkpointId = 0;
                if (this.GetComponent<NPCDriver>() == null && physicsNPC == null)
                {
                    //lapDisplay.text = "Lap: " + (lap + 1);
                }

                if (lap >= totalLaps)
                {                    
                    finishTime = IsSpawned ? LeaderboardController.instance.networkTime.Value : LeaderboardController.instance.curTime;
                    Debug.Log("this is the if where it should call FinalizeFinish");
                    StartCoroutine(FinalizeFinish());
                }
            }
        }
    }

    IEnumerator GameOverWait()
    {
        yield return new WaitForSeconds(10.5f);
        if (MultiplayerManager.Instance.IsMultiplayer)
        {
            gameManager.GameFinishedRpc();
        }
        else
        {
            gameManager.GameFinished();
        }
    }


    /// <summary>
    /// Finishes the race once ALL the players finishes the race
    /// </summary>
    /// <returns></returns>
    IEnumerator FinalizeFinish()
    {
        Debug.Log("In FinalizeFinish");
        yield return new WaitForEndOfFrame(); // Wait for PlacementManager to finish updating

        LeaderboardController leaderboardController = FindAnyObjectByType<LeaderboardController>();
        leaderboardController.Finished(this);
        
        if (leaderboardController.allPlayerKartsFinished.Value)
        {            
            StartCoroutine(GameOverWait());
        }
    }
}
