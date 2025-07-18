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

    private bool allSelected = false;

    public bool AllSelected {  get { return allSelected; } }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // set each player to undecided
            foreach(var clientId in NetworkManager.Singleton.ConnectedClientsIds.ToList())
            {
                playerDecisions.Add(clientId, PlayerDecisions.Undecided);
            }
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "GameOverScene")
        {
            ResetDecisions();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void EnterDecisionServerRpc(PlayerDecisions decision, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        playerDecisions[clientId] = decision;

        Debug.Log($"Client {clientId} chose: {decision}");

        allSelected = AllPlayersDecided();
        if (AllPlayersDecided())
        {
            ProcessDecisions();
        }
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

    private void ProcessDecisions()
    {
        foreach(var pair in playerDecisions)
        {
            ulong clientId = pair.Key;
            // if the player is leaving, disconnect them
            if(pair.Value == PlayerDecisions.Leaving)
            {
                NetworkManager.Singleton.DisconnectClient(clientId);
            }
        }
    }

    private void ResetDecisions()
    {
        foreach (var pair in playerDecisions.ToList())
        {
            playerDecisions[pair.Key] = PlayerDecisions.Undecided;
        }
        allSelected = false;
    }
}
