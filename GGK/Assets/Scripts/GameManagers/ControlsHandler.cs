using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ControlsHandler : MonoBehaviour
{
    public TextMeshProUGUI title;

    public Image keyboardInputs;
    public Image controllerInputs;

    /// <summary>
    /// This is so when this panel opens, the button selected goes to this button set
    /// </summary>
    [Header("ButtonNavigation Settings")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject buttonIn;
    [SerializeField] private GameObject buttonOut;
    public void OnEnable()
    {
        Keyboard();
        // When this panel opens this is the first button to be hovered
        eventSystem.SetSelectedGameObject(buttonIn); 
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        // When this panel closes this is the first button to be hovered outside of the panel
        eventSystem.SetSelectedGameObject(buttonOut);
        gameObject.SetActive(false);
    }

    public void Keyboard()
    {
        keyboardInputs.enabled = true;
        controllerInputs.enabled = false;

        title.text = "Keyboard Controls";
    }

    public void Controller()
    {
        keyboardInputs.enabled = false;
        controllerInputs.enabled = true;

        title.text = "Controller Controls";
    }
}
