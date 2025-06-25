// Joshua Chisholm
// 6/25/25
// Basic button functions for signing in

using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        
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
    }

    /// <summary>
    /// Sets sign in ui as active and disables login options
    /// </summary>
    public void ChoseSignUp()
    {
        loginOptions.SetActive(false);
        signUpUI.SetActive(true);
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
}
