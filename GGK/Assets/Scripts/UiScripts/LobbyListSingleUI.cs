using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playersText;
    [SerializeField] private Button lobbyButton;
    private Lobby lobby;

    private void Start()
    {
        lobbyButton.onClick.AddListener(LobbyButtonOnClick);
    }

    public void UpdateLobby(Lobby lobby)
    {
        this.lobby = lobby;
        lobbyNameText.text = lobby.Name;
        playersText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    private void LobbyButtonOnClick()
    {
        LobbyManager.Instance.JoinLobbyById(lobby.Id);
    }
}
