using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundTest : MonoBehaviour
{
    //This class is not intended as anything permanent.
    //It's just a way to test out Wwise functionality of some kind.
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            MusicStateManager.instance.ResetToLimbo();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            MusicLapStateManager.instance.SetLapState(LapState.Lap1);
            MusicResultsStateManager.instance.SetResultsState(ResultsState.InProgress);
            MusicStateManager.instance.SetMusicState(MusicState.OuterLoop);
        }

    }
}
