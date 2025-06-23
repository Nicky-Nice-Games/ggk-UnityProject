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
    public SpriteRenderer characterSelectedImage;
    public TextMeshProUGUI characterName;
    private Image prevCharacterImageBorder;

    public GameObject kartModel;
    private int rotation = 140;

    // Reference Lists
    public List<GameObject> characterButtons;
    public List<GameObject> colorButtons;
    public List<Color> colors;

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

            btn.onClick.AddListener(() => ChangeCharacter(images[1], images[2], btn.name));
        }

        // Invoke default selection
        characterButtons[0].GetComponentInChildren<Button>().onClick.Invoke();
        ColorChange();

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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            RotateLeft();
        }
        else if (Input.GetKey(KeyCode.E))
        {
            RotateRight();
        }
    }

    // Select this character button
    public void ChangeCharacter(Image characterImage, Image border, string name)
    {
        // Reset previous character button's border color to gray
        if (prevCharacterImageBorder != null)
        {
            prevCharacterImageBorder.enabled = false;
        }

        // Change selected character information to those on the button
        characterSelectedImage.sprite = characterImage.sprite;
        characterName.text = name;

        // Save selected button information to change back later and change border color to yellow
        border.enabled = true;
        prevCharacterImageBorder = border;

        // Save information to characterData script if it exist
        if (characterData != null)
            characterData.characterSprite = characterImage.sprite;
    }

    // Which direction for the color carousel?
    public void LeftColor()
    {
        // Create a new list that shifts everything to the left
        List<Color> temp = new List<Color>();

        for (int i = 1; i < colorButtons.Count; i++)
        {
            temp.Add(colors[i]);
        }
        temp.Add(colors[0]);

        // New list overwrites old list
        colors = temp;

        // Call this to set color of the color carousel
        ColorChange();
    }
    public void RightColor()
    {
        // Create a new list that shifts everything to the Right
        List<Color> temp = new List<Color>();

        temp.Add(colors[colors.Count - 1]);

        for (int i = 0; i < colors.Count - 1; i++)
        {
            temp.Add(colors[i]);
        }

        // New list overwrites old list
        colors = temp;

        // Call this to set color of the color carousel
        ColorChange();
    }
    public void ColorChange()
    {
        // Color Carousel Update
        for (int i = 0; i < colorButtons.Count; i++)
        {
            Image[] images = colorButtons[i].GetComponentsInChildren<Image>();

            // This should be the inner color
            images[1].color = colors[i];
        }

        // Change sprite color (SHOULD THIS CHANGE THE COLOR OF THE SPRITE OR THE KART?)
        Color middleColor = colors[3];
        characterSelectedImage.color = middleColor;

        if (characterData != null)
            characterData.characterColor = middleColor;
    }

    // Kart Rotation
    public void RotateLeft()
    {
        rotation--;
        kartModel.transform.rotation = Quaternion.Euler(0, rotation, 0);
    }
    public void RotateRight()
    {
        rotation++;
        kartModel.transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

    /* Old Code
    // Select this color button
    public void ChangeColor(Image color, Image border)
    {
        // Reset previous color button's border color to black
        if (prevColorImageBorder != null)
        {
            prevColorImageBorder.color = Color.black;
        }

        // Change selected character information to those on the button
        characterSelectedImage.color = color.color;

        // Save selected button information to change back later and change border color to yellow
        prevColorImageBorder = border;

        // Save information to characterData script if it exist
        if (characterData != null)
            characterData.characterColor = color.color;
    }
    */
}
