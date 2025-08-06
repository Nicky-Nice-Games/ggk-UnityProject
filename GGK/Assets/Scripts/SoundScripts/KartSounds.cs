using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Triggers Wwise sound events for kart-related sounds.
/// </summary>
public class KartSounds : MonoBehaviour
{
    //These are Wwise events that must be assigned in the Inspector to work properly.
    [Header("Wwise Events")]
    [SerializeField] AK.Wwise.Event ItemPickup;
    [SerializeField] AK.Wwise.Event Thud;

    public void PlayItemPickup()
    {
        ItemPickup.Post(gameObject);
    }

    public void PlayThud()
    {
        Thud.Post(gameObject);
    }
}
