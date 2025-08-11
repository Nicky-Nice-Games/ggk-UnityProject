using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartCollisionSounds : MonoBehaviour
{
    [SerializeField] KartSounds kartSounds;
    [SerializeField] VoiceLines voiceLines;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Kart"))
        {
            kartSounds.PlayThud();
        }

        if (collision.gameObject.CompareTag("Projectile"))
        {
            voiceLines.PlayHitCollision();
        }
    }
}
