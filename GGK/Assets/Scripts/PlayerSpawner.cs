using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    // prefab to be spawned
    [SerializeField] private Transform spawnedKartObjectPrefab;

    private void Update()
    {
        SpawnKart();
    }

    private void SpawnKart(){
        // server/host only authority
        if(!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            // instantiating the object on the server/host 
            Transform spawnedKartObjectTransform = Instantiate(spawnedKartObjectPrefab);
            // spawning the object on the clients (syncs the object to the clients)
            spawnedKartObjectTransform.GetComponent<NetworkObject>().Spawn();
            Debug.Log("Spawned In Kart");
        }
    }
}
