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
        totalLaps = 3;
        checkpointId = 0;
        checkPointParent = PlacementManager.instance.checkpointManager;
        Transform childTransform = parent.transform.GetChild(0);
        physicsNPC = childTransform.GetComponent<NPCPhysics>();
        foreach (Transform child in checkPointParent.GetComponentsInChildren<Transform>(true))
        {
            if (child != checkPointParent.transform) // Avoid adding the parent itself
                checkpointList.Add(child.gameObject);
        }
        if(IsSpawned && IsOwner){
            if (this.GetComponent<NPCDriver>() == null && physicsNPC == null) {
                lapDisplay.text = "Lap: " + (lap + 1);
            }   
        }else{
            if (this.GetComponent<NPCDriver>() == null && physicsNPC == null)
            {
                lapDisplay.text = "Lap: " + (lap + 1);
            }   
        }

        name = transform.parent.GetChild(0).gameObject.name;
    }


    // Update is called once per frame
    void Update()
    {
        if(name.Length < 1)
        {
            name = transform.parent.GetChild(0).gameObject.name;
        }


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

        if ((other.CompareTag("Checkpoint") && canPass) || (other.CompareTag("RespawnPoint") && canPass))
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
                    // raceposition data to playerinfo
                    parent.transform.GetChild(0).GetComponent<NEWDriver>().playerInfo.racePosition = placement;
                    Debug.Log("this is the if where it should call FinalizeFinish");
                    StartCoroutine(FinalizeFinish());
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // kart collision data to playerinfo
        if (collision.gameObject.CompareTag("Kart"))
        {
            // avoid doubling collisions by only checking the Kart child
            // only count collisions from a player into any other kart
            if(collision.gameObject.GetComponent<KartCheckpoint>() != null &&
                this.transform.parent.transform.GetChild(0).gameObject.GetComponent<NEWDriver>() != null)
            {
                Debug.Log("Collision with kart");
                GameObject driverObj = this.transform.parent.transform.GetChild(0).gameObject;
                driverObj.GetComponent<NEWDriver>().playerInfo.collisionsWithPlayers++;
            }

        }
    }

    IEnumerator GameOverWait()
    {
        yield return new WaitForSeconds(10.5f);
        if (MultiplayerManager.Instance.IsMultiplayer)
        {
            GameManager.thisManagerInstance.GameFinishedRpc();
        }
        else
        {
            GameManager.thisManagerInstance.GameFinished();
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
        Debug.Log(gameObject.transform.parent.GetChild(0));
        Debug.Log(gameObject.transform.parent.GetChild(0).gameObject);
        if (leaderboardController.allPlayerKartsFinished.Value /*&& 
            gameObject.transform.parent.GetChild(0).GetComponent<NEWDriver>() != null*/)
        {
            Debug.Log("Inside");
            StartCoroutine(GameOverWait());
        }
    }
}
