using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Adjusts the Wwise music state.
/// </summary>
public class MusicStateManager : MonoBehaviour
{
    public static MusicStateManager instance;

    //A Wwise event that starts the music at the beginning of the game.
    [Header("Wwise Music event")]
    [SerializeField] AK.Wwise.Event startMusic;

    //These are the music states used with Wwise.
    [Header("Wwise Music States")]
    [SerializeField] AK.Wwise.State dormState;
    [SerializeField] AK.Wwise.State FBRState;
    [SerializeField] AK.Wwise.State golisanoState;
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
        //The Music event gets posted at the very start of the game.
        //This event keeps going perpetually until the game is closed.
        startMusic.Post(gameObject);

        //Changes the music state to the menu, as it's where the game starts.
        SetMusicState(MusicState.Menu);
        //Both of these are set to None since they aren't relevant at the moment.
        MusicLapStateManager.instance.SetLapState(LapState.None);
        MusicResultsStateManager.instance.SetResultsState(ResultsState.None);
    }

    /// <summary>
    /// Changes the Wwise music state to the value of the enum passed in.
    /// </summary>
    /// <param name="newState"></param>
    public void SetMusicState(MusicState newState)
    {
        //If newState matches one of these values,
        //we set the respective state value for the music.
        if (newState == MusicState.Dorm)
        {
            dormState.SetValue();
        }
        else if (newState == MusicState.FBR)
        {
            FBRState.SetValue();
        }
        else if (newState == MusicState.Golisano)
        {
            golisanoState.SetValue();
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

    /// <summary>
    /// Sets the Music state, lap state, and results state to None in Wwise.
    /// </summary>
    public void ResetToLimbo()
    {
        Debug.Log("Limbo");
        SetMusicState(MusicState.None);
        MusicLapStateManager.instance.SetLapState(LapState.None);
        MusicResultsStateManager.instance.SetResultsState(ResultsState.None);
    }
}
