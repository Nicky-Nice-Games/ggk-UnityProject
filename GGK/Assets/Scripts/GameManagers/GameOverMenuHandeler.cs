using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using Unity.Netcode;

public class GameOverMenuHandeler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> buttonOptions = new List<GameObject>();
    private GameManager gamemanagerObj;

    // panels with options tailored for multiplayer postgame
    [SerializeField]
    private GameObject multiplayerPanel;
    [SerializeField]
    private GameObject playAgainPanel;

    // Start is called before the first frame update
    void Start()
    {
        gamemanagerObj = FindAnyObjectByType<GameManager>();

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
    }

    // Update is called once per frame
    void Update()
    {
        
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
        // kick players if the host leaves (until host migration is implemented)

        MultiplayerManager.Instance.IsMultiplayer = false;
        gamemanagerObj.LoggedIn(); // to multisingle select
    }

    public void StayInLobby()
    {
        // take all players who pressed this to the next panel
        playAgainPanel.SetActive(true);
    }
}
