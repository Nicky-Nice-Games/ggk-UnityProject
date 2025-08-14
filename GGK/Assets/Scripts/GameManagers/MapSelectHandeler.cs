using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using TMPro;

/// <summary>
/// Map Enum by Phillip Brown
/// Used to keep track of map votes for multiplayer
/// </summary>
public enum Map
{
    RITOuterLoop,
    RITQuarterMile,
    RITDorm,
    FinalsBrickRoad,
    Golisano,
    RITWoods,
}

public class MapSelectHandeler : MonoBehaviour
{
    [SerializeField] private List<GameObject> mapOptions = new List<GameObject>();
    private GameManager gamemanagerObj;
    [SerializeField]
    private TextMeshProUGUI trackDisplay;

    [SerializeField]
    private Button confirmBtn;
    // [SerializeField] private Transform waitingScreen;
    // [SerializeField] private Transform mapButtons;

    [SerializeField]
    private Sprite mapSelectedTexture;
    [SerializeField]
    private Sprite mapDefaultTexture;
    [SerializeField]
    private Image trackPreview;

    [SerializeField] private List<Button> confirmButtons = new List<Button>();
    [SerializeField] private List<Button> backButtons = new List<Button>();
    [SerializeField] private Button campusButton;
    [SerializeField] private Button dormButton;
    [SerializeField] private Button techHouseButton;
    [SerializeField] private Button allNighterButton;
    private UIClickSound clickSound;
    private AnnouncerLines announcerLines;

    // Start is called before the first frame update
    void Start()
    {
        gamemanagerObj = FindAnyObjectByType<GameManager>();
        clickSound = FindAnyObjectByType<UIClickSound>();
        announcerLines = FindAnyObjectByType<AnnouncerLines>();
        // waitingScreen.gameObject.SetActive(false);
        // mapButtons.gameObject.SetActive(true);

        // Assigning the buttons their listeners
        foreach (GameObject obj in mapOptions)
        {
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            gamemanagerObj.GetComponent<ButtonBehavior>().OnClick());
            button.onClick.AddListener(() =>
            {
                CharacterData.Instance.mapVote = obj.name;
                trackDisplay.text = obj.name;
                trackPreview.sprite = obj.transform.GetChild(0).GetComponent<Image>().sprite;
                foreach (GameObject button in mapOptions)
                {
                    button.GetComponent<Image>().sprite = mapDefaultTexture;
                }
                obj.GetComponent<Image>().sprite = mapSelectedTexture;
            });

            //button.onClick.AddListener(() => DisableButtons());
        }

        if (MusicStateManager.instance != null)
        {
            MusicResultsStateManager.instance.SetResultsState(ResultsState.None);
            MusicLapStateManager.instance.SetLapState(LapState.None);
            MusicStateManager.instance.SetMusicState(MusicState.Menu);
        }
        confirmBtn.onClick.AddListener(() => ConfirmVote());

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

        // Play "Campus Circuit" Announcer Voice line
        campusButton.onClick.AddListener(() =>
        announcerLines.PlayCampusCircuit());

        // Play "Dorm Room Derby" Announcer Voice line
        dormButton.onClick.AddListener(() =>
        announcerLines.PlayDormRoomDerby());

        // Play "Tech House Turnpike" Announcer Voice line
        techHouseButton.onClick.AddListener(() =>
        announcerLines.PlayTechHouseTurnpike());

        // Play "All Nighter Expressway" Announcer Voice line
        allNighterButton.onClick.AddListener(() =>
        announcerLines.PlayAllNighterExpressway());
    }

    public void ConfirmVote()
    {
        gamemanagerObj.MapSelected();
    }

    private void DisableButtons()
    {
        foreach(GameObject obj in mapOptions)
        {
            obj.GetComponent<Button>().enabled = false;
        }
    }

    public void SwitchToWaitingScreen()
    {
        // mapButtons.gameObject.SetActive(false);
        // waitingScreen.gameObject.SetActive(true);
    }
    public void SwitchToMapPanel()
    {
        // mapButtons.gameObject.SetActive(true);
        // waitingScreen.gameObject.SetActive(false);
    }


    // Each button can ready its own map
}
