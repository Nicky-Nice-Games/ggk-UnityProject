using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Kart"))
        {
            //other.transform.position = respawnPoint.position;

            //Rigidbody rb = other.GetComponent<Rigidbody>();

            //if(rb != null)
            //{
            //    //rb.velocity = Vector3.zero;
                
            //}

            DynamicRecovery recovery = other.GetComponent<DynamicRecovery>();
            if (recovery != null)
            {
                Debug.Log("Got here!!!!!!!!!!!!!!!!");
                recovery.StartRecovery();

            }

        
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
