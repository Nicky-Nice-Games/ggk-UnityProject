using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DisconnectHandler : NetworkBehaviour
{
    public static DisconnectHandler instance;

    private void Awake() {
        instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            //Debug.Log("I am the server");
            NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnectHandler;
        }
        else
        {
            //Debug.Log("I am a client");
            NetworkManager.Singleton.OnClientDisconnectCallback += ServerDisconnectHandler;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= ClientDisconnectHandler;
        }
        else
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= ServerDisconnectHandler;
        }
    }

    /// <summary>
    /// this is what happens when the server disconnects as seen from the client's perspective
    /// </summary>
    /// <param name="clientId">your own client id</param>
    private void ServerDisconnectHandler(ulong clientId)
    {
        // script still considered Spawned when this function runs
        // IsSpawned = true
        //this sends the clients back to the multi single select screen
        Time.timeScale = 1;
        GameManager.thisManagerInstance.sceneLoader.LoadScene("MultiSinglePlayerScene");
        GameManager.thisManagerInstance.curState = GameStates.multiSingle;
    }

    /// <summary>
    /// this is what happens when a client disconnects as seen from the server's perspective
    /// </summary>
    /// <param name="clientId">the client id of the player who disconnects</param>
    private void ClientDisconnectHandler(ulong clientId)
    {
        Debug.Log($"Client Disconnected \n ClientId in parameter is {clientId}");
        if (NetworkManager.ConnectedClients.Count <= 1)
        {
            NetworkManager.Singleton.Shutdown();
            GameManager.thisManagerInstance.sceneLoader.LoadScene("MultiSinglePlayerScene");
            GameManager.thisManagerInstance.curState = GameStates.multiSingle;
        }
    }

    public void SafeDisconnect()
    {

        if (IsServer)
        {
            for (int index = NetworkManager.Singleton.ConnectedClientsIds.Count - 1; index >= 0; index--)
            {
                NetworkManager.DisconnectClient(NetworkManager.Singleton.ConnectedClientsIds[index]);
            }
        }
        else
        {
            Debug.Log("before");
            NetworkManager.Singleton.Shutdown();
            Debug.Log("after");

            Time.timeScale = 1;
            GameManager.thisManagerInstance.sceneLoader.LoadScene("MultiSinglePlayerScene");
            GameManager.thisManagerInstance.curState = GameStates.multiSingle;
        }
    }

    [Rpc(SendTo.Server)]
    private void DisconnectClientRpc(RpcParams rpcParams = default)
    {
        Debug.Log($"attempting Disconnecting client");

        ulong senderClientId = rpcParams.Receive.SenderClientId;
        Debug.Log($"Disconnecting client {senderClientId}");

        NetworkManager.DisconnectClient(senderClientId);
    }
}
