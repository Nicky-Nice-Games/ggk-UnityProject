using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerKartHandeler : MonoBehaviour
{
    // References
    public GameManager gameManager;
    public List<GameObject> playerOptions;

    public CharacterData characterData;
    public Image characterSelectedImage;
    public TextMeshProUGUI characterName;
    private Image prevCharacterImageBorder;
    private Image prevColorImageBorder;

    // Reference Lists
    public List<GameObject> characterButtons;
    public List<GameObject> colorButtons;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        characterData = FindAnyObjectByType<CharacterData>();

        // Connect character buttons to ChangeCharacter with appropriate arguments
        foreach (GameObject characterButton in characterButtons)
        {
            Image[] images = characterButton.GetComponentsInChildren<Image>();

            Button btn = characterButton.GetComponentInChildren<Button>();

            btn.onClick.AddListener(() => ChangeCharacter(images[2], images[0], btn.name));
        }

        // Connect color buttons to ChangeColor with appropriate arguments
        foreach (GameObject colorButton in colorButtons)
        {
            Image[] images = colorButton.GetComponentsInChildren<Image>();

            Button btn = colorButton.GetComponentInChildren<Button>();

            btn.onClick.AddListener(() => ChangeColor(images[1], images[0]));
        }

        // Invoke default selection
        characterButtons[0].GetComponentInChildren<Button>().onClick.Invoke();
        colorButtons[0].GetComponentInChildren<Button>().onClick.Invoke();

        // Assigning the buttons their listeners
        foreach (GameObject obj in playerOptions)
        {
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            gameManager.GetComponent<ButtonBehavior>().OnClick());
            button.onClick.AddListener(() =>
            gameManager.GetComponentInChildren<GameManager>().PlayerSelected());
        }
    }

    public void ChangeCharacter(Image characterImage, Image border, string name)
    {
        if (prevCharacterImageBorder != null)
        {
            prevCharacterImageBorder.color = Color.gray;
        }

        characterSelectedImage.sprite = characterImage.sprite;
        if (characterData != null)
            characterData.characterSprite = characterImage.sprite;
        border.color = Color.yellow;
        prevCharacterImageBorder = border;
        characterName.text = name;
    }
    public void ChangeColor(Image color, Image border)
    {
        if (prevColorImageBorder != null)
        {
            prevColorImageBorder.color = Color.black;
        }

        characterSelectedImage.color = color.color;
        if (characterData != null)
            characterData.characterColor = color.color;
        border.color = Color.yellow;
        prevColorImageBorder = border;
    }
}
