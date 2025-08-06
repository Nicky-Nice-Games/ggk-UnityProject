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

    [SerializeField] private GameObject sphereHolder;

    [SerializeField] private List<MeshRenderer> sphereRenderers = new List<MeshRenderer>();
    [SerializeField] private bool isVisible;
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


        foreach (Transform followTransform in sphereHolder.transform)
        {
            // Find the Sphere inside this followX
            Transform sphere = followTransform.Find("Sphere");
            if (sphere != null)
            {
                MeshRenderer mr = sphere.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    sphereRenderers.Add(mr);
                }
                else
                {
                    Debug.LogWarning($"{sphere.name} does not have a MeshRenderer.");
                    sphereRenderers.Add(null);
                }
            }
            else
            {
                Debug.LogWarning($"{followTransform.name} does not have a child named 'Sphere'.");
                sphereRenderers.Add(null);
            }
        }

        isVisible = false;
        for (int i = 0; i < extrudeSplines.Count; i++)
        {
            SplineExtrude spline = extrudeSplines[i];
            MeshFilter mf = spline.GetComponent<MeshFilter>();


            mf.sharedMesh = isVisible ? originalMeshes[i] : null;



            sphereRenderers[i].enabled = isVisible;

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

            
                mf.sharedMesh = isVisible ? originalMeshes[i] : null;
            

            
                sphereRenderers[i].enabled = isVisible;
            
        }
    }
}
