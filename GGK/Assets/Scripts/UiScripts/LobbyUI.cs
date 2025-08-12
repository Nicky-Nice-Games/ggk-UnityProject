using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// LobbyUI Class by Phillip Brown
/// </summary>
public class LobbyUI : MonoBehaviour
{
    public static LobbyUI Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI playersText;
    [SerializeField] private Transform lobbyPlayerSingleTemplate;
    [SerializeField] private Transform container;
    [SerializeField] private Button startButton;
    [SerializeField] private Button leaveButton;

    private void Awake()
    {
        Instance = this;
        startButton.onClick.AddListener(StartGame);
        leaveButton.onClick.AddListener(LeaveLobby);
    }

    private void Start()
    {
        lobbyPlayerSingleTemplate.gameObject.SetActive(false);
        LobbyManager.Instance.OnJoinedLobbyUpdate += LobbyManager_OnJoinedLobbyUpdate;
        LobbyManager.Instance.OnJoinedLobby += LobbyManager_OnJoinedLobby;
        LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
        LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnKickedFromLobby;
        Hide();
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveListener(StartGame);
        leaveButton.onClick.RemoveListener(LeaveLobby);
        LobbyManager.Instance.OnJoinedLobbyUpdate -= LobbyManager_OnJoinedLobbyUpdate;
        LobbyManager.Instance.OnJoinedLobby -= LobbyManager_OnJoinedLobby;
        LobbyManager.Instance.OnLeftLobby -= LobbyManager_OnLeftLobby;
        LobbyManager.Instance.OnKickedFromLobby -= LobbyManager_OnKickedFromLobby;
    }

    private void OnEnable()
    {
        startButton.gameObject.SetActive(LobbyManager.Instance.IsLobbyHost());
    }

    private void LobbyManager_OnKickedFromLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        Hide();
    }

    private void LobbyManager_OnLeftLobby(object sender, EventArgs e)
    {
        Hide();
    }

    private void LobbyManager_OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        Show();
        UpdateLobby(e.lobby);
    }

    private void LobbyManager_OnJoinedLobbyUpdate(object sender, LobbyManager.LobbyEventArgs e)
    {
        UpdatePlayerList(e.lobby.Players);
        startButton.gameObject.SetActive(LobbyManager.Instance.IsLobbyHost());
        if (e.lobby.Players.Count < 2)
        {
            DisableStartButton();
        }
        else
        {
            EnableStartButton();
        }
    }

    private void StartGame()
    {
        LobbyManager.Instance.StartGame();
    }

    private void LeaveLobby()
    {
        LobbyManager.Instance.LeaveLobby();
    }

    private void UpdateLobby(Lobby lobby)
    {
        titleText.text = $"{lobby.Name}";
    }

    private void EnableStartButton()
    {
        startButton.gameObject.GetComponentInChildren<TMP_Text>().color = Color.white;
        startButton.gameObject.GetComponent<Button>().enabled = true;
    }

    private void DisableStartButton()
    {
        Color color;
        UnityEngine.ColorUtility.TryParseHtmlString("#323232", out color);
        startButton.gameObject.GetComponent<Image>().color = color;
        UnityEngine.ColorUtility.TryParseHtmlString("#808080", out color);
        startButton.gameObject.GetComponentInChildren<TMP_Text>().color = color;
        startButton.gameObject.GetComponent<Button>().enabled = false;

    }

    private void UpdatePlayerList(List<Player> playerList)
    {

        playersText.text = $"{playerList.Count}/{LobbyManager.Instance.GetJoinedLobby().MaxPlayers}";
        foreach (Transform child in container)
        {
            if (child == lobbyPlayerSingleTemplate) continue;
            Destroy(child.gameObject);
        }
        foreach (Player player in playerList)
        {
            Transform lobbyPlayerSingleTransform = Instantiate(lobbyPlayerSingleTemplate, container);
            lobbyPlayerSingleTransform.gameObject.SetActive(true);
            LobbyPlayerSingleUI lobbyPlayerSingleUI = lobbyPlayerSingleTransform.GetComponent<LobbyPlayerSingleUI>();
            lobbyPlayerSingleUI.UpdatePlayer(player);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
