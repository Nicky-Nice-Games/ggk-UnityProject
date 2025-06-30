// Joshua Chisholm
// 6/30/25
// This script handles and stores and aethetic changes
// needed in code for menu buttons.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonVisuals : MonoBehaviour
{

    private float initialScale;
    private float initialXPos;
    private float initialYPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Increases the scale of a button when hovering
    /// </summary>
    /// <param name="button"></param>
    public void OnHover(GameObject button)
    {
        button.transform.localScale = new Vector3(3.3f, 3.3f, 3.3f);
    }

    /// <summary>
    /// Returns to original scale of button
    /// </summary>
    /// <param name="button"></param>
    public void ExitHover(GameObject button)
    {
        button.transform.localScale = new Vector3(3f, 3f, 3f);
    }
}
