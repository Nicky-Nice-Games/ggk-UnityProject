using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DisconnectHandler : NetworkBehaviour
{
    public static DisconnectHandler instance;

    private void Awake()
    {
        instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            //Debug.Log("I am the server");
            NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnectHandler;
            NetworkManager.Singleton.OnClientConnectedCallback += ClientConnectHandler;
        }
        else
        {
            //Debug.Log("I am a client");
            NetworkManager.Singleton.OnClientDisconnectCallback += ServerDisconnectHandler;
            NetworkManager.Singleton.OnClientConnectedCallback += ServerConnectHandler;
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
            NetworkManager.Singleton.OnClientConnectedCallback -= ServerConnectHandler;
        }
    }

    private void ServerConnectHandler(ulong clientId)
    {
        Debug.Log($"Server Connected\nClientId in parameter is {clientId}");
    }

    private void ClientConnectHandler(ulong clientId)
    {
        Debug.Log($"Client Connected\nClientId in parameter is {clientId}");
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
        Debug.Log($"Server Disconnected\nClientId in parameter is {clientId}");
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
        Debug.Log($"Client Disconnected\nClientId in parameter is {clientId}");
        if (NetworkManager.ConnectedClients.Count <= 1)
        {
            //NetworkManager.Singleton.Shutdown();
            GameManager.thisManagerInstance.sceneLoader.LoadScene("MultiSinglePlayerScene");
            GameManager.thisManagerInstance.curState = GameStates.multiSingle;
        }
    }
    public void SafeDisconnect()
    {
        if (IsServer) return;
        Debug.Log("before");
        NetworkManager.Singleton.Shutdown();
        Debug.Log("after");
        Time.timeScale = 1;
        GameManager.thisManagerInstance.sceneLoader.LoadScene("MultiSinglePlayerScene");
        GameManager.thisManagerInstance.curState = GameStates.multiSingle;
    }
}
