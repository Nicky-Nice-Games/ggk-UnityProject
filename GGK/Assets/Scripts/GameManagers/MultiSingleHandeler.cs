using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

public class MultiSingleHandeler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> connectionOptions = new List<GameObject>();
    private GameManager gamemanagerObj;

    [SerializeField] private List<Button> confirmButtons = new List<Button>();
    [SerializeField] private List<Button> backButtons = new List<Button>();
    [SerializeField] private Button singleplayerButton;
    [SerializeField] private Button multiplayerButton;
    private UIClickSound clickSound;
    private AnnouncerLines announcerLines;

    // Start is called before the first frame update
    void Start()
    {
        gamemanagerObj = FindAnyObjectByType<GameManager>();
        clickSound = FindAnyObjectByType<UIClickSound>();
        announcerLines = FindAnyObjectByType<AnnouncerLines>();

        // Assigning the buttons their listeners
        foreach (GameObject obj in connectionOptions)
        {
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            gamemanagerObj.GetComponent<ButtonBehavior>().OnClick());
            button.onClick.AddListener(() =>
            gamemanagerObj.GetComponent<GameManager>().MultiSingleConnect());
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

        // Play "Singleplayer" Announcer Voice line
        singleplayerButton.onClick.AddListener(() =>
        announcerLines.PlaySingleplayer());

        // Play "Multiplayer" Announcer Voice line
        multiplayerButton.onClick.AddListener(() =>
        announcerLines.PlayMultiplayer());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
