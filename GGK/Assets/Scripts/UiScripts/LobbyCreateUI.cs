using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

/// <summary>
/// LobbyCreateUI Class by Phillip Brown
/// </summary>
public class LobbyCreateUI : MonoBehaviour
{
    public static LobbyCreateUI Instance { get; private set; }
    [SerializeField] private Button createButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button playersButton;
    [SerializeField] private TextMeshProUGUI playersText;
    [SerializeField] private TMP_InputField nameInput;
    private int maxPlayers = 8;
    private int players = 2;


    private void Awake()
    {
        Instance = this;
        playersText.text = $"Max Player: {players}";

        createButton.onClick.AddListener(CreateLobbyButtonClick);
        backButton.onClick.AddListener(BackButtonClick);
        playersButton.onClick.AddListener(PlayersButtonClick);
    }

    private void Start()
    {
        LobbyManager.Instance.OnJoinedLobby += LobbyManager_OnJoinedLobby;
        Hide();
    }

    private void OnEnable()
    {
        nameInput.text = "";
    }

    private void LobbyManager_OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        Hide();
    }

    private void PlayersButtonClick()
    {
        players += 1;
        if (players > maxPlayers) players = 2;
        if (players < 2) players = maxPlayers;
        playersText.text = $"Max Player: {players}";
    }

    private void CreateLobbyButtonClick()
    {
        LobbyManager.Instance.CreateLobby(nameInput.text.Trim(), players);
    }

    private void BackButtonClick()
    {
        Hide();
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
