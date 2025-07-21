using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spill : BaseItem
{
    // Start is called before the first frame update
    void Start()
    {
        // starts the hazard slightly behind the player
        Vector3 behindPos = transform.position - transform.forward * 6;
        transform.position = behindPos;
    }

    private void OnTriggerEnter(Collider collision)
    {
        // stop the trap from falling when they reach the ground/road
        // for every tier except fake item box (it naturally floats a little)
        if(itemTier < 4 && (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Road")))
        {
            rb.constraints = RigidbodyConstraints.FreezePositionY;
        }
    }
}
