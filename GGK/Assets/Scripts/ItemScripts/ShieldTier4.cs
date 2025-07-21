using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTier4 : BaseItem
{
    // Start is called before the first frame update
    void Start()
    {
        timer = 10.0f;
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
            // ensures that the shield is hitting a player or NPC kart
            if (collision.gameObject.GetComponent<NEWDriver>() != null)
            //&& collision.gameObject.GetComponent<NEWDriver>() != Kart)
            {
                NEWDriver playerKart = collision.gameObject.GetComponent<NEWDriver>();
                playerKart.Stun(2.0f);
            }

            if (collision.gameObject.GetComponent<NPCDriver>() != null)
            //&& collision.gameObject.GetComponent<NPCDriver>() != Kart)
            {
                NPCDriver npcKart = collision.gameObject.GetComponent<NPCDriver>();
                npcKart.StartRecovery(2.0f);
            }
        }
    }
}
