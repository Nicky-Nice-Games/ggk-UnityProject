using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SignInManager : MonoBehaviour
{
    //Bool to check if the game is being run on an arcade machine or not
    bool isArcade = false;
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

    }

    public void SignUp()
    {
        if (isArcade)
        {
         Debug.Log("This is an Arcade Machine");
        }
        else
        {
            Debug.Log("This is not an Arcade Machine");
            SceneManager.LoadScene("SignUpScene");
        }
        
    }
}
