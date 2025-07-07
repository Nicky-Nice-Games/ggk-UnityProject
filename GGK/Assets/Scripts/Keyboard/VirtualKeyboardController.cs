using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VirtualKeyboardController : MonoBehaviour
{
    [SerializeField] public TMP_InputField inputField;
    [SerializeField] private List<Button> keyButtons;
    private int selectedIndex = 0;
    private int rowSize = 10;
    private string curText;
    private bool toCapitol = false;

    // controller variables
    private const float threshold = 0.5f;
    private bool prevRight;
    private bool prevLeft;
    private bool prevUp;
    private bool prevDown;

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

        // read controller left stick and dpad inputs
        float stickX = Input.GetAxisRaw("Horizontal");
        float stickY = Input.GetAxisRaw("Vertical");
        float dpadX = Input.GetAxisRaw("DPadX");          
        float dpadY = Input.GetAxisRaw("DPadY");

        // read keyboard inputs
        bool kbRight = Input.GetKeyDown(KeyCode.RightArrow);
        bool kbLeft = Input.GetKeyDown(KeyCode.LeftArrow);
        bool kbDown = Input.GetKeyDown(KeyCode.DownArrow);
        bool kbUp = Input.GetKeyDown(KeyCode.UpArrow);
        bool kbReturn = Input.GetKeyDown(KeyCode.Return);

        // check if input passes threshold and reads as full press
        bool gpRight = stickX > threshold || dpadX > threshold;
        bool gpLeft = stickX < -threshold || dpadX < -threshold;
        bool gpUp = stickY > threshold || dpadY > threshold;
        bool gpDown = stickY < -threshold || dpadY < -threshold;
        bool gpEnter = Input.GetKeyDown(KeyCode.JoystickButton0);

        // Nav Controls
        if (kbRight || (gpRight && !prevRight)) selectedIndex++;
        else if (kbLeft || (gpLeft && !prevLeft)) selectedIndex--;
        else if ((kbUp || (gpUp && !prevUp)) && selectedIndex != 40) selectedIndex -= rowSize;
        else if ((kbUp || (gpUp && !prevUp)) && selectedIndex == 40) selectedIndex -= rowSize + 1;
        else if (kbDown || (gpDown && !prevDown)) selectedIndex += rowSize;
        else if (kbReturn || gpEnter) keyButtons[selectedIndex].onClick.Invoke();

        // Making sure the index does not go out of bounds
        selectedIndex = Mathf.Clamp(selectedIndex, 0, keyButtons.Count - 1);

        if (oldIndex != selectedIndex)
        {
            HighlightButton(selectedIndex);
        }

        // pressing B on controller deletes a character
        if (Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            KeyPressed("Backspace");
        }

        // keep a reference to previous input so index is only incremented once per press
        prevRight = gpRight;
        prevLeft = gpLeft;
        prevDown = gpDown;
        prevUp = gpUp;
    }

    /// <summary>
    /// Handles highlighting the uttons a different color
    /// </summary>
    /// <param name="index"></param>
    void HighlightButton(int index)
    {
        // Clear Unity's automatic selection to avoid grey highlight
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

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
        // Temporary fix so that when switching text inputs
        // the current text will be set to the input's text 
        // when typing/using virtual keyboard
        curText = inputField.text;
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

