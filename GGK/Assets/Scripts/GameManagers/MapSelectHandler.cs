using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectHandler : MonoBehaviour
{
    public GameManager gameManager;
    public List<GameObject> trackButtons;

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        foreach (GameObject trackButton in trackButtons)
        {
            Button btn = trackButton.GetComponentInChildren<Button>();

            btn.onClick.AddListener(() => MapSelected(trackButton.GetComponentInChildren<TextMeshProUGUI>().text));
        }
    }

    public void MapSelected(string trackName)
    {
        gameManager.MapSelected(trackName);
    }

    // Temporary
    public void SkipToEnd()
    {
        gameManager.GameFinished();
    }
}
