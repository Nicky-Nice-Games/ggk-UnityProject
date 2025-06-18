using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftSound : MonoBehaviour
{
    [SerializeField]
    AudioClip driftSound;

    AudioSource soundPlayer;

    [SerializeField]
    NEWDriver driverScript;

    // Start is called before the first frame update
    void Start()
    {
        soundPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // checks if driver is drifting and plays audio if it is
        // stops if driver cancels drift
        if (driverScript.isDrifting)
        {
            if (!soundPlayer.isPlaying)
            {
                soundPlayer.PlayOneShot(driftSound);
            }
        }
        else
        {
            if (soundPlayer.isPlaying)
            {
                soundPlayer.Stop();
            }
        }
    }
}
