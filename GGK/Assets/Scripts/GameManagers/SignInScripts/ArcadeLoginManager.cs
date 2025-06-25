using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArcadeLoginManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameManager gameManager;

    private string uIState;
    private string username;
    private string password;
    //Game ibjects constianging UI fir different states of the Scene
    public GameObject startUI;
    public GameObject enterInfoUI;
    public GameObject swipeUI;

    //Bool to check if the arcade machine is accepting swipe input
    public bool acceptingSwipe = false;


    public TMP_InputField hiddenInput;
    // Displays only first group of UI elements when the scene gets loaded
    void Start()
    {
       startUI.SetActive(true);
        enterInfoUI.SetActive(false);
        swipeUI.SetActive(false);
    }

    /// <summary>
    /// If the accepting swipe bool is true, then it checks if the hidden input field ends with a question mark.
    /// If it does then then parse the input and stops the swipe mode.
    /// </summary>
    void Update()
    {
        if (acceptingSwipe){

        if (hiddenInput.text.EndsWith("?"))
        {
                ParseSwipeData(hiddenInput.text);
            }
            else
            {
                hiddenInput.text = "";
            }
        }
    }
    // Change UI state
    public void LoadInfoState()
    {
        
        startUI.SetActive(false);
        swipeUI.SetActive(false);
        enterInfoUI.SetActive(true);
    }
    // These next 2 methouds are used to read the username and password input from the UI
    public void ReadUsernameInput(string s) {
        username = s;
    }
    public void ReadUPasswordInput(string s)
    {
        password = s;
    }

    public void UsernameLogin()
    {
        // Here you would insert code to verify and pasasing username and password with the server
        // then save neccesasary data to the game server
        gameManager.LoadMultiSinglePlay();

    }

    public void SwipeLogin()
    {
        // Here you would insert code to verify and pasasing username and password onto the 
        // then save neccesasary data to the game server
        gameManager.LoadMultiSinglePlay();
    }
    public void AddSwipe()
    {
        // Here you would insert code to send swipe information to the

    }
    public void LoadSwipeState()
    {
        enterInfoUI.SetActive(false);
        startUI.SetActive(false);
        swipeUI.SetActive(true);
    }
    /// <summary>
    /// Allows for swipe input into the hiddend input filed. It also clears the field too
    /// </summary>
    private void StartSwipeMode()
    {
        acceptingSwipe = true;
        hiddenInput.text = "";  // Clear old input
        hiddenInput.ActivateInputField(); // Focus it
        EventSystem.current.SetSelectedGameObject(hiddenInput.gameObject);
        
        // for testing
        gameManager.LoadMultiSinglePlay();

    }
    /// <summary>
    /// Stops the game from accepting swie input
    /// </summary>y way to enter swipe input
    private void StopSwipeMode()
    {
        acceptingSwipe = false;
        hiddenInput.DeactivateInputField();
        EventSystem.current.SetSelectedGameObject(null);
    }
    /// <summary>
    /// This Method takes in the input and from the swpie, parses it, and makes sure its valid 
    /// </summary>
    /// <param name="data"></param>
    private void ParseSwipeData(string data)

    {
        StopSwipeMode(); // Stop accepting swipe input
        if (!data.StartsWith(";") || !data.Contains("="))
        {
            // you would send a message like swipe again to the scene
            StartSwipeMode(); // Restart swipe mode if invalid
        }

        int equalsIndex = data.IndexOf('=');
        string pan = data.Substring(1, equalsIndex - 1); // Exclude the ';'

        if (pan.Length == 9 && pan.All(char.IsDigit))// checks if Dada is valid
        {
              // Here if valid you would send UID back to the. 
        }
        else
        {
            StartSwipeMode(); // Restart swipe mode if invalid
        }
    }
}
