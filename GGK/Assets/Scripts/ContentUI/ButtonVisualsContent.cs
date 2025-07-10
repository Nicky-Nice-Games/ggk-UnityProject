// Joshua Chisholm & Joshua Ward
// 7/10/25
// This script handles and stores and aethetic changes
// needed in code for menu buttons.
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ButtonVisualsContent : MonoBehaviour
{
    private float scaleFactor = 1.03f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Increases the scale of a button when hovering
    /// </summary>
    /// <param name="button"></param>
    public void OnHover(GameObject button)
    {
        button.transform.localScale = button.transform.localScale * scaleFactor;

        if (button.tag != "NoUnderline")
        {
            button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Underline;
        }
    }

    /// <summary>
    /// Returns to original scale of button
    /// </summary>
    /// <param name="button"></param>
    public void ExitHover(GameObject button)
    {
        button.transform.localScale = button.transform.localScale / scaleFactor;

        if (button.tag != "NoUnderline")
        {
            button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
        }
    }
}
