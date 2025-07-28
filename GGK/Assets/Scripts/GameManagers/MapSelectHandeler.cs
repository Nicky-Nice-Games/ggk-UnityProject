using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

public class MapSelectHandeler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> mapOptions = new List<GameObject>();
    private GameManager gamemanagerObj;
    private CharacterData characterData;

    [SerializeField]
    private Button continueButton;

    // Start is called before the first frame update
    void Start()
    {
        gamemanagerObj = FindAnyObjectByType<GameManager>();
        characterData = FindAnyObjectByType<CharacterData>();

        // Assigning the buttons their listeners
        foreach (GameObject obj in mapOptions)
        {
            Button button = obj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            gamemanagerObj.GetComponent<ButtonBehavior>().OnClick());
            button.onClick.AddListener(() =>
            characterData.mapSelected = obj.name);
            
        }

        continueButton.onClick.AddListener(() => SelectMap());
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Each button can ready its own map

    public void SelectMap()
    {
        gamemanagerObj.MapSelected(characterData.mapSelected);
        Debug.Log(characterData.mapSelected);
    }
}
