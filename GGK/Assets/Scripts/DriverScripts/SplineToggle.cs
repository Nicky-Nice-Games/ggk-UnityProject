using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class SplineToggle : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    List <SplineExtrude> extrudeSplines;
    [SerializeField]
    GameObject splineHolder;

    [SerializeField]
    private List<Mesh> originalMeshes = new List<Mesh>();
    private bool isVisible = true;
    void Start()
    {
        SplineExtrude[] foundSplines = splineHolder.GetComponentsInChildren<SplineExtrude>(true);

        foreach (SplineExtrude spline in foundSplines)
        {
            MeshFilter mf = spline.GetComponent<MeshFilter>();
            if (mf != null)
            {
                extrudeSplines.Add(spline);
                originalMeshes.Add(mf.sharedMesh);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnToggle(InputAction.CallbackContext context)
    {
        isVisible = !isVisible;

        for (int i = 0; i < extrudeSplines.Count; i++)
        {
            SplineExtrude spline = extrudeSplines[i];
            MeshFilter mf = spline.GetComponent<MeshFilter>();

            if (mf != null)
            {
                mf.sharedMesh = isVisible ? originalMeshes[i] : null;
            }
        }
    }
}
