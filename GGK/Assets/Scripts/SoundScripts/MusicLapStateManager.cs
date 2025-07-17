using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLapStateManager : MonoBehaviour
{
    public static MusicLapStateManager instance;

    [SerializeField] AK.Wwise.State lap1State;
    [SerializeField] AK.Wwise.State lap2State;
    [SerializeField] AK.Wwise.State lap3State;
    [SerializeField] AK.Wwise.State noneState;

    public enum LapState
    {
        Lap1,
        Lap2,
        Lap3,
        None
    }

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

    public void SetLapState(LapState newState)
    {
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
        }
    }
}
