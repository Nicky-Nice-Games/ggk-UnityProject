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

    // Start is called before the first frame update
    void Start()
    {
        gamemanagerObj = FindAnyObjectByType<GameManager>();

        // Assigning the buttons their listeners
        foreach (GameObject obj in mapOptions)
        {
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            gamemanagerObj.GetComponent<ButtonBehavior>().OnClick());
            button.onClick.AddListener(() =>
            gamemanagerObj.GetComponent<GameManager>().MapSelected());
        }

        continueButton.onClick.AddListener(() => SelectMap());
    }

    private void DisableButtons()
    {
        foreach(GameObject obj in mapOptions)
        {
            obj.GetComponent<Button>().enabled = false;
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

    public void SelectMap()
    {
        gamemanagerObj.MapSelected(characterData.mapSelected);
        continueButton.interactable = false;
        Debug.Log(characterData.mapSelected);
    }
}
