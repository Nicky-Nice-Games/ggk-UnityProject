using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyboardHandler : MonoBehaviour
{
    [SerializeField]
    private EventSystem eventSystem;

    [SerializeField]
    private Selectable elementToSelect;

    [SerializeField]
    GameObject UIPanel;


    private void Start()
    {
        eventSystem = FindObjectOfType<EventSystem>();

        if (eventSystem == null)
        {
            Debug.Log("Did not find an Event System in your scene.", context:this);
        }
    }

    /// <summary>
    /// starts at a specified gameobject when changing UI panels
    /// </summary>
    public void JumpToElement()
    {
        if (eventSystem == null)
        {
            Debug.Log("This item has no event system referenced yet", context: this);
        }

        if (elementToSelect == null)
        {
            Debug.Log("This should jump where?", context: this);
        }

        eventSystem.SetSelectedGameObject(elementToSelect.gameObject);
    }

    // opens controller or keyboard binds
    public void Open()
    {
        UIPanel.SetActive(true);
    }

    // close controller or keyboard binds
    public void Close()
    {
        UIPanel.SetActive(false);
    }
}
