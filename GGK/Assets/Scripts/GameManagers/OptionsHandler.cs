using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening.Core.Easing;
using UnityEngine.EventSystems;

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

    public GameObject closeBtn;
    private Vector3 initialScale;

    public Toggle fullScreenToggle;

    public TMP_InputField widthInputField;
    public TMP_InputField heightInputField;

    private bool initalizing;

    /// <summary>
    /// This is so when this panel closes, the button selected goes to this button set
    /// </summary>
    [Header("ButtonNavigation Settings")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject buttonOut;

    private void Update()
    {
        // Updates resolution text fields when resolution changes
        if (optionsData.resolution.x != Screen.width || optionsData.resolution.y != Screen.height)
        {
            optionsData.resolution.x = Screen.width;
            optionsData.resolution.y = Screen.height;
            widthInputField.text = optionsData.resolution.x.ToString();
            heightInputField.text = optionsData.resolution.y.ToString();
        }
    }

    void OnEnable()
    {
        if (initialScale == Vector3.zero)
        {
            initialScale = closeBtn.transform.localScale;
        }
        else
        {
            closeBtn.transform.localScale = initialScale;
        }
        SetOptions();
    }

    // Initialize saved values from OptionsData
    public void SetOptions()
    {
        initalizing = true;

        masterVolumeSlider.value = optionsData.masterVolume;
        masterVolumeValue.text = optionsData.masterVolume.ToString();

        dialougeVolumeSlider.value = optionsData.dialougeVolume;
        dialougeVolumeValue.text = optionsData.dialougeVolume.ToString();

        sfxVolumeSlider.value = optionsData.sfxVolume;
        sfxVolumeValue.text = optionsData.sfxVolume.ToString();

        musicVolumeSlider.value = optionsData.musicVolume;
        musicVolumeValue.text = optionsData.musicVolume.ToString();

        // Set checkmark on or off if full screen or not
        if (optionsData.IsFullScreen)
        {
            fullScreenToggle.isOn = true;
        }
        else
        {
            fullScreenToggle.isOn = false;
        }

        // Grab resolution of game window and update resolution text fields
        optionsData.resolution.x = Screen.width;
        optionsData.resolution.y = Screen.height;
        widthInputField.text = optionsData.resolution.x.ToString();
        heightInputField.text = optionsData.resolution.y.ToString();

        initalizing = false;
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
    public void FullScreenChange()
    {
        if (initalizing) return;

        if (optionsData.IsFullScreen)
        {
            optionsData.IsFullScreen = false;
            Screen.SetResolution((int)optionsData.resolution.x, (int)optionsData.resolution.y, optionsData.IsFullScreen);
        }
        else
        {
            optionsData.IsFullScreen = true;
            Screen.SetResolution((int)optionsData.resolution.x, (int)optionsData.resolution.y, optionsData.IsFullScreen);
        }
    }
    public void ResolutionWidthChange()
    {
        int width = int.Parse(widthInputField.text);
        if (width < 320)
        {
            width = 320;
        }

        Screen.SetResolution(width, (int)optionsData.resolution.y, optionsData.IsFullScreen);
    }
    public void ResolutionHeightChange()
    {
        int height = int.Parse(heightInputField.text);
        if (height < 480)
        {
            height = 480;
        }

        Screen.SetResolution((int)optionsData.resolution.x, height, optionsData.IsFullScreen);
    }

    // Close Options
    public void Close()
    {
        // When this panel closes this is the first button to be hovered outside of the panel
        eventSystem.SetSelectedGameObject(buttonOut);
        gameObject.SetActive(false);
        optionsData.GameManager.RefreshSelected();
    }
}
