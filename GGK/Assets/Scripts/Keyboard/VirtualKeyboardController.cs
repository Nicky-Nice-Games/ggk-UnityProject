using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VirtualKeyboardController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private List<Button> keyButtons;
    private int selectedIndex = 0;
    private int rowSize = 10;
    private string curText;
    private bool toCapitol = false;

    void Start()
    {
        HighlightButton(selectedIndex);
    }

    void Update()
    {
        HandleNavigation();
    }

    void HandleNavigation()
    {
        int oldIndex = selectedIndex;

        // Nav controls
        if (Input.GetKeyDown(KeyCode.RightArrow)) selectedIndex++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) selectedIndex--;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) selectedIndex -= rowSize;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) selectedIndex += rowSize;
        else if (Input.GetKeyDown(KeyCode.Return)) keyButtons[selectedIndex].onClick.Invoke();

        // Making sure the index does not go out of bounds
        selectedIndex = Mathf.Clamp(selectedIndex, 0, keyButtons.Count - 1);

        if (oldIndex != selectedIndex)
        {
            HighlightButton(selectedIndex);
        }
    }

    /// <summary>
    /// Handles highlighting the uttons a different color
    /// </summary>
    /// <param name="index"></param>
    void HighlightButton(int index)
    {
        // This could be done differentally to better optimize but works for now
        for (int i = 0; i < keyButtons.Count; i++)
        {
            ColorBlock colors = keyButtons[i].colors;
            colors.normalColor = (i == index) ? Color.yellow : Color.white;
            keyButtons[i].colors = colors;
        }
    }

    /// <summary>
    /// Handles direct changes to the imput field
    /// </summary>
    /// <param name="value"></param>
    public void KeyPressed(string value)
    {
        // Backspace
        if (value == "Backspace")
        {
            // Making sure the text is not empty then taking the substring
            if (curText.Length > 0)
            {
                curText = curText.Substring(0, curText.Length - 1);
            }
        }

        // Space will prob not be included
        else if (value == "SPACE")
        {
            curText += " ";
        }

        // Letters and shift
        else
        {
            // Checking for shift key to handle capitalization
            if (value == "Shift")
            {
                toCapitol = true;
                return;
            }

            // Checking for next letter capitalization
            if (toCapitol)
            {
                curText += value.ToUpper();
                toCapitol = false;
            }
            else
            {
                curText += value;
            }
        }
        inputField.text = curText;
    }
}

