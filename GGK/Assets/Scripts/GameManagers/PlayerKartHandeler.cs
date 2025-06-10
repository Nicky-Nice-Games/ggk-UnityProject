using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerKartHandeler : MonoBehaviour
{
    // References
    public GameManager gameManager;
    public Image characterSelectImage;
    private Image prevImageBorder;

    // Reference Lists
    public List<GameObject> characterButtons;
    public List<GameObject> colorButtons;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        // So scene can still work if not started from start scene
        if ( gameManager != null )
        {
            gameManager.characterSprite = gameManager.currentSceneFirst.GetComponent<Image>().sprite;
            gameManager.characterColor = Color.white;
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

    public void PlayerSelected()
    {
        gameManager.PlayerSelected();
    }

    public void ChangeCharacter(Image characterImage, Image border)
    {
        if (prevImageBorder != null)
        {
            prevImageBorder.color = Color.white;
        }

        characterSelectImage.sprite = characterImage.sprite;
        if (gameManager != null) 
            gameManager.characterSprite = characterImage.sprite;
        border.color = Color.yellow;
        prevImageBorder = border;
    }
    public void ChangeColor(Image color)
    {
        characterSelectImage.color = color.color;
        if (gameManager != null)
            gameManager.characterColor = color.color;
    }
}
