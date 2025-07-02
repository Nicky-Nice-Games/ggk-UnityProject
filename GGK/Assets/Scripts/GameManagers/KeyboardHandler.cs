using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardHandler : MonoBehaviour
{
    // opens controller or keyboard binds
    public void Open()
    {
        gameObject.SetActive(true);
    }

    // close controller or keyboard binds
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
