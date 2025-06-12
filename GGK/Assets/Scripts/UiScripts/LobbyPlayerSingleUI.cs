using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerSingleUI : MonoBehaviour
{
    public const string KEY_PLAYER_NAME = "PlayerName";
    [SerializeField] private Image hostIcon;
    [SerializeField] private TextMeshProUGUI playerText;
    private Player player;

    public void UpdatePlayer(Player player)
    {
        this.player = player;
        playerText.text = $"{player.Data["PlayerName"].Value}";
        hostIcon.gameObject.SetActive(LobbyManager.Instance.GetJoinedLobby().HostId == player.Id);
    }
}
