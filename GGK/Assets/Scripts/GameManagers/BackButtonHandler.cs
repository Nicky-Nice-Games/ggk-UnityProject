using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

public class BackButtonHandler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> backOption = new List<GameObject>();
    private GameManager gamemanagerObj;

    // Start is called before the first frame update
    void Start()
    {
        gamemanagerObj = FindAnyObjectByType<GameManager>();

        // Assigning the buttons their listeners
        foreach (GameObject obj in backOption)
        {
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            gamemanagerObj.GetComponent<ButtonBehavior>().OnClick());
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
            case GameStates.multiSingle:
                gamemanagerObj.LoadStartMenu();
                break;
            case GameStates.gameMode:
                gamemanagerObj.StartGame();
                break;
            case GameStates.playerKart:
            //PLEASE FIX THIS LATER -Giovanni Paulino
                //gamemanagerObj.sceneLoader.LoadScene("GameModeSelectScene");
                //gamemanagerObj.curState = GameStates.gameMode;
                break;
            case GameStates.map:
                gamemanagerObj.LoadedGameMode();
                break;
            default:
                break;
        }
    }
}