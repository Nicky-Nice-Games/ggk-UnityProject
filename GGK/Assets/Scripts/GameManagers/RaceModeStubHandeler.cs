using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

public class RaceModeStubHandeler : MonoBehaviour
{
    [SerializeField]
    private GameObject menuOption;
    private GameManager gamemanagerObj;

    // Start is called before the first frame update
    void Start()
    {
        gamemanagerObj = FindAnyObjectByType<GameManager>();
       
        Button button = menuOption.GetComponent<Button>();
        button.onClick.AddListener(() =>
        gamemanagerObj.GetComponent<ButtonBehavior>().OnClick());
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
}
