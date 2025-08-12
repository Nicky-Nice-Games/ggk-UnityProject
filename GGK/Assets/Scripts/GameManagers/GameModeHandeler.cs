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
    // [SerializeField] private Transform gameModeButtons;
    // [SerializeField] private Transform waitingScreen;

    [SerializeField] private List<Button> confirmButtons = new List<Button>();
    [SerializeField] private List<Button> backButtons = new List<Button>();
    [SerializeField] private Button grandPrixButton;
    [SerializeField] private Button raceButton;
    [SerializeField] private Button timeTrialButton;
    private UIClickSound clickSound;
    private AnnouncerLines announcerLines;

    // Start is called before the first frame update
    void Start()
    {
        /*
        if single player show gamemode options
        else if multiplayer host show gamemode options 
        else if multiplayer client show waiting screen
        */
        gamemanagerObj = FindAnyObjectByType<GameManager>();
        clickSound = FindAnyObjectByType<UIClickSound>();
        announcerLines = FindAnyObjectByType<AnnouncerLines>();

        // Assigning the buttons their listeners
        foreach (GameObject obj in modeOptions)
        {
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            gamemanagerObj.GetComponent<ButtonBehavior>().OnClick());
            // checks for each mode's button
            // likely to change when Grand Prix & Time Trial are actually implemented
            if(obj.name == "Race")
            {
                button.onClick.AddListener(() =>
                gamemanagerObj.GetComponent<GameManager>().LoadedGameMode());

                button.onClick.AddListener(() =>
                gamemanagerObj.ChangeGameMode(GameModes.quickRace));
            }
            else if(obj.name == "Time Trial")
            {
                button.onClick.AddListener(() =>
                gamemanagerObj.GetComponent<GameManager>().LoadedGameMode());

                button.onClick.AddListener(() =>
                gamemanagerObj.ChangeGameMode(GameModes.timeTrial));
            }
            else if(obj.name == "Grand Prix")
            {
                button.onClick.AddListener(() =>
                gamemanagerObj.GetComponent<GameManager>().LoadedGameMode());

                button.onClick.AddListener(() =>
                gamemanagerObj.ChangeGameMode(GameModes.grandPrix));
            }
        }

        // Buttons playing Confirm Sound
        foreach (Button button in confirmButtons)
        {
            button.onClick.AddListener(() =>
            clickSound.onClickConfirm());
        }

        // Buttons playing Back Sound
        foreach (Button button in backButtons)
        {
            button.onClick.AddListener(() =>
            clickSound.onClickBack());
        }

        // Play "Grand Prix" Announcer Voice line
        grandPrixButton.onClick.AddListener(() =>
        announcerLines.PlayGrandPrix());

        // Play "Race" Announcer Voice line
        raceButton.onClick.AddListener(() =>
        announcerLines.PlayRace());

        // Play "Time Trial" Announcer Voice line
        timeTrialButton.onClick.AddListener(() =>
        announcerLines.PlayTimeTrial());

        // this is to bring up the waiting screens for clients when in multiplayer
        // if (!MultiplayerManager.Instance.IsMultiplayer)
        // {
        //     waitingScreen.gameObject.SetActive(false);
        //     gameModeButtons.gameObject.SetActive(true);
        // }
        // else
        // {
        //     if (MultiplayerManager.Instance.IsHost)
        //     {
        //         waitingScreen.gameObject.SetActive(false);
        //         gameModeButtons.gameObject.SetActive(true);
        //     }
        //     else
        //     {
        //         gameModeButtons.gameObject.SetActive(false);
        //         waitingScreen.gameObject.SetActive(true);
        //     }
        // }
    }

    // Each game mode can load their own stuff
}
