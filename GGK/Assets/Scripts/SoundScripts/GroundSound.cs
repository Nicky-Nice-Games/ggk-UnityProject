using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSounds : MonoBehaviour
{
    [SerializeField]
    AudioClip environmentSound;

    AudioSource soundPlayer;

    [SerializeField]
    NEWDriver driverScript;

    bool onGround;

    // Start is called before the first frame update
    void Start()
    {
        soundPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // plays ground audio when driver is moving across it
        if (onGround && driverScript.isDriving)
        {
            if (!soundPlayer.isPlaying)
            {
                soundPlayer.PlayOneShot(environmentSound);
            }
        }
        else
        {
            // stops audio when driver stops moving
            if (soundPlayer.isPlaying)
            {
                soundPlayer.Stop();
            }
        }
    }

    // checks if driver is on the ground
    private void OnCollisionEnter(Collision collision)
    {
        onGround = true;
    }

    // checks when driver leaves ground
    private void OnCollisionExit(Collision collision)
    {
        onGround = false;
    }
}
