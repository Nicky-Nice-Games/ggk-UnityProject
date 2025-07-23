//Daniel Wright
//7/22/25
//The purpose of this script is to encrypt the characters being typed into the login password once show password is toggled

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PasswordHider : MonoBehaviour
{
    [SerializeField] Toggle showPassword;
    [SerializeField] TMP_InputField passwordInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //When show password is toggled on, the content type is set to standard(the characters are visible)
        //When it is toggled off, the content type becomes password(makes the characters "*")
        if (showPassword.isOn)
        {
            passwordInput.contentType = TMP_InputField.ContentType.Standard;
            passwordInput.ForceLabelUpdate();
        }
        else
        {
            passwordInput.contentType = TMP_InputField.ContentType.Password;
            passwordInput.ForceLabelUpdate();
        }

    }
}
