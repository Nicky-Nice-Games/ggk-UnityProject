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
            gamemanagerObj.GetComponent<GameManager2>().MapSelected());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Each button can ready its own map
}
