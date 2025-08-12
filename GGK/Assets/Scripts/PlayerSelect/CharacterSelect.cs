// Joshua Chisholm, Yusef
// 8/7/25
// Character select logic
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    // References 

    [SerializeField]
    private GameObject buttons;

    [SerializeField]
    private Sprite buttonDeselected;

    [SerializeField]
    private Sprite buttonSelectedSprite;

    [SerializeField]
    private TextMeshProUGUI characterNameDisplay;

    [SerializeField]
    private List<GameObject> characterModels;
    private GameObject kart;
    private GameManager gameManager;

    [SerializeField]
    private GameObject colorSelectMenu;
    [SerializeField]
    private GameObject charSelectMenu;

    [SerializeField]
    private EventSystem eventSystem;

    [SerializeField]
    private GameObject firstColor;

    [SerializeField]
    private Button colorOptionsPanel;
    [SerializeField]
    private Button charOptionsPanel;
    [SerializeField]
    private GameObject kartDisplay;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    /// <summary>
    /// On click for character buttons. It will record the player's choice
    /// </summary>
    /// <param name="character">The button the user selected</param>
    public void SelectCharacter(Button character)
    {
        // Get all the buttons and update sprites
        Button[] characters = buttons.GetComponentsInChildren<Button>();
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].GetComponent<Image>().sprite = buttonDeselected;
        }

        // If character data exists, set player's choice and update display
        if (CharacterData.Instance != null)
        {
            CharacterData.Instance.characterName = character.name;
            CharacterData.Instance.characterSprite = character.gameObject.transform.GetChild(0).GetComponent<Image>().sprite;
        }
        character.GetComponent<Image>().sprite = buttonSelectedSprite;
        characterNameDisplay.text = character.name;


        // Logic for the character model visuals. Set all the models
        // to be not be active, but then set the chosen character's
        // model to be active and visible
        foreach(GameObject model in characterModels)
        {
            model.SetActive(false);
        }
        switch (character.name)
        {
            case "Gizmo":
                characterModels[0].SetActive(true);
                break;
            case "Morgan":
                characterModels[1].SetActive(true);
                break;
            case "Reese":
                characterModels[2].SetActive(true);
                break;
            case "Emma":
                characterModels[4].SetActive(true);
                break;
            case "Kai":
                characterModels[3].SetActive(true);
                break;
            case "Jamster":
                characterModels[5].SetActive(true);
                break;
        }
    }


    /// <summary>
    /// Confirms the player's choice and continues to color selects
    /// </summary>
    public void Confirm()
    {
        if (CharacterData.Instance.characterName != "")
        {
            colorSelectMenu.SetActive(true);
            kartDisplay.SetActive(true);
            charSelectMenu.SetActive(false);
            eventSystem.SetSelectedGameObject(firstColor);
            colorOptionsPanel.onClick = charOptionsPanel.onClick;

            // disable all character models
            foreach (GameObject model in characterModels)
            {
                model.SetActive(false);
            }
        }
    }
}