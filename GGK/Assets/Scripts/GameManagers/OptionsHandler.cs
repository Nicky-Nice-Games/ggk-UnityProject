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

    public Slider volumeSlider;
    public TextMeshProUGUI volumeValue;

    void OnEnable()
    {
        SetOptions();
    }

    // Initialize saved values from OptionsData
    public void SetOptions()
    {
        volumeSlider.value = optionsData.volume;
        volumeValue.text = optionsData.volume.ToString();
    }

    // Methods to attach to options panel interactables
    public void VolumeChange()
    {
        optionsData.volume = (int)volumeSlider.value;
        volumeValue.text = optionsData.volume.ToString();
    }

    // Close Options
    public void Close()
    {
        gameObject.SetActive(false);
        optionsData.GameManager.RefreshSelected();
    }
}
