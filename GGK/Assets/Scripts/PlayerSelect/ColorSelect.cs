// 7/31/25
// Joshua Chisholm
// Button behavior for color select buttons
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ColorSelect : MonoBehaviour
{
    // Start is called before the first frame update

    private CharacterData charData;
    private GameManager gameManager;
    [SerializeField]
    private TextMeshProUGUI colorDisplay;

    [SerializeField]
    private Sprite buttonDeselected;

    [SerializeField]
    private Sprite buttonSelectedSprite;

    [SerializeField]
    private GameObject buttons;


    void Start()
    {
        // Get character data
        charData = FindAnyObjectByType<CharacterData>();
        gameManager = FindAnyObjectByType<GameManager>();
    }


    /// <summary>
    /// On click for the color select buttons. Will record player's choice
    /// </summary>
    /// <param name="color"></param>
    public void SelectColor(Button color)
    {
        // Get all buttons and update sprites
        Button[] colors = buttons.transform.GetComponentsInChildren<Button>();
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i].GetComponent<Image>().sprite = buttonDeselected;
        }

        // If character data exists, update choice and update display afterwards
        if (charData != null)
        {
            charData.characterColor = color.transform.GetChild(0).GetComponent<Image>().color;
        }
        color.GetComponent<Image>().sprite = buttonSelectedSprite;
        colorDisplay.text = color.name;
    }

    /// <summary>
    /// If a choice has been made proceed to next scene.
    /// </summary>
    public void Confirm()
    {
        if (charData.characterColor != default)
        {
            SendAppearanceToPlayerInfo();
            gameManager.PlayerSelected();
        }
    }

    /// <summary>
    /// Organizes the models name to a number based off of organization in backend
    /// </summary>
    private void SendAppearanceToPlayerInfo()
    {
        int index = 0;
        switch (charData.characterName)
        {
            case "Gizmo":
                index = 1;
                break;

            case "Morgan":
                index = 2;
                break;

            case "Reese":
                index = 3;
                break;

            case "Emma":
                index = 4;
                break;

            case "Kai":
                index = 5;
                break;

            case "Jamster":
                index = 6;
                break;

            default:
                index = 1;
                break;
        }
        if (gameManager.playerInfo != null)
        {
            gameManager.playerInfo.characterUsed = index;
            //Debug.Log("Character selected: " + gameManager.playerInfo.characterUsed + "\nShould be: " + index);
        }
        else
        {
            //Debug.Log("Playerinfo was null!");
        }
    }

}
