using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicResultsStateManager : MonoBehaviour
{
    public static MusicResultsStateManager instance;

    [Header("Wwise Results States")]
    [SerializeField] AK.Wwise.State inProgressState;
    [SerializeField] AK.Wwise.State LossState;
    [SerializeField] AK.Wwise.State noneState;
    [SerializeField] AK.Wwise.State startingState;
    [SerializeField] AK.Wwise.State winState;

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
    /// Changes the Wwise results state to the value of the enum passed in.
    /// </summary>
    /// <param name="newState"></param>
    public void SetResultsState(ResultsState newState)
    {
        //If newState matches one of these values,
        //we set the respective state value for the music.
        if (newState == ResultsState.InProgress)
        {
            inProgressState.SetValue();
        }
        else if (newState == ResultsState.Loss)
        {
            LossState.SetValue();
        }
        else if (newState == ResultsState.None)
        {
            noneState.SetValue();
        }
        else if (newState == ResultsState.Starting)
        {
            startingState.SetValue();
        }
        else if (newState == ResultsState.Win)
        {
            winState.SetValue();
        }
    }
}
