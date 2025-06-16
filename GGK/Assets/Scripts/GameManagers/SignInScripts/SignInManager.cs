using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Diagnostics;

public class SignInManager : MonoBehaviour
{
    //Bool to check if the game is being run on an arcade machine or not
    bool isArcade = false;

    //Used to hide things on the scene
    [SerializeField] GameObject hider;
    [SerializeField] GameObject shower;
    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public void Login()
    {
        if (isArcade)
        {
            UnityEngine.Debug.Log("This is an Arcade Machine");
            SceneManager.LoadScene("ArcadeLogin");
        }
        else
        {
            UnityEngine.Debug.Log("This is not an Arcade Machine");
            SceneManager.LoadScene("Login");
        }
    }

    public void SignUp()
    {
        if (isArcade)
        {
            UnityEngine.Debug.Log("This is an Arcade Machine");
            SceneManager.LoadScene("ArcadeSignUp");
        }
        else
        {
            UnityEngine.Debug.Log("This is not an Arcade Machine");
            SceneManager.LoadScene("SignUpScene");
        }
        
    }

    //This method is used to hide some of the UI to focus on another part of it
    public void HideandShow()
    {
        hider.SetActive(false);
        shower.SetActive(true);
    }
}
