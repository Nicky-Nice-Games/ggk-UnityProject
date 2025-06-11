using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerKartHandeler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> playerOptions = new List<GameObject>();
    private GameManager gamemanagerObj;


    // Start is called before the first frame update
    void Start()
    {
        gamemanagerObj = FindAnyObjectByType<GameManager>();

        // Assigning the buttons their listeners
        foreach (GameObject obj in playerOptions)
        {
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            gamemanagerObj.GetComponent<ButtonBehavior>().OnClick());
            button.onClick.AddListener(() =>
            gamemanagerObj.GetComponent<GameManager>().PlayerSelected());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Each button can assign the unique player 
}
