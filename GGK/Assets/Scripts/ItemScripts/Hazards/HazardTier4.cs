using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardTier4 : BaseItem
{
    uint hitSpinoutID = 0;
    private void Start()
    {
        Vector3 behindPos = transform.position - transform.forward * 8;
        behindPos.y += 3.0f;
        transform.position = behindPos;
    }

    private new void Update()
    {
        RotateBox();
    }

    private void RotateBox()
    {
        transform.rotation *= new Quaternion(0.0f, 2.0f * Time.deltaTime, 0.0f, 1.0f);
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

                hitSpinoutID = AkUnitySoundEngine.PostEvent("Play_hit_spinout", gameObject);
            }
            NEWDriver playerKart = collision.gameObject.gameObject.GetComponentInChildren<NEWDriver>();
            if (playerKart)
            {
                playerKart.confusedTimer = 10;
                playerKart.isConfused = true;
                playerKart.movementDirection *= -1;
            }
            
            // destroy puck if single player, if multiplayer call rpc in base item to destroy and despawn
            if (!MultiplayerManager.Instance.IsMultiplayer)
            {
                Destroy(this.gameObject);
            }
            else
            {
                DestroyItemRpc(this);
            }
        }
    }
}
