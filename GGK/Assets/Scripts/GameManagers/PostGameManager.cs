using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System.Linq;

// options for the player post-game
public enum PlayerDecisions
{
    Undecided,
    Staying,
    Leaving
}

public class PostGameManager : NetworkBehaviour
{
    private Dictionary<ulong, PlayerDecisions> playerDecisions = new();

    // Check if all players have selected to stay or leave
    private bool allSelected = false;

    public bool AllSelected {  get { return allSelected; } }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += ConnectClient;
        NetworkManager.Singleton.OnClientDisconnectCallback += DisconnectClient;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= ConnectClient;
        NetworkManager.Singleton.OnClientDisconnectCallback -= DisconnectClient;
    }

    private void Update()
    {
        // reset decisions when the game over scene isn't active
        if (SceneManager.GetActiveScene().name != "GameOverScene")
        {
            ResetDecisions();
        }

        // if the connectedclients count doesn't equal the playerdecisions count, update it
        //if(NetworkManager.Singleton.ConnectedClientsIds.ToList().Count != playerDecisions.Count)
        //{
        //    MakeClientsList();
        //}
    }

    // Stores each players decision and gives the host the ability to proceed when every player makes a decision
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void EnterDecisionRpc(PlayerDecisions decision, RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        playerDecisions[clientId] = decision;

        Debug.Log($"Client {clientId} chose: {decision}");

        allSelected = AllPlayersDecided();
        ProcessDecisions();
    }

    // Checks if all players have chosen between Staying or Leaving
    private bool AllPlayersDecided()
    {
        foreach (var decision in playerDecisions.Values)
        {
            if(decision == PlayerDecisions.Undecided)
            {
                return false;
            }
        }
        return true;
    }

    // If a player decides to leave, disconnect them
    private void ProcessDecisions()
    {
        foreach(KeyValuePair <ulong, PlayerDecisions> pair in playerDecisions)
        {
            ulong clientId = pair.Key;
            // if the player is leaving, disconnect them
            if(pair.Value == PlayerDecisions.Leaving)
            {
                NetworkManager.Singleton.DisconnectClient(clientId);
            }
        }
    }

    // resets all decisions to Undecided so they don't carry over into a second or following race
    private void ResetDecisions()
    {
        foreach (KeyValuePair <ulong, PlayerDecisions> pair in playerDecisions.ToList())
        {
            playerDecisions[pair.Key] = PlayerDecisions.Undecided;
        }
        allSelected = false;
    }

    private void ConnectClient(ulong clientId)
    {
        playerDecisions.Add(clientId, PlayerDecisions.Undecided);
    }

    private void DisconnectClient(ulong clientId)
    {
        playerDecisions.Remove(clientId);
    }

    public Dictionary<ulong, PlayerDecisions> GetClientsList()
    {
        return playerDecisions;
    }
}
