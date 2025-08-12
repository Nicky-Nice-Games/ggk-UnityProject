using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class ChangeMenuKartColor : MonoBehaviour
{
    public UnityEngine.Color color;
    public Renderer renderer;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Run on-click of the color buttons and changes the color of the kart in the scene.
    /// </summary>
    /// <param name="_color">The selected color.</param>
    public void SetColor(Button _color)
    {
        color = _color.transform.GetChild(0).GetComponent<Image>().color;

        if (color != null)
        {
            Material[] materials = renderer.materials;

            // Create a new instance of the existing material so we don't modify the shared one
            Material newMaterial = new Material(materials[3]);
            newMaterial.color = color;

            // Replace the old material in the array
            materials[3] = newMaterial;

            // Assign the array back to the renderer
            renderer.materials = materials;
        }
    }

}
