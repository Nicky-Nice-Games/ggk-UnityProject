using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ShieldTier1 : BaseItem
{
    private VFXHandler vfxScript;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        // this line clears the inventory instantly and allows for new items to get picked up again
        timerEndCallback();
        // this line makes inventory clear when the shield ends
        // OnTimerEnd += (object sender, EventArgs e) => { timerEndCallback(); };

        timer = 4.0f;

        if (kart.gameObject.GetComponent<NEWDriver>() != null) // for players
        {
            // find the visual effect script from the kart
            vfxScript = kart.gameObject.GetComponent<NEWDriver>().vfxHandler;

            // play shield effect from VFXHandler script 
            vfxScript.PlayShieldVFX(timer);

            kart.gameObject.GetComponent<NEWDriver>().playerInfo.offenceUsage["shield1"]++;
        }
        else if (kart.gameObject.GetComponent<NPCPhysics>() != null) // for npcs
        {
            // find the visual effect script from the npc kart
            vfxScript = kart.gameObject.GetComponent<VFXHandler>();

            // play shield effect from VFXHandler script 
            vfxScript.PlayShieldVFX(timer);
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
        // destroys projectiles and hazards
        if (collision.gameObject.CompareTag("Projectile") || collision.gameObject.CompareTag("Hazard"))
        {
            if (!MultiplayerManager.Instance.IsMultiplayer)
            {
                Destroy(collision.gameObject);
            }
            else
            {
                DestroyItemRpc(collision.gameObject.GetComponent<BaseItem>());
            }
        }
    }
}
