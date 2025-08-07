using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerSpawner : NetworkBehaviour
{
    // prefab to be spawned
    [SerializeField] private Transform playerKartPrefab;
    [SerializeField] private Transform npcKartPrefab;
    [SerializeField] private Transform multiplayerNPC;

    [SerializeField] private Transform spawnPointParent;
    [SerializeField] private List<Transform> spawnPoints;
    private List<GameObject> karts = new List<GameObject>();

    public static PlayerSpawner instance;

    public Dictionary<ulong, ItemHolder> kartAndID = new Dictionary<ulong, ItemHolder>();

    private int spawnedKartCount = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void LoadSpawnPoints()
    {
        spawnPoints.Clear();
        foreach (Transform spawnPoint in spawnPointParent.GetComponentInChildren<Transform>(true))
        {
            spawnPoints.Add(spawnPoint);
        }
    }

    private void OnDestroy()
    {
        if (IsServer)
        {
            NetworkManager.SceneManager.OnLoadEventCompleted -= OnAllClientsInGame;

            //despawn/cull spawned karts in case they ever persist between scenes for some reason
            for (int i = 0; i < karts.Count; i++)
            {
                if (karts[i] != null)
                {
                    karts[i].GetComponent<NetworkObject>().Despawn();
                }
            }
        }
        
    }

    private void Start()
    {
        if (spawnPoints.Count < 1)
        {
            LoadSpawnPoints();
        }
        spawnedKartCount = 0;
        if (!IsSpawned)
        {
            StartCoroutine(DelayedLocalSpawn());

        }
        else if (IsServer)
        {
            //StartCoroutine(DelayedServerSpawn());
            NetworkManager.SceneManager.OnLoadEventCompleted += OnAllClientsInGame;
        }

        //if (!IsSpawned)
        //{
        //    Transform kartObject = Instantiate(playerKartPrefab, spawnPoints[0].position, spawnPoints[0] .rotation);
        //    kartObject.SetPositionAndRotation(spawnPoints[0].position, spawnPoints[0].rotation);
        //    spawnedKartCount++;
        //
        //    while (spawnedKartCount < 8)
        //    {
        //        kartObject = Instantiate(npcKartPrefab, spawnPoints[spawnedKartCount].position, spawnPoints[spawnedKartCount].rotation);
        //        NetworkRigidbody NetworkRb = kartObject.GetComponentInChildren<NetworkRigidbody>();
        //        
        //        kartObject.SetPositionAndRotation(spawnPoints[spawnedKartCount].position, spawnPoints[spawnedKartCount].rotation);
        //        spawnedKartCount++;
        //    }
        //}
        //else
        //{
        // FillGrid();
    }

    /// <summary>
    /// kart spawn order will be host, then first to last connected clients, then if there are left over grid slots then fill the rest with npc karts
    /// </summary>
    private void FillGrid()
    {
        // only the server can spawn karts
        if (!IsServer) return;

        CharacterBuilder.StartCharacterBatch();
        // spawn a kart for each player
        if (spawnPoints != null && spawnPoints.Count > 0)
        {
            foreach (KeyValuePair<ulong, NetworkClient> connectedClient in NetworkManager.ConnectedClients)
            {
                Transform kartObject = Instantiate(playerKartPrefab, spawnPoints[spawnedKartCount].position, spawnPoints[spawnedKartCount].rotation);

                kartObject.GetChild(0).transform.position = spawnPoints[spawnedKartCount].position;
                GameObject colliderGO = kartObject.GetChild(1).gameObject;
                colliderGO.GetComponent<SpawnHandler>().spawnPoints = spawnPoints;
                //kartObject.GetChild(1).transform.position = spawnPoints[spawnedKartCount].position;
                //kartObject.rotation = spawnPoints[spawnedKartCount].rotation;
                NetworkObject kartNetworkObject = kartObject.GetComponent<NetworkObject>();
                colliderGO.GetComponent<SpawnHandler>().spawnIndex.Value = spawnedKartCount;
                kartNetworkObject.SpawnAsPlayerObject(connectedClient.Key);
                colliderGO.GetComponent<SpawnHandler>().TeleportToSpawnClientRpc(spawnedKartCount);
                spawnedKartCount++;

                // get a list of itemholder scripts and each karts id
                kartAndID.Add(connectedClient.Key, kartObject.GetComponentInChildren<ItemHolder>());
                karts.Add(kartObject.gameObject);
            }
        }

        while (spawnedKartCount < 8)
        {
            Debug.Log("Spawning NPC");
            Transform kartObject = Instantiate(multiplayerNPC, spawnPoints[spawnedKartCount].position, spawnPoints[spawnedKartCount].rotation);
            kartObject.SetPositionAndRotation(spawnPoints[spawnedKartCount].position, spawnPoints[spawnedKartCount].rotation);
            NetworkObject kartNetworkObject = kartObject.GetComponent<NetworkObject>();
            AppearanceSettings settings = kartObject.GetComponentInChildren<AppearanceSettings>();
            if (settings)
            {
                CharacterBuilder.RandomizeUniqueAppearance(settings);
            }
            kartNetworkObject.Spawn();
            spawnedKartCount++;
            karts.Add(kartObject.gameObject);
        }
    }


    private IEnumerator DelayedLocalSpawn()
    {
        yield return new WaitForSeconds(0.0f); // slight delay
        CharacterBuilder.StartCharacterBatch();
        Transform kartObject = Instantiate(playerKartPrefab, spawnPoints[0].position, spawnPoints[0].rotation);
        AppearanceSettings appearance = kartObject.GetComponentInChildren<AppearanceSettings>();
        if (appearance)
        {
            appearance.UpdateAppearance();
            CharacterBuilder.AddCharacter(appearance);
        }
        kartObject.SetPositionAndRotation(spawnPoints[0].position, spawnPoints[0].rotation);
        spawnedKartCount++;

        while (spawnedKartCount < 8)
        {
            kartObject = Instantiate(npcKartPrefab, spawnPoints[spawnedKartCount].position, spawnPoints[spawnedKartCount].rotation);       
            kartObject.SetPositionAndRotation(spawnPoints[spawnedKartCount].position, spawnPoints[spawnedKartCount].rotation);
            CharacterBuilder.RandomizeUniqueAppearance(kartObject.GetComponentInChildren<AppearanceSettings>());
            spawnedKartCount++;
        }
    }

    //set to fire once all clients are ingame
    void OnAllClientsInGame(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        //print("All clients have loaded into the scene, spawning karts");
        StartCoroutine(DelayedServerSpawn());
    }
    private IEnumerator DelayedServerSpawn()
    {

        FillGrid();
        yield return new WaitForSeconds(0.0f); // slight delay
        Countdown.instance.StartCoroutine(Countdown.instance.ServerCountdownRoutine());
    }
}
