using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Multiplayer Scene Manager by Phillip Brown
/// Manager script that acts as a proxy for the NetworkSceneManager and controls scene loading and transfer if
/// If the NetworkManager is started then use these functions for scene transitions instead of the normal unity sceneloader
/// </summary>
public class MultiplayerSceneManager : NetworkBehaviour
{
    // Manager Initialzation
    public static MultiplayerSceneManager Instance;
    private bool multiplayerActive = false;
    void Awake()
    {
        if (Instance == null)
        {
            multiplayerActive = false;
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    [Rpc(SendTo.ClientsAndHost,RequireOwnership = false)]
    public void SceneAnnouncementRpc(string sceneName){
        Debug.Log($"Server pulling client to {sceneName} scene");
    }

    public override void OnNetworkSpawn()
    {
        multiplayerActive = true;
        NetworkManager.SceneManager.OnLoad += ServerStartedLoadingScene;
        NetworkManager.SceneManager.OnLoadEventCompleted += ClientsHaveLoadedScene;
    }

    public override void OnNetworkDespawn()
    {
        multiplayerActive = false;
    }

    private void ServerStartedLoadingScene(ulong clientId, string sceneName, LoadSceneMode loadSceneMode, AsyncOperation asyncOperation)
    {
        Debug.Log($"client {clientId} has started loading the scene {sceneName}");
    }

    private void ClientsHaveLoadedScene(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        Debug.Log($"{clientsCompleted.Count} clients have completed loading the scene {sceneName} \n"+
                  $"{clientsTimedOut.Count} clients have not loaded the scene {sceneName}");
    }

    // General Scene Load function
    public void LoadScene(string sceneName)
    {
        if (!multiplayerActive)
        {
            Debug.LogWarning("Tried to change scene using NetworkSceneManager while the NetworkManager was shutdown");
        }
        if (!IsServer)
        {
            Debug.LogWarning("Non-Server connection tried to change scene");
            return;
        }
        SceneAnnouncementRpc(sceneName);
        SceneEventProgressStatus status = NetworkManager.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.LogWarning($"Failed to load {sceneName} with a {nameof(SceneEventProgressStatus)}: {status}");
        }
    }
    // Specific menu load functions
    public void ToGameModeSelectScene()
    {
        LoadScene("GameModeSelectScene");
    }
    public void ToPlayerKartScene()
    {
        LoadScene("PlayerKartScene");
    }
    public void ToMapSelectScene()
    {
        LoadScene("MapSelectScene");
    }
    public void ToGameOverScene()
    {
        LoadScene("GameOverScene");
    }
    // Specific track load functions
    public void ToRITOuterLoop()
    {
        LoadScene("LD_RITOuterLoop");
    }
    public void ToGolisano()
    {
        LoadScene("GSP_Golisano");
    }
    public void ToRITDorm()
    {
        LoadScene("LD_RITDorm");
    }
    public void ToRITQuarterMile()
    {
        LoadScene("GSP_RITQuarterMile");
    }
    public void ToFinalsBrickRoad()
    {
        LoadScene("GSP_FinalsBrickRoad");
    }
    public void ToConnectionBuffer()
    {
        LoadScene("MultiplayerConnectionBufferScene");
    }
}
