using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adjusts the Wwise lap state.
/// </summary>
public class MusicLapStateManager : MonoBehaviour
{
    public static MusicLapStateManager instance;

    [Header("Wwise Lap States")]
    [SerializeField] AK.Wwise.State lap1State;
    [SerializeField] AK.Wwise.State lap2State;
    [SerializeField] AK.Wwise.State lap3State;
    [SerializeField] AK.Wwise.State noneState;

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

    /// <summary>
    /// Changes the Wwise lap state to the value of the enum passed in.
    /// </summary>
    /// <param name="newState"></param>
    public void SetLapState(LapState newState)
    {
        //If newState matches one of these values,
        //we set the respective state value for the music.
        if (newState == LapState.Lap1)
        {
            lap1State.SetValue();
        }
        else if (newState == LapState.Lap2)
        {
            lap2State.SetValue();
        }
        else if (newState == LapState.Lap3)
        {
            lap3State.SetValue();
        }
        else if (newState == LapState.None)
        {
            noneState.SetValue();
            Debug.Log("Lap is none");
        }
    }
}
