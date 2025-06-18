using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

public class GameModeHandeler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> modeOptions = new List<GameObject>();
    private GameManager gamemanagerObj;
    [SerializeField] private Transform gameModeButtons;
    [SerializeField] private Transform waitingScreen;


    // Start is called before the first frame update
    void Start()
    {
        /*
        if single player show gamemode options
        else if multiplayer host show gamemode options 
        else if multiplayer client show waiting screen
        */
        gamemanagerObj = FindAnyObjectByType<GameManager>();

        // Assigning the buttons their listeners
        foreach (GameObject obj in modeOptions)
        {
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            gamemanagerObj.GetComponent<ButtonBehavior>().OnClick());
            button.onClick.AddListener(() =>
            gamemanagerObj.GetComponent<GameManager>().LoadedGameMode());
        }

        if (!MultiplayerManager.Instance.IsMultiplayer)
        {
            waitingScreen.gameObject.SetActive(false);
            gameModeButtons.gameObject.SetActive(true);
        }
        else
        {
            if (MultiplayerManager.Instance.IsHost)
            {
                waitingScreen.gameObject.SetActive(false);
                gameModeButtons.gameObject.SetActive(true);
            }
            else
            {
                gameModeButtons.gameObject.SetActive(false);
                waitingScreen.gameObject.SetActive(true);
            }
        }
    }

    // Each game mode can load their own stuff
}
