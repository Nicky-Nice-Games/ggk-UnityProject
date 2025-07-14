// Joshua Chisholm
// 6/25/25
// Basic button functions for signing in

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SignInManager : MonoBehaviour
{

    // UI elements for different parts of sign in scene
    [SerializeField]
    GameObject loginUI;
    [SerializeField]
    GameObject signUpUI;
    [SerializeField]
    GameObject loginOptions;
    [SerializeField]
    VirtualKeyboardController keyboard;

    [SerializeField] private List<TMP_InputField> inputFieldsList = new List<TMP_InputField>();         // Holds all fields
    private Dictionary<string, TMP_InputField> inputFields = new Dictionary<string, TMP_InputField>();  // Organizes fields

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        // Assigning the buttons their listeners
        foreach (GameObject obj in continueButtons)
        {
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            gameManager.GetComponent<ButtonBehavior>().OnClick());
            button.onClick.AddListener(() =>
            gameManager.GetComponentInChildren<GameManager>().LoggedIn());
        }

        // Getting Client ID / creating player info
        playerInfo = new PlayerInfo();
        gameManager.playerInfo = playerInfo;

        // Organizing fields list into dict
        foreach(TMP_InputField field in inputFieldsList)
        {
            inputFields[field.name] = field;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Sets login ui as active and disables login options
    /// </summary>
    public void ChoseLogin()
    {
        loginOptions.SetActive(false);
        loginUI.SetActive(true);
        logInOption = "Login";

        keyboard.inputField.Add(inputFields["Username Login"]);
        keyboard.inputField.Add(inputFields["Password Login"]);
    }

    /// <summary>
    /// Sets sign in ui as active and disables login options
    /// </summary>
    public void ChoseSignUp()
    {
        loginOptions.SetActive(false);
        signUpUI.SetActive(true);
        logInOption = "SignUp";

        keyboard.inputField.Add(inputFields["Email Sign Up"]);
        keyboard.inputField.Add(inputFields["Username Sign Up"]);
        keyboard.inputField.Add(inputFields["Password Sign Up"]);
    }

    /// <summary>
    /// Disables login or signup if they are active and enables login options
    /// </summary>
    public void BackToSignInOptions()
    {
        loginOptions.SetActive(true);
        signUpUI.SetActive(false);
        loginUI.SetActive(false);
    }

    /// <summary>
    /// Brings user to start scene as a guest
    /// 
    /// NOTE:
    /// 
    /// It should be the multisingle scene, but I don't know
    /// how GameManager works so this is temporary.
    /// </summary>
    public void ContinueAsGuest()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void DisplayKeyboard(GameObject sender) 
    {
        TMP_InputField input = sender.GetComponent<TMP_InputField>();
        keyboard.gameObject.SetActive(true);
    }

    public void HideKeyboard()
    {
        keyboard.gameObject.SetActive(false);
    }
}
