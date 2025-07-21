using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicStateManager : MonoBehaviour
{
    public static MusicStateManager instance;

    [SerializeField] AK.Wwise.Event startMusic;
    [SerializeField] AK.Wwise.State dormState;
    [SerializeField] AK.Wwise.State FBRState;
    [SerializeField] AK.Wwise.State menuState;
    [SerializeField] AK.Wwise.State noneState;
    [SerializeField] AK.Wwise.State outerLoopState;
    [SerializeField] AK.Wwise.State postRaceState;

    void Awake()
    {
        // only have one instance of this script at a time
        if (instance == null)
        {
            instance = this;

            // persists across scenes
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // destroy this script if one already exists
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //AkUnitySoundEngine.PostEvent("Music", gameObject);
        startMusic.Post(gameObject);
        SetMusicState(MusicState.Menu);
    }

    public void SetMusicState(MusicState newState)
    {
        if (newState == MusicState.Dorm)
        {
            dormState.SetValue();
        }
        else if (newState == MusicState.FBR)
        {
            FBRState.SetValue();
        }
        else if (newState == MusicState.Menu)
        {
            menuState.SetValue();
        }
        else if (newState == MusicState.None)
        {
            noneState.SetValue();
        }
        else if (newState == MusicState.OuterLoop)
        {
            outerLoopState.SetValue();
        }
        else if (newState == MusicState.PostRace)
        {
            postRaceState.SetValue();
        }
    }
}
