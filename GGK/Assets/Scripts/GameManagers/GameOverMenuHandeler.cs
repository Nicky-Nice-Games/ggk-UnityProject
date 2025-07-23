using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenuHandeler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> buttonOptions = new List<GameObject>();

    private GameManager gamemanagerObj;
    private PostGameManager postgamemanager;

    // panels with options tailored for multiplayer postgame
    [SerializeField]
    private GameObject multiplayerPanel;
    [SerializeField]
    private GameObject playAgainPanel;
    [SerializeField]
    private TextMeshProUGUI waiting;
    [SerializeField]
    private TextMeshProUGUI playerLeft;

    int clientCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        gamemanagerObj = FindAnyObjectByType<GameManager>();
        postgamemanager = gamemanagerObj.postGameManager;

        // deactivate playagain panel in case this is the 2nd+ multiplayer game
        playAgainPanel.SetActive(false);

        // Assigning the buttons their listeners
        foreach (GameObject obj in buttonOptions)
        {
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            gamemanagerObj.GetComponent<ButtonBehavior>().OnClick());
        }

        if (MultiplayerManager.Instance.IsMultiplayer)
        {
            multiplayerPanel.SetActive(true);
        }
        else
        {
            multiplayerPanel.SetActive(false);
        }

        if (MultiplayerManager.Instance.NetworkManager.IsHost)
        {
            waiting.text = "Waiting for players. . .";
        }
        else
        {
            waiting.text = "Waiting for host. . .";
        }
    }

    // Update is called once per frame
    void Update()
    {
        // all players selected to stay or leave and this is the host
        if (postgamemanager.AllSelected &&
            MultiplayerManager.Instance.NetworkManager.IsHost)
        {
            // if everyone else left EXCEPT the host then they leave too
            if (OnlyHostConnected())
            {
                StartCoroutine(HostExit());
            }
            else
            {
                playAgainPanel.SetActive(true);
            }
        }

        if (MultiplayerManager.Instance.IsMultiplayer &&
            NetworkManager.Singleton.ConnectedClientsIds.Count != clientCount)
        {
            if (OnlyHostConnected())
            {
                StartCoroutine(HostExit());
            }
            else
            {
                ShowLeaver();
            }
        }
        clientCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
    }

    public void ReplayButton()
    {
        gamemanagerObj.PlayerSelected();
    }

    public void ReturnToMMButton()
    {
        gamemanagerObj.LoadStartMenu();
    }

    // Multiplayer Panel Button Functions
    public void ReturnToMapMulti()
    {
        if (MultiplayerManager.Instance.NetworkManager.IsHost)
        {
            gamemanagerObj.ToMapSelectScreenRpc();
        }
    }

    public void ReturnToCharMulti()
    {
        if (MultiplayerManager.Instance.NetworkManager.IsHost)
        {
            gamemanagerObj.LoadedGameMode();
        }
    }

    public void ReturnToModesMulti()
    {
        if (MultiplayerManager.Instance.NetworkManager.IsHost)
        {
            gamemanagerObj.ToGameModeSelectSceneRpc();
        }
    }

    public void LeaveLobby()
    {
        postgamemanager.EnterDecisionRpc(PlayerDecisions.Leaving);
        MultiplayerManager.Instance.IsMultiplayer = false;
        // to multisingle select
        gamemanagerObj.sceneLoader.LoadScene("MultiSinglePlayerScene");
        gamemanagerObj.curState = GameStates.multiSingle;
    }

    public void StayInLobby()
    {
        GameObject stayButton = GetButton("StayInLobbyButton");
        stayButton.SetActive(false);
        waiting.gameObject.SetActive(true);

        postgamemanager.EnterDecisionRpc(PlayerDecisions.Staying);
    }

    // helper method to get a specific button
    private GameObject GetButton(string name)
    {
        foreach(GameObject obj in buttonOptions)
        {
            if (buttonOptions[buttonOptions.IndexOf(obj)].name == name)
            {
                return obj;
            }
        }
        return null;
    }

    // helper method that checks if only the host is connected
    private bool OnlyHostConnected()
    {
        return NetworkManager.Singleton.ConnectedClientsIds.Count == 1 &&
               NetworkManager.Singleton.ConnectedClientsIds.Contains((ulong)0);
    }

    private IEnumerator HostExit()
    {
        StartCoroutine(AnimateText("All Players Left. Returning to Mode Select. . ."));
        yield return new WaitForSeconds(3);
        LeaveLobby();
    }

    private void ShowLeaver()
    {
        Dictionary<ulong, PlayerDecisions> players = postgamemanager.GetClientsList();
        for (int i = 0; i < players.Count; i++)
        {
            PlayerDecisions decision;
            players.TryGetValue((ulong)i, out decision);
            if (decision == PlayerDecisions.Leaving)
            {
                StartCoroutine(AnimateText($"client {i} has left."));
            }
        }
    }

    private IEnumerator AnimateText(string txt)
    {
        if (playerLeft != null)
        {
            playerLeft.text = txt;
            playerLeft.gameObject.SetActive(true);
            playerLeft.color = new Color(playerLeft.color.r, playerLeft.color.g, playerLeft.color.b, 1);
            float dt;
            while (playerLeft.color.a > 0)
            {
                dt = Time.deltaTime;
                playerLeft.color = new Color(playerLeft.color.r, playerLeft.color.g, playerLeft.color.b, Mathf.Max(0, playerLeft.color.a - dt));
                yield return new WaitForSeconds(dt);
            }

            playerLeft.gameObject.SetActive(false);
            playerLeft.color = new Color(playerLeft.color.r, playerLeft.color.g, playerLeft.color.b, 1);
        }

    }
}
