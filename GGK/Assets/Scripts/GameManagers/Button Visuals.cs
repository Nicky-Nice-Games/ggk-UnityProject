// Joshua Chisholm
// 6/30/25
// This script handles and stores and aethetic changes
// needed in code for menu buttons.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonVisuals : MonoBehaviour
{
    private Vector3 initialScale;
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
        initialScale = button.transform.localScale;
        button.transform.localScale = button.transform.localScale * 1.1f;
    }

    /// <summary>
    /// Returns to original scale of button
    /// </summary>
    /// <param name="button"></param>
    public void ExitHover(GameObject button)
    {
        button.transform.localScale = initialScale;
    }
}
