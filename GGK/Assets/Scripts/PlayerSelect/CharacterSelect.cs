// Joshua Chisholm
// 8/7/25
// Character select logic
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
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
    private GameManager gameManager;

    [SerializeField]
    private GameObject colorSelectMenu;
    [SerializeField]
    private GameObject charSelectMenu;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    /// On click for character buttons. It will record the player's choice
    /// </summary>
    /// <param name="character"></param>
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
    }


    /// <summary>
    /// Confirms the player's choice and continues to color selects
    /// </summary>
    public void Confirm()
    {
        if (CharacterData.Instance.characterName != null)
        {
            colorSelectMenu.SetActive(true);
            charSelectMenu.SetActive(false);
        }
    }
}