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
    private List<GameObject> buttons;


    void Start()
    {
        // Get character data
        charData = FindAnyObjectByType<CharacterData>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void SelectColor(Button color)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].GetComponent<Image>().sprite = buttonDeselected;
        }
        if (charData != null)
        {
            charData.characterColor = color.transform.GetChild(0).GetComponent<Image>().color;
        }
        color.GetComponent<Image>().sprite = buttonSelectedSprite;
        colorDisplay.text = color.name;
    }

    public void Confirm()
    {
        if(charData.characterColor != null)
        {
            SceneManager.LoadScene("PlayerKartScene");
        }
    }
}

