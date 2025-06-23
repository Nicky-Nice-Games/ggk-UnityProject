using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening.Core.Easing;

public class OptionsHandler : MonoBehaviour
{
    // References
    public OptionsData optionsData;

    public Slider masterVolumeSlider;
    public TextMeshProUGUI masterVolumeValue;

    public Slider dialougeVolumeSlider;
    public TextMeshProUGUI dialougeVolumeValue;

    public Slider sfxVolumeSlider;
    public TextMeshProUGUI sfxVolumeValue;

    public Slider musicVolumeSlider;
    public TextMeshProUGUI musicVolumeValue;

    void OnEnable()
    {
        SetOptions();
    }

    // Initialize saved values from OptionsData
    public void SetOptions()
    {
        masterVolumeSlider.value = optionsData.masterVolume;
        masterVolumeValue.text = optionsData.masterVolume.ToString();

        dialougeVolumeSlider.value = optionsData.dialougeVolume;
        dialougeVolumeValue.text = optionsData.dialougeVolume.ToString();

        sfxVolumeSlider.value = optionsData.sfxVolume;
        sfxVolumeValue.text = optionsData.sfxVolume.ToString();

        musicVolumeSlider.value = optionsData.musicVolume;
        musicVolumeValue.text = optionsData.musicVolume.ToString();
    }

    // Methods to attach to options panel interactables
    public void MasterVolumeChange()
    {
        optionsData.masterVolume = (int)masterVolumeSlider.value;
        masterVolumeValue.text = optionsData.masterVolume.ToString();
    }
    public void DialougeVolumeChange()
    {
        optionsData.dialougeVolume = (int)dialougeVolumeSlider.value;
        dialougeVolumeValue.text = optionsData.dialougeVolume.ToString();
    }
    public void SFXVolumeChange()
    {
        optionsData.sfxVolume = (int)sfxVolumeSlider.value;
        sfxVolumeValue.text = optionsData.sfxVolume.ToString();
    }
    public void MusicVolumeChange()
    {
        optionsData.musicVolume = (int)musicVolumeSlider.value;
        musicVolumeValue.text = optionsData.musicVolume.ToString();
    }

    // Close Options
    public void Close()
    {
        gameObject.SetActive(false);
        optionsData.GameManager.RefreshSelected();
    }
}
