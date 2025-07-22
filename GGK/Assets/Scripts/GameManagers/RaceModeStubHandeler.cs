using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

public class RaceModeStubHandeler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> menuOptions = new List<GameObject>();
    private GameManager gamemanagerObj;

    // Start is called before the first frame update
    void Start()
    {
        gamemanagerObj = FindAnyObjectByType<GameManager>();

        // Assigning the buttons their listeners
        foreach (GameObject obj in menuOptions)
        {
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            gamemanagerObj.GetComponent<ButtonBehavior>().OnClick());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnToModeSelectButton()
    {
        gamemanagerObj.sceneLoader.LoadScene("GameModeSelectScene");
        gamemanagerObj.curState = GameStates.gameMode;
    }

    public void ReturnToMMButton()
    {
        gamemanagerObj.LoadStartMenu();
    }
}
