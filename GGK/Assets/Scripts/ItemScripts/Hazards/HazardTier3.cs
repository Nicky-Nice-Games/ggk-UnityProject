using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Confused Ritchie
/// </summary>
public class HazardTier3 : BaseItem
{
    private void Start()
    {

    }

    private new void Update()
    {
        RotateBox();
    }

    // Code for fake item box & Confused Ritchie
    private void RotateBox()
    {
        transform.rotation *= new Quaternion(0.0f, 2.0f * Time.deltaTime, 0.0f, 1.0f);
    }
}
