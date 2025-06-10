using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerKartHandler : MonoBehaviour
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
        gameManager.characterSprite = gameManager.currentSceneFirst.GetComponent<Image>().sprite;
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
        gameManager.characterSprite = characterImage.sprite;
        border.color = Color.yellow;
        prevImageBorder = border;
    }
    public void ChangeColor(Image color)
    {
        characterSelectImage.color = color.color;
        gameManager.characterColor = color.color;
    }
}
