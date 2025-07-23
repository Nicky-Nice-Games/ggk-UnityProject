using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{
    // prefab to be spawned
    [SerializeField] private Transform playerKartPrefab;
    [SerializeField] private Transform npcKartPrefab;
    [SerializeField] private Transform multiplayerNPC;

    [SerializeField] private Transform spawnPointParent;
    [SerializeField] private List<Transform> spawnPoints;

    private int spawnedKartCount = 0;

    private void LoadSpawnPoints()
    {
        spawnPoints.Clear();
        foreach (Transform spawnPoint in spawnPointParent.GetComponentInChildren<Transform>(true))
        {
            spawnPoints.Add(spawnPoint);
        }
    }

    private void Start()
    {
        LoadSpawnPoints();
        spawnedKartCount = 0;
        if (!IsSpawned)
        {
            StartCoroutine(DelayedLocalSpawn());
        }
        else if (IsServer)
        {
            StartCoroutine(DelayedServerSpawn());
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
        //    FillGrid();
        //}

    }

    /// <summary>
    /// kart spawn order will be host, then first to last connected clients, then if there are left over grid slots then fill the rest with npc karts
    /// </summary>
    private void FillGrid()
    {
        // only the server can spawn karts
        if (!IsServer) return;

        // spawn a kart for each player
        if(spawnPoints != null && spawnPoints.Count > 0)
        {
            foreach (KeyValuePair<ulong, NetworkClient> connectedClient in NetworkManager.ConnectedClients)
            {

                Transform kartObject = Instantiate(
                    playerKartPrefab,
                    spawnPoints[spawnedKartCount].position,
                    spawnPoints[spawnedKartCount].rotation
                );

                // Temporarily disable sync
                ClientNetworkTransform netTransformKart = kartObject.GetChild(0).GetComponent<ClientNetworkTransform>();
                ClientNetworkTransform netTransformCollider = kartObject.GetChild(0).GetComponent<ClientNetworkTransform>();
                if (netTransformKart != null && netTransformCollider != null)
                {
                    netTransformKart.enabled = false;
                    netTransformCollider.enabled = false;
                }

                // Set position
                kartObject.position = spawnPoints[spawnedKartCount].position;
                kartObject.rotation = spawnPoints[spawnedKartCount].rotation;

                // Spawn
                NetworkObject kartNetworkObject = kartObject.GetComponent<NetworkObject>();
                kartNetworkObject.SpawnAsPlayerObject(connectedClient.Key);

                // Re-enable sync next frame
                StartCoroutine(EnableNetworkTransformNextFrame(kartObject));

                spawnedKartCount++;
            }
        }
        
        while(spawnedKartCount < 8){
            Debug.Log("Spawning NPC");
            Transform kartObject = Instantiate(multiplayerNPC, spawnPoints[spawnedKartCount].position, spawnPoints[spawnedKartCount].rotation);
            kartObject.SetPositionAndRotation(spawnPoints[spawnedKartCount].position, spawnPoints[spawnedKartCount].rotation);
            NetworkObject kartNetworkObject = kartObject.GetComponent<NetworkObject>();
            kartNetworkObject.Spawn();
            spawnedKartCount++;
        }
    }


    private IEnumerator DelayedLocalSpawn()
    {
        yield return new WaitForSeconds(0.2f); // slight delay
        Transform kartObject = Instantiate(playerKartPrefab, spawnPoints[0].position, spawnPoints[0].rotation);
        kartObject.SetPositionAndRotation(spawnPoints[0].position, spawnPoints[0].rotation);
        spawnedKartCount++;

        while (spawnedKartCount < 8)
        {
            kartObject = Instantiate(npcKartPrefab, spawnPoints[spawnedKartCount].position, spawnPoints[spawnedKartCount].rotation);
            kartObject.SetPositionAndRotation(spawnPoints[spawnedKartCount].position, spawnPoints[spawnedKartCount].rotation);
            spawnedKartCount++;
        }
    }

    private IEnumerator DelayedServerSpawn()
    {
        yield return new WaitForSeconds(0.2f); // slight delay
        FillGrid();
    }

    private IEnumerator EnableNetworkTransformNextFrame(Transform kartObject)
    {
        yield return null; // Wait one frame

        ClientNetworkTransform netTransformKart = kartObject.GetChild(0).GetComponent<ClientNetworkTransform>();
        ClientNetworkTransform netTransformCollider = kartObject.GetChild(0).GetComponent<ClientNetworkTransform>();
        if (netTransformKart != null && netTransformCollider != null)
        {
            netTransformKart.enabled = true;
            netTransformCollider.enabled = true;
        }
    }
}
