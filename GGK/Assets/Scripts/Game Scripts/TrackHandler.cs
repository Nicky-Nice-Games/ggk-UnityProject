using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

public class TrackHandler : MonoBehaviour
{
    // References
    public GameManager gameManager;
    public GameObject leaderboard;
    public GameObject leaderboardItem;

    // Kart Spawning
    public List<GameObject> spawnpoints;
    public List<CheckPoint> checkpoints;

    public LapCounter player;
    public List<LapCounter> karts;

    public GameObject playerKart;
    public GameObject NPCKart;

    // Timer
    public float currentTime;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        SpawnKarts();
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (leaderboard.activeSelf)
            {
                leaderboard.SetActive(false);
            }
            else
            {
                leaderboard.SetActive(true);
            }
        }
    }

    public void SpawnKarts()
    {
        int count = spawnpoints.Count;
        karts = new List<LapCounter>();

        // Spawn NPC Karts
        for (int i = 0; i < count - 1; i++)
        {
            karts.Add(Instantiate(NPCKart).GetComponent<LapCounter>());
            karts[i].placement = i;
        }

        // Spawn player
        player = Instantiate(playerKart).GetComponent<LapCounter>();
        player.placement = count;
        ConnectPlayer();
        karts.Add(player);

        // Place karts at spawnpoints & pass down checkpoints
        for (int i = 0; i < count; i++)
        {
            karts[i].checkpoints = checkpoints;
            karts[i].gameObject.transform.position = spawnpoints[i].transform.position;
            karts[i].gameObject.transform.rotation = spawnpoints[i].transform.rotation;
        }
    }

    public void ConnectPlayer()
    {
        // HUD
        player.lapCountText = GameObject.Find("LapText").GetComponent<TextMeshProUGUI>();
        player.checkpointText = GameObject.Find("CheckText").GetComponent<TextMeshProUGUI>();
        player.placementText = GameObject.Find("PlacementText").GetComponent<TextMeshProUGUI>();
        player.lapCountText.text = "LAP 1/3";
        player.checkpointText.text = "Check 1/" + checkpoints.Count;

        // Camera
        SpeedCameraEffect cam = FindAnyObjectByType<SpeedCameraEffect>();
        cam.target = player.gameObject.transform.GetChild(2);
        cam.targetRigidbody = player.gameObject.GetComponent<Rigidbody>();
        cam.lookBackTarget = player.gameObject.transform.GetChild(3);

        // Input
        PlayerInput input = FindAnyObjectByType<PlayerInput>();
        Driver driver = player.gameObject.GetComponent<Driver>();
        input.actions["Move"].started += driver.OnMove;
        input.actions["Move"].performed += driver.OnMove;
        input.actions["Move"].canceled += driver.OnMove;
        input.actions["Accelerate"].started += driver.OnAcceleration;
        input.actions["Accelerate"].performed += driver.OnAcceleration;
        input.actions["Accelerate"].canceled += driver.OnAcceleration;
        input.actions["Turn"].started += driver.OnTurn;
        input.actions["Turn"].performed += driver.OnTurn;
        input.actions["Drift Hop"].started += driver.OnDrift;
        input.actions["Drift Hop"].canceled += driver.OnDrift;
    }

    public void Finished(LapCounter kart)
    {
        // Pull up leaderboards if player finished
        if (kart == player)
        {
            leaderboard.gameObject.SetActive(true);
        }

        // Otherwise add time to leaderboad like normal
        GameObject tempItem = Instantiate(leaderboardItem);
        TextMeshProUGUI[] tempArray = leaderboardItem.GetComponentsInChildren<TextMeshProUGUI>();
        tempArray[0].text = kart.placement.ToString();
        tempArray[1].text = kart.name;
        tempArray[2].text = kart.timeFinished.ToString();

        tempItem.transform.SetParent(leaderboard.transform);
    }
}
