// Joshua Chisholm and Logan Larrondo
// 6/25/25
// Basic button functions for signing in

using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.InputSystem.DefaultInputActions;

public class SignInManager : MonoBehaviour
{

    // UI elements for different parts of sign in scene
    [SerializeField] GameObject loginUI;
    [SerializeField] GameObject signUpUI;
    [SerializeField] GameObject loginOptions;
    [SerializeField] VirtualKeyboardController keyboard;
    private GameManager gameManager;
    [SerializeField] private List<GameObject> continueButtons;
    private string logInOption;
    PlayerInfo playerInfo;
    APIManager apiManager;
    public GameObject emailError;
    public GameObject usernameError;
    public GameObject loginError;

    [SerializeField] private List<TMP_InputField> inputFieldsList = new List<TMP_InputField>();         // Holds all fields
    private Dictionary<string, TMP_InputField> inputFields = new Dictionary<string, TMP_InputField>();  // Organizes fields

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        apiManager = FindAnyObjectByType<APIManager>();

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
        foreach (TMP_InputField field in inputFieldsList)
        {
            inputFields[field.name] = field; 
        }
    }

    // Update is called once per frame
    void Update()
    {
             
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fieldName">input field name</param>
    /// <param name="data">data from user input</param>
    public async void SetPlayerLoginData(string fieldName, string data)
    {
        if (logInOption == "Login")
        {
            // Checking imput fields to assign correct player data
            switch (fieldName)
            {
                case "Username Login":
                    playerInfo.playerName = data;
                    break;

                case "Password Login":
                    playerInfo.playerPassword = data;
                    await apiManager.CheckPlayer(playerInfo);
                    return;

                default:
                    break;
            }
        }
        else if (logInOption == "SignUp")
        {
            // Checking imput fields to assign correct player data
            switch (fieldName)
            {
                case "Email Sign Up":
                    playerInfo.playerEmail = data;
                    break;

                case "Username Sign Up":
                    playerInfo.playerName = data;
                    break;

                case "Password Sign Up":
                    playerInfo.playerPassword = data;
                    break;

                case "Confirm Password":
                    if(data != playerInfo.playerPassword)
                    {
                        Debug.Log("Passwords do not match!");
                        keyboard.curField --;
                        return;
                    }
                    await apiManager.CreatePlayer(playerInfo);
                    return;

                default:
                    break;
            }
        }
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
        keyboard.inputField.Add(inputFields["Confirm Password"]);
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