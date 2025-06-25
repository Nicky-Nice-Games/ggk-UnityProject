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
        if (value == "BACK")
        {
            if (inputField.text.Length > 0)
                inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }

        // Space will prob not be included
        else if (value == "SPACE")
        {
            inputField.text += " ";
        }
        else
        {
            inputField.text += value;
        }
    }
}

