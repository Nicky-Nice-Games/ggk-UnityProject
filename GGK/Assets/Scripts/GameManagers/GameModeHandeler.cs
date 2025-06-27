using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

public class GameModeHandeler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> modeOptions = new List<GameObject>();
    private GameManager gamemanagerObj;

    // Start is called before the first frame update
    void Start()
    {
        gamemanagerObj = FindAnyObjectByType<GameManager>();

        // Assigning the buttons their listeners
        foreach (GameObject obj in modeOptions)
        {
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            gamemanagerObj.GetComponent<ButtonBehavior>().OnClick());
            // checks for each mode's button
            // likely to change when Grand Prix & Time Trial are actually implemented
            if(obj.name == "Race")
            {
                button.onClick.AddListener(() =>
                gamemanagerObj.GetComponent<GameManager>().LoadedGameMode());
            }
            else if(obj.name == "Time Trial")
            {
                button.onClick.AddListener(() =>
                gamemanagerObj.sceneLoader.LoadScene("TimeTrialStub"));
            }
            else if(obj.name == "Grand Prix")
            {
                button.onClick.AddListener(() =>
                gamemanagerObj.sceneLoader.LoadScene("GrandPrixStub"));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Each game mode can load their own stuff
}
