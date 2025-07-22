using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartSounds : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event ItemPickup;
    [SerializeField] AK.Wwise.Event HitSpinout;
    public void PlayItemPickup()
    {
        ItemPickup.Post(gameObject);
    }

    public void PlayHitSpinout()
    {
        HitSpinout.Post(gameObject);
    }
}
