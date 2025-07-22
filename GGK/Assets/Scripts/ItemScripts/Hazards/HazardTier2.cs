using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cracked Brick Wall
/// </summary>
public class HazardTier2 : BaseItem
{
    private void Start()
    {
        Vector3 behindPos = transform.position - transform.forward * 6 + transform.up * 3;
        transform.position = behindPos;
    }

    private void OnTriggerEnter(Collider collision)
    {
        // stop the trap from falling when they reach the ground/road
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Road"))
        {
            rb.constraints = RigidbodyConstraints.FreezePositionY;
        }
        if (collision.gameObject.CompareTag("Kart")) // checks if kart gameobject player or npc
        {
            Rigidbody kartRigidbody;
            if (collision.gameObject.TryGetComponent<Rigidbody>(out kartRigidbody)) // checks if they have rb while also assigning if they do
            {
                kartRigidbody.velocity *= 0.125f; //this slows a kart down to an eighth of its speed
                Destroy(gameObject);
            }
        }
    }
}
