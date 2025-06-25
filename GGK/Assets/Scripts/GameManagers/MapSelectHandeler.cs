using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

/// <summary>
/// Map Enum by Phillip Brown
/// Used to keep track of map votes for multiplayer
/// </summary>
public enum Map
{
    RITOuterLoop,
    RITQuarterMile,
    RITDorm,
    FinalsBrickRoad,
    Golisano,
    RITWoods,
}

public class MapSelectHandeler : MonoBehaviour
{
    [SerializeField] private List<GameObject> mapOptions = new List<GameObject>();
    private GameManager gamemanagerObj;
    // [SerializeField] private Transform waitingScreen;
    // [SerializeField] private Transform mapButtons;


    // Start is called before the first frame update
    void Start()
    {
        gamemanagerObj = FindAnyObjectByType<GameManager>();
        // waitingScreen.gameObject.SetActive(false);
        // mapButtons.gameObject.SetActive(true);

        // Assigning the buttons their listeners
        foreach (GameObject obj in mapOptions)
        {
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            gamemanagerObj.GetComponent<ButtonBehavior>().OnClick());
            button.onClick.AddListener(() =>
            gamemanagerObj.GetComponent<GameManager>().MapSelected());
        }
    }

    public void SwitchToWaitingScreen()
    {
        // mapButtons.gameObject.SetActive(false);
        // waitingScreen.gameObject.SetActive(true);
    }
    public void SwitchToMapPanel()
    {
        // mapButtons.gameObject.SetActive(true);
        // waitingScreen.gameObject.SetActive(false);
    }


    // Each button can ready its own map
}
