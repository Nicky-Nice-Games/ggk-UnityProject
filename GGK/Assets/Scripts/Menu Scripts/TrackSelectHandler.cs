using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrackSelectHandler : MonoBehaviour
{
    // References
    public GameManager gameManager;
    public List<GameObject> trackButtons;

    void Start()
    {
        gameManager = GameObject.FindAnyObjectByType<GameManager>();

        foreach (GameObject trackButton in trackButtons)
        {
            Button btn = trackButton.GetComponentInChildren<Button>();

            btn.onClick.AddListener(() => LoadTrack(trackButton.GetComponentInChildren<TextMeshProUGUI>().text));
        }
    }

    // Taking the relevant buttons from Game Manager
    public void LoadTrack(string trackName)
    {
        gameManager.LoadTrack(trackName);
    }
}
