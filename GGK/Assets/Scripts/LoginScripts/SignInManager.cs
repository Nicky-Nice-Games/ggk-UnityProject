// Joshua Chisholm
// 6/25/25
// Basic button functions for signing in

using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.InputSystem.DefaultInputActions;

public class SignInManager : NetworkBehaviour
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
    private GameManager gameManager;
    [SerializeField] private List<GameObject> continueButtons;
    private string logInOption;
    PlayerInfo playerInfo;

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
        playerInfo = new PlayerInfo(NetworkManager.LocalClientId);
        gameManager.playerList.Add(playerInfo);
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
    public void SetPlayerLoginData(string fieldName, string data)
    {
        if(logInOption == "Login")
        {
            // Checking imput fields to assign correct player data
            switch (fieldName)
            {
                case "Username Login":
                    playerInfo.playerName = data;
                    break;

                case "Password Login":
                    playerInfo.playerPassword = data;
                    break;

                default:
                    break;
            }
        }
        else if(logInOption == "SignUp")
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
                    // TODO: Add functionality to validate password
                    break;

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
    }

    /// <summary>
    /// Sets sign in ui as active and disables login options
    /// </summary>
    public void ChoseSignUp()
    {
        loginOptions.SetActive(false);
        signUpUI.SetActive(true);
        logInOption = "SignUp";
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
        keyboard.inputField = input;
    }

    public void HideKeyboard()
    {
        keyboard.gameObject.SetActive(false);
    }
}
