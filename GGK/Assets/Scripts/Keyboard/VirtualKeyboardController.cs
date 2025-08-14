using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEditor;
using System;

public class VirtualKeyboardController : MonoBehaviour
{
    public List<TMP_InputField> inputField = new List<TMP_InputField>();    // Ordered list according to screen visuals

    [HideInInspector]
    public int curField = 0;

    [SerializeField] private List<Button> keyButtons;
    private int selectedIndex = 0;
    private int rowSize = 10;
    private string curText;
    private bool toCapitol = false;
    private GameManager gameManager;

    // Script that holds the function to record the responces
    [SerializeField] private SignInManager signInScript;

    // controller variables
    private const float threshold = 0.5f;
    private bool prevRight;
    private bool prevLeft;
    private bool prevUp;
    private bool prevDown;

    // holding delete variables
    private bool holdingBackspace = false;
    private float timer = 0;

    //terrible terrible bandaid fix to a problem with being unable to use the
    //virtual keyboard.. FIX WHEN THERE IS TIME (if this project gets picked back up in the future)!!!
    //this used to be called "a" cuz we had like 10 minutes left on work when we typed it. whoops
    public bool inputFieldSelected;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        // HandleNavigation();

        // allows user to hold button on controllers to delete multiple characters
        if (holdingBackspace)
        {
            timer += Time.deltaTime;

            if (timer > 0.15f)
            {
                KeyPressed("Backspace");
                timer = 0;
            }
        }

        if(Input.GetKeyUp(KeyCode.Return))
        {
            KeyPressed("Enter");
        }
        if(Input.GetKeyUp(KeyCode.Backspace))
        {
            //KeyPressed("Backspace");
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            KeyPressed("Tab");
        }

        if (inputFieldSelected)
        {
            //allows the keyboard to be navigated if an input field is currently being selected
            StartKeyboardSelect(false);
            inputFieldSelected = false;
        }
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
        //bool kbRight = Input.GetKeyDown(KeyCode.RightArrow);
        //bool kbLeft = Input.GetKeyDown(KeyCode.LeftArrow);
        //bool kbDown = Input.GetKeyDown(KeyCode.DownArrow);
        //bool kbUp = Input.GetKeyDown(KeyCode.UpArrow);
        //bool kbReturn = Input.GetKeyDown(KeyCode.Return);

        // check if input passes threshold and reads as full press
        bool gpRight = stickX > threshold || dpadX > threshold;
        bool gpLeft = stickX < -threshold || dpadX < -threshold;
        bool gpUp = stickY > threshold || dpadY > threshold;
        bool gpDown = stickY < -threshold || dpadY < -threshold;
        bool gpEnter = Input.GetKeyDown(KeyCode.JoystickButton0);

        // Nav Controls
        //if (kbRight || (gpRight && !prevRight)) selectedIndex++;
        //else if (kbLeft || (gpLeft && !prevLeft)) selectedIndex--;
        //else if ((kbUp || (gpUp && !prevUp)) && selectedIndex != 40) selectedIndex -= rowSize;
        //else if ((kbUp || (gpUp && !prevUp)) && selectedIndex == 40) selectedIndex -= rowSize + 1;
        //else if (kbDown || (gpDown && !prevDown)) selectedIndex += rowSize;
        //else if (kbReturn || gpEnter) keyButtons[selectedIndex].onClick.Invoke();
        if ((gpRight && !prevRight)) selectedIndex++;
        else if ((gpLeft && !prevLeft)) selectedIndex--;
        else if (((gpUp && !prevUp)) && selectedIndex != 40) selectedIndex -= rowSize;
        else if (((gpUp && !prevUp)) && selectedIndex == 40) selectedIndex -= rowSize + 1;
        else if ((gpDown && !prevDown)) selectedIndex += rowSize;
        else if (gpEnter) keyButtons[selectedIndex].onClick.Invoke();

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
            colors.normalColor = (i == index) ? new Color(255, 255, 150, 52) : Color.white;
            keyButtons[i].colors = colors;
        }
    }

    /// <summary>
    /// Handles direct changes to the imput field
    /// </summary>
    /// <param name="value"></param>
    public void KeyPressed(string value)
    {
        curText = inputField[curField].text;
        // Backspace
        if (value == "Backspace")
        {
            // Making sure the text is not empty then taking the substring
            if (curText.Length > 0)
            {
                curText = curText.Substring(0, curText.Length - 1);
            }

        }

        else if (value == "Tab")
        {
            Selectable nextSelected = inputField[curField].navigation.selectOnUp;
            TMP_InputField nextInputField = nextSelected.GetComponent<TMP_InputField>();
            //print(nextSelected + ", " + nextInputField);
            if (curField > 0)
            {
                // If the text is empty and the current field is not the first one, go back to the previous field

                //if the next selected item is NOT in the tmp list
                if (nextSelected && !inputField.Contains(nextInputField))
                {
                    print("hi");
                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(nextSelected.gameObject);
                    gameObject.SetActive(false);
                    //curText = "";
                }
                else
                {
                    curField--;
                    curText = inputField[curField].text;

                    //Activating OnSelect triggers for inputfield
                    if (curField < inputField.Count)
                    {
                        inputField[curField].ActivateInputField();
                        inputField[curField].Select();
                    }
                }
            }
        }

        // Space will prob not be included
        else if (value == "SPACE")
        {
            curText += " ";
        }

        // Swapping input fields
        else if (value == "Enter")
        {
            print("hi");
            signInScript.SetPlayerLoginData(inputField[curField].name, curText);
            Selectable nextSelected = inputField[curField].navigation.selectOnDown;
            TMP_InputField nextInputField = nextSelected.GetComponent<TMP_InputField>();
            //print(inputField[curField]+ ": " + nextSelected + ", " + nextInputField);
            if (curField < inputField.Count - 1)
            {
                if (nextSelected && !inputField.Contains(nextInputField))
                {
                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(inputField[curField].navigation.selectOnDown.gameObject);
                    gameObject.SetActive(false);
                    //curText = "";
                }
                else
                {
                    curField++;
                    curText = inputField[curField].text;
                }
            }

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
        inputField[curField].text = curText;
   }

    /// <summary>
    /// input method for controllers to delete a character
    /// </summary>
    /// <param name="context"></param>
    public void OnBackSpace(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            KeyPressed("Backspace");
        }
    }

    /// <summary>
    /// hold input method for controllers to delete multiple characters
    /// </summary>
    /// <param name="context"></param>
    public void OnHoldBackspace(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            holdingBackspace = true;
        }

        if (context.canceled)
        {
            holdingBackspace = false;
        }
    }

    public void ResetCurrentFields()
    {
        ResetCurrentFields(0);
    }

    public void ResetCurrentFields(int startIndex)
    {
        if (gameObject.activeSelf)
        {
            Debug.Log("Resetting all input fields past " + startIndex);

            curText = "";
            for (int i = startIndex; i < inputField.Count; i++)
            {
                inputField[i].text = "";
            }
            curField = startIndex;
            inputField[curField].ActivateInputField();
            inputField[curField].Select();
        }
    }

    public void StartKeyboardSelect(bool reset)
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(keyButtons[0].gameObject);
        if (reset)
        {
            ResetCurrentFields();
        }
    }
    

}

