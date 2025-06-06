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

    void Start()
    {
        gameManager = gameObject.GetComponent<GameManager>();
    }

    public void VolumeChange(float value)
    {
        gameManager.volume = (int)value;
        volumeValue.text = gameManager.volume.ToString();
    }
}
