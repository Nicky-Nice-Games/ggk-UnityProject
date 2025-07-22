using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spill
/// </summary>
public class HazardTier1 : BaseItem
{
    private void Start()
    {
        Vector3 behindPos = transform.position - transform.forward * 6;
        transform.position = behindPos;
    }

    private void OnTriggerEnter(Collider collision)
    {
        // stop the trap from falling when they reach the ground/road
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Road"))
        {
            rb.constraints = RigidbodyConstraints.FreezePositionY;
        }
    }
}
