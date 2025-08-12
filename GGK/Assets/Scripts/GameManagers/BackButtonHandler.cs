using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using Unity.Netcode;

public class BackButtonHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject backOption;
    private GameManager gamemanagerObj;

    // Start is called before the first frame update
    void Start()
    {
        gamemanagerObj = FindAnyObjectByType<GameManager>();

        Button button = backOption.GetComponent<Button>();

        // back button is inactive during multiplayer
        if (MultiplayerManager.Instance.IsMultiplayer)
        {
            button.gameObject.SetActive(false);
        }
        else
        {
            button.gameObject.SetActive(true);
        }
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
                //if (MultiplayerManager.Instance.IsMultiplayer)
                //{
                //    // get the players in the lobby and kick all (except the host)
                //    if (LobbyManager.Instance.IsLobbyHost())
                //    {
                //        Debug.Log("Kicking");
                //        int players = LobbyManager.Instance.GetJoinedLobby().Players.Count;
                //        for (int i = 0; i < players; i++)
                //        {
                //            LobbyManager.Instance.KickPlayer();
                //        }
                //    }
                //    // end multiplayer and leave the lobby
                //    Debug.Log("Leaving");
                //    LobbyManager.Instance.LeaveLobby();
                //    MultiplayerManager.Instance.IsMultiplayer = false;
                //}
                gamemanagerObj.sceneLoader.LoadScene("MultiSinglePlayerScene");
                gamemanagerObj.curState = GameStates.multiSingle;
                break;
            case GameStates.gameMode:
                //if (MultiplayerManager.Instance.IsMultiplayer)
                //{
                //    // get the players in the lobby and kick all (except the host)
                //    if (LobbyManager.Instance.IsLobbyHost())
                //    {
                //        Debug.Log("Kicking");
                //        int players = LobbyManager.Instance.GetJoinedLobby().Players.Count;
                //        for (int i = 0; i < players; i++)
                //        {
                //            LobbyManager.Instance.KickPlayer();
                //        }
                //    }
                //    // end multiplayer and leave the lobby
                //    Debug.Log("Leaving");
                //    LobbyManager.Instance.LeaveLobby();
                //    MultiplayerManager.Instance.IsMultiplayer = false;
                //}
                gamemanagerObj.sceneLoader.LoadScene("MultiSinglePlayerScene");
                gamemanagerObj.curState = GameStates.multiSingle;
                break;
            case GameStates.playerKart:
                gamemanagerObj.sceneLoader.LoadScene("GameModeSelectScene");
                gamemanagerObj.curState = GameStates.gameMode;
                break;
            case GameStates.map:
                gamemanagerObj.sceneLoader.LoadScene("CharSelectMenu");
                gamemanagerObj.curState = GameStates.playerKart;
                break;
            default:
                break;
        }
    }

}
