using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
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

    public void SelectCharacter(Button character)
    {
        Button[] characters = buttons.GetComponentsInChildren<Button>();
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].GetComponent<Image>().sprite = buttonDeselected;
        }

        // for (int i = 0; i < characterModels.Count; i++)
        // {
        //     if (characterModels[i].name == character.name)
        //     {
        //         characterModels[i].SetActive(true);
        //     }
        //     else
        //     {
        //         characterModels[i].SetActive(false);
        //     }
        // }

        if (CharacterData.Instance != null)
        {
            CharacterData.Instance.characterName = character.name;
            CharacterData.Instance.characterSprite = character.gameObject.transform.GetChild(0).GetComponent<Image>().sprite;
        }
        character.GetComponent<Image>().sprite = buttonSelectedSprite;
        characterNameDisplay.text = character.name;
    }

    public void Confirm()
    {
        if (CharacterData.Instance.characterName != null)
        {
            colorSelectMenu.SetActive(true);
            charSelectMenu.SetActive(false);
        }
    }
}