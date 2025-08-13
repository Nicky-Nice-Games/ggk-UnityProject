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

    public void SetWwiseVolume(WwiseSlider slider)
    {
        float sliderValue = thisSlider.value;

        if(slider == WwiseSlider.Master)
        {
            masterVolume = thisSlider.value;
            AkUnitySoundEngine.SetRTPCValue("MasterVolume", masterVolume);
        }

        if(slider == WwiseSlider.Music)
        {
            musicVolume = thisSlider.value;
            AkUnitySoundEngine.SetRTPCValue("MusicVolume", musicVolume);
        }

        if(slider == WwiseSlider.SFX)
        {
            SFXVolume = thisSlider.value;
            AkUnitySoundEngine.SetRTPCValue("SFXVolume", SFXVolume);
        }

        if (slider == WwiseSlider.Dialogue)
        {
            dialogueVolume = thisSlider.value;
            AkUnitySoundEngine.SetRTPCValue("VoiceVolume", dialogueVolume);
        }
    }
}
