// Joshua Chisholm & Joshua Ward
// 7/10/25
// This script handles and stores and aethetic changes
// needed in code for menu buttons.
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ButtonVisualsContent : MonoBehaviour
{
    private float scaleFactor = 1.03f;
    private bool isMouseDown = false;
    private bool exitWhileMouseDown = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ScaleUp(GameObject button)
    {
        button.transform.localScale = Vector3.one * scaleFactor;
    }

    private void ScaleDown(GameObject button)
    {
        button.transform.localScale = Vector3.one / scaleFactor;
    }

    private void ScaleReset(GameObject button)
    {
        button.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Increases the scale of a button when hovering
    /// </summary>
    /// <param name="button"></param>
    public void OnHover(GameObject button)
    {
        ScaleUp(button);

        if (button.tag != "NoUnderline")
        {
            button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Underline;
        }
    }

    /// <summary>
    /// Returns to original scale of button
    /// </summary>
    /// <param name="button"></param>
    public void ExitHover(GameObject button)
    {
        if (isMouseDown)
        {
            exitWhileMouseDown = true;
        }

        ScaleReset(button);

        if (button.tag != "NoUnderline")
        {
            button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
        }
    }

    public void MouseDown(GameObject button)
    {
        isMouseDown = true;
        ScaleDown(button);
    }

    public void MouseUp(GameObject button)
    {
        isMouseDown = false;

        if (!exitWhileMouseDown)
        {
            ScaleUp(button);
        }
        exitWhileMouseDown = false;
    }
}
