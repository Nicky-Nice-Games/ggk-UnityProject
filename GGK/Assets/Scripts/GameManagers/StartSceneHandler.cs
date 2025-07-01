using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Button startButton;

    [SerializeField]
    private Button quitButton;
    
    private GameManager gamemanagerObj;

    void Start()
    {
        gamemanagerObj = FindObjectOfType<GameManager>();
        startButton.onClick.AddListener(() => gamemanagerObj.StartGame());
        startButton.onClick.AddListener(() => gamemanagerObj.GetComponent<ButtonBehavior>().OnClick());
        quitButton.onClick.AddListener(() => gamemanagerObj.ExitGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
