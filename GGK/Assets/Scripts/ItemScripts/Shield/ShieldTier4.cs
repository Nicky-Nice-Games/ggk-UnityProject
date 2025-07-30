using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ShieldTier4 : BaseItem
{
    private VFXHandler vfxScript;

    public override void OnNetworkSpawn()
    {
        base.Start();

        timer = 10.0f;

        if (kart.gameObject.GetComponent<NEWDriver>() != null) // for players
        {
            // find the visual effect script from the kart
            vfxScript = kart.gameObject.GetComponent<NEWDriver>().vfxHandler;

            // play shield effect from VFXHandler script 
            vfxScript.PlayShieldVFX(timer);
        }
        else if (kart.gameObject.GetComponent<NPCPhysics>() != null) // for npcs
        {
            // find the visual effect script from the npc kart
            vfxScript = kart.gameObject.GetComponent<VFXHandler>();

            // play shield effect from VFXHandler script 
            vfxScript.PlayShieldVFX(timer);
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        if (!IsSpawned)
        {
            base.Start();
            timer = 10.0f;

            if (kart.gameObject.GetComponent<NEWDriver>() != null) // for players
            {
                // find the visual effect script from the kart
                vfxScript = kart.gameObject.GetComponent<NEWDriver>().vfxHandler;

                // play shield effect from VFXHandler script 
                vfxScript.PlayShieldVFX(timer);
            }
            else if (kart.gameObject.GetComponent<NPCPhysics>() != null) // for npcs
            {
                // find the visual effect script from the npc kart
                vfxScript = kart.gameObject.GetComponent<VFXHandler>();

                // play shield effect from VFXHandler script 
                vfxScript.PlayShieldVFX(timer);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Counts down to despawn
        DecreaseTimer();

        // Sets shield position to the karts position
        if (kart)
        {
            transform.position = new Vector3(kart.transform.position.x, kart.transform.position.y, kart.transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Kart"))
        {
            if (collision.gameObject.transform.parent.GetChild(0).GetComponent<ItemHolder>() == kart)
            {
                return;
            }

            // ensures that the shield is hitting a player or NPC kart
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
        }
    }
}
