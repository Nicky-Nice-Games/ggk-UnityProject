using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// LobbyListUI Class by Phillip Brown
/// </summary>
public class LobbyListUI : MonoBehaviour
{
    public static LobbyListUI Instance { get; private set; }
    [SerializeField] private Transform lobbySingleTemplate;
    [SerializeField] private Transform container;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button createLobbyButton;

    private void Awake()
    {
        Instance = this;
        refreshButton.onClick.AddListener(RefreshButtonClick);
        createLobbyButton.onClick.AddListener(CreateLobbyButtonClick);
        if (container == null) {
            Debug.Log("had to grab container");
            container = transform.GetChild(0);
        }
        Show();
    }

    private void Start()
    {
        lobbySingleTemplate.gameObject.SetActive(false);
        LobbyManager.Instance.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
        LobbyManager.Instance.OnJoinedLobby += LobbyManager_OnJoinedLobby;
        LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
        LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnKickedFromLobby;
    }

    private void OnDestroy()
    {
        refreshButton.onClick.RemoveListener(RefreshButtonClick);
        createLobbyButton.onClick.RemoveListener(CreateLobbyButtonClick);
        LobbyManager.Instance.OnLobbyListChanged -= LobbyManager_OnLobbyListChanged;
        LobbyManager.Instance.OnJoinedLobby -= LobbyManager_OnJoinedLobby;
        LobbyManager.Instance.OnLeftLobby -= LobbyManager_OnLeftLobby;
        LobbyManager.Instance.OnKickedFromLobby -= LobbyManager_OnKickedFromLobby;
    }

    private void LobbyManager_OnKickedFromLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        Show();
    }

    private void LobbyManager_OnLeftLobby(object sender, EventArgs e)
    {
        Show();
    }

    private void LobbyManager_OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        Hide();
    }

    private void LobbyManager_OnLobbyListChanged(object sender, LobbyManager.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        Debug.Log($"container exists: {container != null}");
        Debug.Log($"lobbylist exists: {lobbyList != null}");
        if (lobbyList != null) Debug.Log($"there are {lobbyList.Count} found lobbies");
        foreach (Transform child in container)
        {
            if (child == null) Debug.Log("child is null");
            if (child == lobbySingleTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbySingleTransform = Instantiate(lobbySingleTemplate, container);
            lobbySingleTransform.gameObject.SetActive(true);
            LobbyListSingleUI lobbyListSingleUI = lobbySingleTransform.GetComponent<LobbyListSingleUI>();
            if (lobbyList.IndexOf(lobby) % 2 == 0)
            {
                lobbySingleTransform.gameObject.GetComponentInChildren<Image>().color = new Color(0, 0, 0, 0.25f);
            }
            else
            {
                lobbySingleTransform.gameObject.GetComponentInChildren<Image>().color = new Color(0, 0, 0, 0.4f);
            }
                lobbyListSingleUI.UpdateLobby(lobby);
        }
    }

    private void RefreshButtonClick()
    {
        LobbyManager.Instance.RefreshLobbyList();
    }

    private void CreateLobbyButtonClick()
    {
        LobbyCreateUI.Instance.Show();
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
