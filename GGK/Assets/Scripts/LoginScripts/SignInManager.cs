using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SignInManager : MonoBehaviour
{
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

    public void ChoseLogin()
    {
        loginOptions.SetActive(false);
        loginUI.SetActive(true);
    }
    public void ChoseSignUp()
    {
        loginOptions.SetActive(false);
        signUpUI.SetActive(true);
    }

    public void BackToSignInOptions()
    {
        loginOptions.SetActive(true);
        signUpUI.SetActive(false);
        loginUI.SetActive(false);
    }

    public void ContinueAsGuest()
    {
        SceneManager.LoadScene("StartScene");
    }
}
