using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class StartSceneHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Button startButton;

    [SerializeField]
    private Button quitButton;
    
    private GameManager gamemanagerObj;

    private void Awake()
    {
        AkBankManager.UnloadAllBanks();
        AkBankManager.LoadBank("SFX", false, false);
        AkBankManager.LoadBank("Music", false, false);
    }

    void Start()
    {
        gamemanagerObj = FindObjectOfType<GameManager>();
        startButton.onClick.AddListener(() => gamemanagerObj.StartGame());
        startButton.onClick.AddListener(() => gamemanagerObj.GetComponent<ButtonBehavior>().OnClick());
        quitButton.onClick.AddListener(() => gamemanagerObj.ExitGame());

        // listens for any button press on the start screen
        InputSystem.onAnyButtonPress.CallOnce(OnAnyButton);
        
        if (MusicStateManager.instance != null)
        {
            MusicResultsStateManager.instance.SetResultsState(ResultsState.None);
            MusicLapStateManager.instance.SetLapState(LapState.None);
            MusicStateManager.instance.SetMusicState(MusicState.Menu);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// takes in which button is pressed then starts game
    /// </summary>
    /// <param name="control">which button was pressed</param>
    private void OnAnyButton(InputControl control)
    {
        Debug.Log("Pressed: " + control.name);

        gamemanagerObj.StartGame();
    }
}
