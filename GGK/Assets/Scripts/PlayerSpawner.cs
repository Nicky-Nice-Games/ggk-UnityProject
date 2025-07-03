using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    // prefab to be spawned
    [SerializeField] private Transform playerKartPrefab;
    //[SerializeField] private Transform npcKartPrefab;

    [SerializeField] private List<Transform> spawnPoints;
    private int spawnedKartCount = 0;

    private void Start()
    {
        spawnedKartCount = 0;
        FillGrid();
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
                Transform kartObject = Instantiate(playerKartPrefab);
                kartObject.SetPositionAndRotation(spawnPoints[spawnedKartCount].position, spawnPoints[spawnedKartCount].rotation);
                NetworkObject kartNetworkObject = kartObject.GetComponent<NetworkObject>();
                kartNetworkObject.SpawnAsPlayerObject(connectedClient.Key);
                spawnedKartCount++;
            }
        }
        
        // while(spawnedKartCount < 8){
        //     Transform kartObject = Instantiate(npcKartPrefab);
        //     NetworkObject kartNetworkObject = kartObject.GetComponent<NetworkObject>();
        //     kartNetworkObject.Spawn();
        //     spawnedKartCount++;
        // }
    }
}
