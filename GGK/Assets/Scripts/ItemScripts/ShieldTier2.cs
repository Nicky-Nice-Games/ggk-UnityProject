using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ShieldTier2 : BaseItem
{
    private VisualEffect shieldEffect;

    // Start is called before the first frame update
    void Start()
    {
        timer = 6.0f;

        if (kart.GetComponent<NEWDriver>() != null)
        {
            // find shield effect attached to the kart
            shieldEffect = kart.transform.
                Find("Normal/Parent/KartModel/ShieldVFX/Shield").GetComponent<VisualEffect>();
        }

        // play shield effect for timer duration
        shieldEffect.SetFloat("Duration", timer);
        shieldEffect.Play();
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
            Destroy(collision.gameObject);
        }
    }
}
