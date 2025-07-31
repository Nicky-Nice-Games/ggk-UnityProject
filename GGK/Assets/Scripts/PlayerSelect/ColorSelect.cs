// 7/31/25
// Joshua Chisholm
// Button behavior for color select buttons
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelect : MonoBehaviour
{
    // Start is called before the first frame update

    private CharacterData charData;
    [SerializeField]
    private TextMeshProUGUI colorDisplay;

    void Start()
    {
        // Get character data
        charData = FindAnyObjectByType<CharacterData>();
    }

    public void SelectColor(Button color)
    {
        if (charData != null)
        {
            charData.characterColor = color.transform.GetChild(0).GetComponent<Image>().color;
        }
        colorDisplay.text = color.name;
    }
}
