using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

public class GameOverMenuHandeler : MonoBehaviour
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
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReplayButton()
    {
        gamemanagerObj.PlayerSelected();
    }

    public void ReturnToMMButton()
    {
        gamemanagerObj.LoadStartMenu();
    }
}
