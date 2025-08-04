using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DisconnectHandler : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Debug.Log("I am the server");
            NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnectHandler;
        }
        else
        {
            Debug.Log("I am a client");
            NetworkManager.Singleton.OnClientDisconnectCallback += ServerDisconnectHandler;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer) {
            NetworkManager.Singleton.OnClientDisconnectCallback -= ClientDisconnectHandler;
        } else {
            NetworkManager.Singleton.OnClientDisconnectCallback -= ServerDisconnectHandler;
        }
    }

    /// <summary>
    /// this is what happens when the server disconnects as seen from the client's perspective
    /// </summary>
    /// <param name="clientId"></param>
    private void ServerDisconnectHandler(ulong clientId)
    {
        // script still considered Spawned when this function runs
        // IsSpawned = true

        Debug.Log($"Server Disconnected \n ClientId in parameter is {clientId}");
    }

    /// <summary>
    /// this is what happens when a client disconnects as seen from the server's perspective
    /// </summary>
    /// <param name="clientId"></param>
    private void ClientDisconnectHandler(ulong clientId)
    {
        Debug.Log($"Client Disconnected \n ClientId in parameter is {clientId}");
    }  
}
