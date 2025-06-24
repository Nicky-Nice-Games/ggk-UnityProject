using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterExpression : MonoBehaviour
{
    // References
    public GameObject facePlane;
    public Material normalFace;
    public Material happyFace;
    public Material sadFace;
    public Material shockedFace;

    // Methods
    public void Normal()
    {
        facePlane.GetComponent<SkinnedMeshRenderer>().material = normalFace;
    }
    public void Happy()
    {
        facePlane.GetComponent<SkinnedMeshRenderer>().material = happyFace;
    }
    public void Sad()
    {
        facePlane.GetComponent<SkinnedMeshRenderer>().material = sadFace;
    }
    public void Shocked()
    {
        facePlane.GetComponent<SkinnedMeshRenderer>().material = shockedFace;
    }
}
