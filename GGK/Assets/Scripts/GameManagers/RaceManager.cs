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

    [Header("User Kart Attachments")]
    [SerializeField] private SpeedAndTimeDisplay speedBar;
    [SerializeField] private SpeedCameraEffect speedCamera;

    [Header("Kart Spawning")]
    private int numberOfPlayers = 4;
    [SerializeField] private List<Transform> gridPositions;
    [SerializeField] private Transform playerKartPrefab;
    [SerializeField] private Transform npcKartPrefab;

    private List<Transform> Karts;

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

    private void FillGrid()
    {
        Debug.Log($"Filling {gridPositions.Count} grid positions");

        int playerCount = numberOfPlayers;
        int npcCount = gridPositions.Count - numberOfPlayers;

        Debug.Log($"spawning {numberOfPlayers} players and {npcCount} npc racers");
        for (int index = 0; index < gridPositions.Count; index++)
        {
            if (playerCount > 0) // instantiating/spawning the player karts first
            {
                Transform kartObject = Instantiate(playerKartPrefab);
                kartObject.SetPositionAndRotation(gridPositions[index].position, gridPositions[index].rotation);
                kartObject.GetComponent<NetworkObject>().Spawn();
                if (index == 0)
                {
                    AttachCameraToKart(kartObject);
                    AttachHudToKart(kartObject);
                }
                playerCount--;
            }
            else // filling the rest of the grid slots with npc karts by instantiating/spawning
            {
                Transform kartObject = Instantiate(npcKartPrefab);
                kartObject.SetLocalPositionAndRotation(gridPositions[index].position, gridPositions[index].rotation);
            }
            Debug.Log($"filled position {index + 1}");
        }
        
    }
    private void Start()
    {
        AnnounceConnectionRpc();
        numberOfPlayers = MultiplayerManager.Instance.players.Count;
        //if (!(NetworkManager.LocalClientId == 0)) return;
        FillGrid();
    }

    [Rpc(SendTo.Server)]
    public void AnnounceConnectionRpc(RpcParams rpcParams = default)
    {
        Debug.Log($"Client {rpcParams.Receive.SenderClientId} has connected to the race scene");
    }
    // Tells the speed camera script which kart to follow
    private void AttachCameraToKart(Transform kart)
    {
        Debug.Log("Setting Camera to Follow Kart");
        speedCamera.target = kart.Find("CameraFollowFront");
        speedCamera.targetRigidbody = kart.GetComponentInChildren<Rigidbody>();
        speedCamera.lookBackTarget = kart.Find("CameraFollowBack");
    }

    // Tells the speed bar script which kart to get information of
    private void AttachHudToKart(Transform kart)
    {
        Debug.Log("attaching kart to speed bar");
        Debug.Log(kart.Find("Kart"));
        speedBar.kart = kart.Find("Kart").GetComponentInChildren<NetcodeNEWDriver>();
    }
    // void Start()
    // {
    //     //--------Server/Host----------
    //     if (NetworkManager.Singleton.IsServer)
    //     {
    //         GameObject kartInstance = Instantiate(kartPrefab);
    //         NetworkObject kartNetworkObject = kartInstance.GetComponent<NetworkObject>();
    //         Debug.Log($"Player count: {MultiplayerManager.Instance.players.Count}");
    //         foreach (KeyValuePair<ulong, PlayerData> player in MultiplayerManager.Instance.players)
    //         {
    //             kartNetworkObject.SpawnWithOwnership(player.Key, true); // player key is the client id of each connected player (includes the host)
    //             kartNetworkObject.transform.SetPositionAndRotation(gridPositions[0].transform.position, gridPositions[0].transform.rotation);
    //             Debug.Log($"Spawnned Kart for clientId: {player.Key} at position {kartNetworkObject.transform.position}");
    //             Karts.Add(kartNetworkObject.gameObject);
    //         }
    //         for (int index = 0; index < Karts.Count; index++)
    //         {
    //             Debug.Log($"Added Kart {index} to the grid");
    //             Karts[index].transform.position = gridPositions[index].position;
    //             Karts[index].transform.rotation = gridPositions[index].rotation;
    //         }
    //     }
    // }

    // public override void OnNetworkSpawn()
    // {
    //     SpawnPlayerRpc();
    //     base.OnNetworkSpawn();
    // }

    // [Rpc(SendTo.Server)]
    // public void SpawnPlayerRpc(RpcParams rpcParams = default)
    // {
    //     ulong senderClientId = rpcParams.Receive.SenderClientId;
    //     Debug.Log($"SpawnPlayerRpc - clientId: {senderClientId}");
    //     SpawnPlayerOnClient(senderClientId);
    // }

    // private void SpawnPlayerOnClient(ulong clientId)
    // {
    //     GameObject player = Instantiate(kartPrefab);
    //     NetworkObject playerNetworkObject = player.GetComponent<NetworkObject>();
    //     playerNetworkObject.SpawnAsPlayerObject(clientId);
    // }
}

