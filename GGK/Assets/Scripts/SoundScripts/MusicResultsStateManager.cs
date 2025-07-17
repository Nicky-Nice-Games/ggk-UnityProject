using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicResultsStateManager : MonoBehaviour
{
    public static MusicResultsStateManager instance;

    [SerializeField] AK.Wwise.State inProgressState;
    [SerializeField] AK.Wwise.State LossState;
    [SerializeField] AK.Wwise.State noneState;
    [SerializeField] AK.Wwise.State startingState;
    [SerializeField] AK.Wwise.State winState;

    public enum ResultsState
    {
        InProgress,
        Loss,
        None,
        Starting,
        Win
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

    public void SetResultsState(ResultsState newState)
    {
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
