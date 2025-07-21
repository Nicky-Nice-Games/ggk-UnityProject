using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedBrickWall : BaseItem
{
    private void Start() {
        Vector3 behindPos = transform.position - transform.forward * 6 + transform.up * 3;
        transform.position = behindPos;
    }
}
