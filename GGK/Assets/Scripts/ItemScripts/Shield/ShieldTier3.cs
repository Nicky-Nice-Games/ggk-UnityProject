using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ShieldTier3 : BaseItem
{
    private VFXHandler vfxScript;

    // Start is called before the first frame update
    void Start()
    {
        timer = 8.0f;

        if (kart.GetComponent<NEWDriver>() != null)
        {
            vfxScript = kart.gameObject.GetComponent<NEWDriver>().vfxHandler;

            // play shield effect from VFXHandler script 
            vfxScript.PlayShieldVFX(timer);
            // find shield effect attached to the kart
            //shieldEffect = kart.transform.
            //    Find("Normal/Parent/KartModel/ShieldVFX/Shield").GetComponent<VisualEffect>();
        }

        // play shield effect for timer duration
        //shieldEffect.SetFloat("Duration", timer);
        //shieldEffect.Play();
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
}
