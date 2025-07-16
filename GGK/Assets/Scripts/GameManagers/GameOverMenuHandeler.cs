using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

public class GameOverMenuHandeler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> mapOptions = new List<GameObject>();
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
        foreach (GameObject obj in mapOptions)
        {
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            gamemanagerObj.GetComponent<ButtonBehavior>().OnClick());
        }

        if (MultiplayerManager.Instance.IsMultiplayer)
        {
            multiplayerPanel.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OpenPlayAgain()
    {
        if (!playAgainPanel.activeSelf)
        {
            playAgainPanel.SetActive(true);
        }
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
        gamemanagerObj.ToMapSelectScreenRpc();
    }

    public void ReturnToCharMulti()
    {
        gamemanagerObj.LoadedGameModeRpc();
    }

    public void ReturnToModesMulti()
    {
        gamemanagerObj.ToGameModeSelectSceneRpc();
    }
}
