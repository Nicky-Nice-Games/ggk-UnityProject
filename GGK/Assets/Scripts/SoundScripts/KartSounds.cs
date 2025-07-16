using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartSounds : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event ItemPickup;
    public void PlayItemPickup()
    {
        ItemPickup.Post(gameObject);
    }
}
