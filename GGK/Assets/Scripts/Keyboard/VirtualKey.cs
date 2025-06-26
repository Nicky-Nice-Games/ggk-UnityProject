using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualKey : MonoBehaviour
{
    public string keyValue;
    private VirtualKeyboardController keyboardController;

    // Start is called before the first frame update
    void Start()
    {
        keyboardController = FindAnyObjectByType<VirtualKeyboardController>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        keyboardController.KeyPressed(keyValue);
    }
}
