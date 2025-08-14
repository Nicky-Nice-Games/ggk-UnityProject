using UnityEngine;

/// <summary>
/// Spill
/// </summary>
public class HazardTier1 : BaseItem
{
    uint hitSpinoutID = 0;

    private void Start()
    {
        base.Start();
        Vector3 behindPos = transform.position - transform.forward * 6;
        behindPos.y += 3.5f;
        transform.position = behindPos;

        // Checking for multiplayer
        if(IsSpawned)
        {
            kart.gameObject.GetComponent<NEWDriver>().IncrementHazardUsageTier1Rpc();
        }
        else
        {
            kart.gameObject.GetComponent<NEWDriver>().playerInfo.trapUsage["oilSpill1"]++;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.ToString());
        // stop the trap from falling when they reach the ground/road
        //if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Road"))
        //{
        //    rb.constraints = RigidbodyConstraints.FreezePositionY;
        //}

        if (collision.gameObject.CompareTag("Kart")) // checks if kart gameobject player or npc
        {
            Rigidbody kartRigidbody;
            if (collision.gameObject.TryGetComponent<Rigidbody>(out kartRigidbody)) // checks if they have rb while also assigning if they do
            {
                kartRigidbody.velocity *= 0.125f; //this slows a kart down to an eighth of its speed

                hitSpinoutID = AkUnitySoundEngine.PostEvent("Play_hit_spinout", gameObject);
                if (collision.gameObject.transform.parent.GetChild(0).GetComponent<NEWDriver>() != null)
                {
                    NEWDriver playerKart = collision.gameObject.transform.parent.GetChild(0).GetComponent<NEWDriver>();
                    playerKart.Stun(2.0f);
                }

                if (collision.gameObject.transform.parent.GetChild(0).GetComponent<NPCPhysics>() != null)
                {
                    NPCPhysics npcKart = collision.gameObject.transform.parent.GetChild(0).GetComponent<NPCPhysics>();
                    npcKart.Stun(2.0f);
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
}
