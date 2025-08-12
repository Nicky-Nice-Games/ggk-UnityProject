using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectSoundHandler : MonoBehaviour
{
    [SerializeField] private List<Button> confirmButtons = new List<Button>();
    [SerializeField] private List<Button> backButtons = new List<Button>();
    [SerializeField] private Button gizmoButton;
    [SerializeField] private Button morganButton;
    [SerializeField] private Button reeseButton;
    [SerializeField] private Button emmaButton;
    [SerializeField] private Button kaiButton;
    [SerializeField] private Button jamsterButton;
    private UIClickSound clickSound;
    private AnnouncerLines announcerLines;


    // Start is called before the first frame update
    void Start()
    {
        clickSound = FindAnyObjectByType<UIClickSound>();
        announcerLines = FindAnyObjectByType<AnnouncerLines>();

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

        // Play "Gizmo" Announcer Voice line
        gizmoButton.onClick.AddListener(() =>
        announcerLines.PlayGizmo());

        // Play "Morgan" Announcer Voice line
        morganButton.onClick.AddListener(() =>
        announcerLines.PlayMorgan());

        // Play "Reese" Announcer Voice line
        reeseButton.onClick.AddListener(() =>
        announcerLines.PlayReese());

        // Play "Emma" Announcer Voice line
        emmaButton.onClick.AddListener(() =>
        announcerLines.PlayEmma());

        // Play "Kai" Announcer Voice line
        kaiButton.onClick.AddListener(() =>
        announcerLines.PlayKai());

        // Play "Jamster" Announcer Voice line
        jamsterButton.onClick.AddListener(() =>
        announcerLines.PlayJamster());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
