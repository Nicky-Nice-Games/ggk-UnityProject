using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundVolume : MonoBehaviour
{
    // Reference
    public OptionsData optionsData;

    // Volume Methods
    public void PlayDialouge(AudioSource audioSource)
    {
        audioSource.volume = (optionsData.masterVolume / 100) * (optionsData.dialougeVolume / 100);
        audioSource.Play();
    }
    public void PlayDialougeOneShot(AudioSource audioSource, AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip, (optionsData.masterVolume / 100) * (optionsData.dialougeVolume / 100));
    }
    public void ChangeDialougeVolume(AudioSource audioSource)
    {
        audioSource.volume = (optionsData.masterVolume / 100) * (optionsData.dialougeVolume / 100);
    }
    public void PlaySFX(AudioSource audioSource)
    {
        audioSource.volume = (optionsData.masterVolume / 100) * (optionsData.sfxVolume / 100);
        audioSource.Play();
    }
    public void PlaySFXOneShot(AudioSource audioSource, AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip, (optionsData.masterVolume / 100) * (optionsData.sfxVolume / 100));
    }
    public void ChangeSFXVolume(AudioSource audioSource)
    {
        audioSource.volume = (optionsData.masterVolume / 100) * (optionsData.sfxVolume / 100);
    }
    public void PlayMusic(AudioSource audioSource)
    {
        audioSource.volume = (optionsData.masterVolume / 100) * (optionsData.musicVolume / 100);
        audioSource.Play();
    }
    public void PlayMusicOneShot(AudioSource audioSource, AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip, (optionsData.masterVolume / 100) * (optionsData.musicVolume / 100));
    }
    public void ChangeMusicVolume(AudioSource audioSource)
    {
        audioSource.volume = (optionsData.masterVolume / 100) * (optionsData.musicVolume / 100);
    }
}
