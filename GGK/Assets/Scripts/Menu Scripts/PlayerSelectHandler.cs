using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectHandler : MonoBehaviour
{
    // References
    public GameManager gameManager;
    public Image characterSelectImage;
    private Image prevImageBorder;

    // Reference Lists
    public List<GameObject> characterButtons;
    public List<GameObject> colorButtons;

    void Start()
    {
        gameManager = GameObject.FindAnyObjectByType<GameManager>();
        // gameManager.characterSprite = characterButtons[0].GetComponentsInChildren<Image>()[2].sprite;
        gameManager.characterSprite = gameManager.firstInteractable.GetComponent<Image>().sprite;
        gameManager.characterColor = Color.white;

        foreach (GameObject characterButton in characterButtons)
        {
            Image[] images = characterButton.GetComponentsInChildren<Image>(); 

            Button btn = characterButton.GetComponentInChildren<Button>();

            btn.onClick.AddListener(() => ChangeCharacter(images[2], images[0]));
        }

        foreach (GameObject colorButton in colorButtons)
        {
            Image[] images = colorButton.GetComponentsInChildren<Image>();

            Button btn = colorButton.GetComponentInChildren<Button>();

            btn.onClick.AddListener(() => ChangeColor(images[1]));
        }
    }

    // Taking the relevant buttons from Game Manager
    public void PlayerSelected()
    {
        gameManager.PlayerSelected();
    }

    // Changing Character
    public void ChangeCharacter(Image characterImage, Image border)
    {
        if (prevImageBorder != null)
        {
            prevImageBorder.color = Color.white;
        }

        characterSelectImage.sprite = characterImage.sprite;
        gameManager.characterSprite = characterImage.sprite;
        border.color = Color.yellow;
        prevImageBorder = border;
    }

    // Changing Color
    public void ChangeColor(Image color)
    {
        characterSelectImage.color = color.color;
        gameManager.characterColor = color.color;
    }
}
