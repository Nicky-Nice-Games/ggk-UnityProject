using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapTextDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI mapLabel;

    [SerializeField]
    private Sprite buttonDeselected;

    [SerializeField]
    private Sprite buttonSelectedSprite;
    
    
    public void OnSelect(GameObject button)
    {
        mapLabel.text = button.name;
        button.GetComponent<Image>().sprite = buttonSelectedSprite;
    }

    public void OnDeselect(GameObject button)
    {
        button.GetComponent<Image>().sprite = buttonDeselected;
    }
}
