using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> characterButtons;

    [SerializeField]
    private Sprite buttonDeselected;

    [SerializeField]
    private Sprite buttonSelectedSprite;

    [SerializeField]
    private TextMeshProUGUI characterNameDisplay;

    private CharacterData characterData;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        characterData = FindObjectOfType<CharacterData>();
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectCharacter(Button character)
    {
        for (int i = 0; i < characterButtons.Count; i++)
        {
            characterButtons[i].GetComponent<Image>().sprite = buttonDeselected;
        }
        if (characterData != null)
        {
            characterData.characterName = character.name;
            characterData.characterSprite = character.gameObject.transform.GetChild(0).GetComponent<Image>().sprite;
        }
        character.GetComponent<Image>().sprite = buttonSelectedSprite;
        characterNameDisplay.text = character.name;
    }

    public void Confirm()
    {
        if (characterData.characterName != null)
        {
            gameManager.PlayerSelected();
        }
    }
}
