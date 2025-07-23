using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class Shield : BaseItem
{
    // save the default color of the shield
    private Renderer renderer;
    private Material material;
    private Color defaultColor;
    // the color the shield flashes the last few seconds of it's duration
    private Color timerColor;

    // the interval between color switches on the shield
    private float blinkInterval = 1.0f;

    // the time when the shield should indicate it's ending
    private float indicatorTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (!MultiplayerManager.Instance.IsMultiplayer)
        {
            // save the default color of the shield
            renderer = GetComponent<Renderer>();
            material = renderer.material;
            defaultColor = material.color;

            timerColor = Color.red;
            timerColor.a = defaultColor.a;

            shieldEffect.SetFloat("Duration", timer);
            shieldEffect.Play();
        }
        else
        {
            // save the default color of the shield
            renderer = GetComponent<Renderer>();
            material = renderer.material;
            defaultColor = material.color;

            timerColor = Color.red;
            timerColor.a = defaultColor.a;

            if (IsServer)
            {
                currentPos.Value = transform.position;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Counts down to despawn
        DecreaseTimer();

        if (timer <= indicatorTime)
        {
            // flash a different color to indicate the shield is ending
            StartCoroutine(ColorBlink());
        }

        // Sets shield position to the karts position
        if (kart)
        {
            transform.position = new Vector3(kart.transform.position.x, kart.transform.position.y, kart.transform.position.z);
        }

        if (!IsSpawned)
        {
            return;
        }

        if (NetworkManager.IsHost)
        {
            currentPos.Value = transform.position;
        }
        else if (NetworkManager.IsClient && !NetworkManager.IsHost)
        {
            transform.position = currentPos.Value;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        // Destroys hazards at any tier and projectiles when less than tier 3
        if ((itemTier < 3 && collision.gameObject.CompareTag("Projectile")) || collision.gameObject.CompareTag("Hazard"))
        {
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Kart") && itemTier == 4)
        {
            // ensures that the shield is hitting a player or NPC kart
            if (collision.gameObject.GetComponent<NEWDriver>() != null)
                //&& collision.gameObject.GetComponent<NEWDriver>() != Kart)
            {
                NEWDriver playerKart = collision.gameObject.GetComponent<NEWDriver>();
                playerKart.Stun(2.0f);
            }

            if(collision.gameObject.GetComponent<NPCDriver>() != null)
                //&& collision.gameObject.GetComponent<NPCDriver>() != Kart)
            {
                NPCDriver npcKart = collision.gameObject.GetComponent<NPCDriver>();
                npcKart.StartRecovery(2.0f);
            }
        }
    }

    // Should be moved to BaseItem to be overridable
    // Added separately for now so there aren't big conflicts with the Puck when that's pushed
    //public void LevelUp()
    //{
    //    // update the icon based on the item tier
    //    switch (itemTier)
    //    {
    //        case 2:
    //            itemIcon = tierTwoItemIcon;
    //            break;
    //        case 3:
    //            itemIcon = tierThreeItemIcon;
    //            break;
    //        case 4:
    //            itemIcon = tierFourItemIcon;
    //            break;
    //        default:
    //            itemIcon = tierOneItemIcon;
    //            break;
    //    }
    //}

    IEnumerator ColorBlink()
    {
        float elapsed = 0f;
        bool toggle = false;

        while (elapsed < indicatorTime)
        {

            material.color = toggle ? defaultColor : timerColor;
            toggle = !toggle;

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        material.color = defaultColor;
    }
}
