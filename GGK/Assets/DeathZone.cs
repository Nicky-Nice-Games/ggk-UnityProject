using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Kart"))
        {
            if (other.GetComponent<NEWDriver>() != null) { other.GetComponent<NEWDriver>().IncrementFellOffMapRpc(); }

            DynamicRecovery recovery = other.GetComponent<DynamicRecovery>();
            if (recovery != null)
            {
                //Debug.Log("Got here!!!!!!!!!!!!!!!!");
                recovery.StartRecovery();

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
