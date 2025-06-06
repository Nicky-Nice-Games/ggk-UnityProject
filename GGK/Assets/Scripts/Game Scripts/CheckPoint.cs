using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        LapCounter kart = other.GetComponent<LapCounter>();

        if (kart != null ) kart.CheckCheckPoint(this);
    }
}
