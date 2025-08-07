using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquirrelSound : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event Squirrels;

    public void PlaySquirrels()
    {
        Squirrels.Post(gameObject);
    }
}
