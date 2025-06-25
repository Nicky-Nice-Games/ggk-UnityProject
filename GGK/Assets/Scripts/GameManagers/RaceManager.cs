using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// RaceManager
/// Written by Phillip Brown
/// Handles spawning racers and placing racers on the starting grid
/// </summary>
public class RaceManager : NetworkBehaviour
{
    public static RaceManager Instance;

    [SerializeField] private GameObject kartPrefab;

    [SerializeField] private List<Transform> gridPositions;

    private List<GameObject> Karts;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //--------Server/Host----------
        if (IsServer)
        {
            GameObject kartInstance = Instantiate(kartPrefab);
            NetworkObject kartNetworkObject = kartInstance.GetComponent<NetworkObject>();
            foreach (KeyValuePair<ulong, PlayerData> player in MultiplayerManager.Instance.players)
            {
                kartNetworkObject.SpawnWithOwnership(player.Key, true); // player key is the client id of each connected player (includes the host)
                kartNetworkObject.transform.SetPositionAndRotation(gridPositions[0].transform.position, gridPositions[0].transform.rotation);
                Karts.Add(kartNetworkObject.gameObject);
            }
            for (int index = 0; index < Karts.Count; index++)
            {
                Karts[index].transform.position = gridPositions[index].position;
                Karts[index].transform.rotation = gridPositions[index].rotation;
            }
        }
    }

    // public override void OnNetworkSpawn()
    // {
    //     SpawnPlayerRpc();
    //     base.OnNetworkSpawn();
    // }

    [Rpc(SendTo.Server)]
    public void SpawnPlayerRpc(RpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        Debug.Log($"SpawnPlayerRpc - clientId: {senderClientId}");
        SpawnPlayerOnClient(senderClientId);
    }

    private void SpawnPlayerOnClient(ulong clientId)
    {
        GameObject player = Instantiate(kartPrefab);
        NetworkObject playerNetworkObject = player.GetComponent<NetworkObject>();
        playerNetworkObject.SpawnAsPlayerObject(clientId);
    }
}
