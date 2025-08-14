using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomTrapIcon : MonoBehaviour
{
    [SerializeField] private Material[] fakeIcons; // The fake item brick's mesh renderer

    void Start()
    {
        HazardTier2 parentScript = GetComponentInParent<HazardTier2>();
        if (parentScript != null)
        {
            int index = parentScript.index;

            if (fakeIcons.Length == 0) return;
            // Apply it to this object's MeshRenderer
            MeshRenderer myRenderer = GetComponent<MeshRenderer>();

            myRenderer.material = fakeIcons[index];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
