using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WwiseVolumeSliders : MonoBehaviour
{
    [SerializeField] Slider thisSlider;
    [SerializeField] float masterVolume;
    [SerializeField] float musicVolume;
    [SerializeField] float SFXVolume;
    [SerializeField] float dialogueVolume;

    public void SetMasterVolume()
    {
        masterVolume = thisSlider.value;
        AkUnitySoundEngine.SetRTPCValue("MasterVolume", masterVolume);
    }

    public void SetMusicVolume()
    {
        musicVolume = thisSlider.value;
        AkUnitySoundEngine.SetRTPCValue("MusicVolume", musicVolume);
    }

    public void SetSFXVolume()
    {
        SFXVolume = thisSlider.value;
        AkUnitySoundEngine.SetRTPCValue("SFXVolume", SFXVolume);
    }

    public void SetDialogueVolume()
    {
        dialogueVolume = thisSlider.value;
        AkUnitySoundEngine.SetRTPCValue("VoiceVolume", dialogueVolume);
    }
}
