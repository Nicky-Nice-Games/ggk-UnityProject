using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundVolume : MonoBehaviour
{
    // Reference
    public OptionsData optionsData;

    // Volume Methods
    public void PlayDialouge(AudioSource audioSource, AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip, (optionsData.masterVolume / 100) * (optionsData.dialougeVolume / 100));
    }

    public void PlaySFX(AudioSource audioSource, AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip, (optionsData.masterVolume / 100) * (optionsData.sfxVolume / 100));
    }

    public void PlayMusic(AudioSource audioSource, AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip, (optionsData.masterVolume / 100) * (optionsData.musicVolume / 100));
    }
}
