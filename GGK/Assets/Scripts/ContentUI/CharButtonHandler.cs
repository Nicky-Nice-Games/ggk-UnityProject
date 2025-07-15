using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharButtonHandler : MonoBehaviour
{
    [SerializeField]
    Sprite buttonSprite;

    [SerializeField]
    Sprite buttonSelectedSprite;

    [SerializeField]
    List<GameObject> buttons;

    [SerializeField]
    TextMeshProUGUI nameplateText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClick(GameObject button)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].GetComponent<Image>().sprite = buttonSprite;
        }

        button.GetComponent<Image>().sprite = buttonSelectedSprite;
        nameplateText.text = button.name;
    }
}
