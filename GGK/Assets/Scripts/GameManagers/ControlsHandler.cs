using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlsHandler : MonoBehaviour
{
    public TextMeshProUGUI title;

    public Image keyboardInputs;
    public Image controllerInputs;

    public void OnEnable()
    {
        Keyboard();
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
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
