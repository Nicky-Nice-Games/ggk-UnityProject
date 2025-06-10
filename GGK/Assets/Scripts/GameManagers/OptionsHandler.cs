using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsHandler : MonoBehaviour
{
    // References
    public GameManager gameManager;
    public Slider volumeSlider;
    public TextMeshProUGUI volumeValue;

    public void VolumeChange()
    {
        gameManager.volume = (int)volumeSlider.value;
        volumeValue.text = gameManager.volume.ToString();
    }
}
