using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerKartHandeler : MonoBehaviour
{
    // References
    public GameManager gameManager;
    public CharacterData characterData;
    public List<GameObject> playerOptions;
    public Image characterSelectImage;
    private Image prevImageBorder;

    [SerializeField] private Transform playerSelectPanel;
    [SerializeField] private Transform waitingScreen;

    // Reference Lists
    public List<GameObject> characterButtons;
    public List<GameObject> colorButtons;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        characterData = FindAnyObjectByType<CharacterData>();
        waitingScreen.gameObject.SetActive(false);
        playerSelectPanel.gameObject.SetActive(true);

        // Assigning the buttons their listeners
        foreach (GameObject obj in playerOptions)
        {
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            gameManager.GetComponent<ButtonBehavior>().OnClick());
            button.onClick.AddListener(() =>
            gameManager.GetComponent<GameManager>().PlayerSelected());
        }

        // So scene can still work if not started from start scene
        if (characterData != null)
        {
            characterData.characterSprite = gameManager.currentSceneFirst.GetComponent<Image>().sprite;
            characterData.characterColor = Color.white;
        }

        // Connect character buttons to ChangeCharacter with appropriate arguments
        foreach (GameObject characterButton in characterButtons)
        {
            Image[] images = characterButton.GetComponentsInChildren<Image>();

            Button btn = characterButton.GetComponentInChildren<Button>();

            btn.onClick.AddListener(() => ChangeCharacter(images[2], images[0]));
        }

        // Connect color buttons to ChangeColor with appropriate arguments
        foreach (GameObject colorButton in colorButtons)
        {
            Image[] images = colorButton.GetComponentsInChildren<Image>();

            Button btn = colorButton.GetComponentInChildren<Button>();

            btn.onClick.AddListener(() => ChangeColor(images[1]));
        }
    }

    public void SwitchToWaitingScreen()
    {
        playerSelectPanel.gameObject.SetActive(false);
        waitingScreen.gameObject.SetActive(true);
    }
    public void SwitchToPlayerSelectPanel()
    {
        playerSelectPanel.gameObject.SetActive(true);
        waitingScreen.gameObject.SetActive(false);
    }

    public void ChangeCharacter(Image characterImage, Image border)
    {
        if (prevImageBorder != null)
        {
            prevImageBorder.color = Color.white;
        }

        characterSelectImage.sprite = characterImage.sprite;
        if (characterData != null)
            characterData.characterSprite = characterImage.sprite;
        border.color = Color.yellow;
        prevImageBorder = border;
    }
    public void ChangeColor(Image color)
    {
        characterSelectImage.color = color.color;
        if (characterData != null)
            characterData.characterColor = color.color;
    }
}
