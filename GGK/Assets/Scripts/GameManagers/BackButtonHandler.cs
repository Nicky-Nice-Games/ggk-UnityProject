using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

public class BackButtonHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject backOption;
    private GameManager gamemanagerObj;

    // Start is called before the first frame update
    void Start()
    {
        gamemanagerObj = FindAnyObjectByType<GameManager>();

        // assign back button
        Button button = backOption.GetComponent<Button>();
        button.onClick.AddListener(() =>
            GetComponent<BackButtonHandler>().GoBack());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoBack()
    {
        switch (gamemanagerObj.curState)
        {
            case GameStates.login:
                gamemanagerObj.sceneLoader.LoadScene("StartScene");
                gamemanagerObj.curState = GameStates.start;
                break;
            case GameStates.multiSingle:
                gamemanagerObj.sceneLoader.LoadScene("Login");
                gamemanagerObj.curState = GameStates.login;
                break;
            case GameStates.lobby:
                if (MultiplayerManager.Instance.IsMultiplayer)
                {
                    // get the players in the lobby and kick all (except the host)
                    if (LobbyManager.Instance.IsLobbyHost())
                    {
                        Debug.Log("Kicking");
                        int players = LobbyManager.Instance.GetJoinedLobby().Players.Count;
                        for (int i = 0; i < players; i++)
                        {
                            LobbyManager.Instance.KickPlayer();
                        }
                    }
                    // end multiplayer and leave the lobby
                    Debug.Log("Leaving");
                    LobbyManager.Instance.LeaveLobby();
                    MultiplayerManager.Instance.IsMultiplayer = false;
                }
                gamemanagerObj.sceneLoader.LoadScene("MultiSinglePlayerScene");
                gamemanagerObj.curState = GameStates.multiSingle;
                break;
            case GameStates.gameMode:
                if (MultiplayerManager.Instance.IsMultiplayer)
                {
                    // get the players in the lobby and kick all (except the host)
                    if (LobbyManager.Instance.IsLobbyHost())
                    {
                        Debug.Log("Kicking");
                        int players = LobbyManager.Instance.GetJoinedLobby().Players.Count;
                        for (int i = 1; i < players; i++)
                        {
                            LobbyManager.Instance.KickPlayer();
                        }
                    }
                    // end multiplayer and leave the lobby
                    Debug.Log("Leaving");
                    LobbyManager.Instance.LeaveLobby();
                    MultiplayerManager.Instance.IsMultiplayer = false;
                }
                gamemanagerObj.sceneLoader.LoadScene("MultiSinglePlayerScene");
                gamemanagerObj.curState = GameStates.multiSingle;
                break;
            case GameStates.playerKart:
                gamemanagerObj.sceneLoader.LoadScene("GameModeSelectScene");
                gamemanagerObj.curState = GameStates.gameMode;
                break;
            case GameStates.map:
                gamemanagerObj.sceneLoader.LoadScene("PlayerKartScene");
                gamemanagerObj.curState = GameStates.playerKart;
                break;
            default:
                break;
        }
    }

}
