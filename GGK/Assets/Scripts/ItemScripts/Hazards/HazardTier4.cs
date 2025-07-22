using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Fake Item Box
/// </summary>
public class HazardTier4 : BaseItem
{
    // Start is called before the first frame update
    void Start()
    {

        // sends the hazard slightly up and behind the player before landing on the ground
        // Transform kartTransform = kart.transform; // assuming BaseItem has `kart` reference set by ItemHolder

        transform.position = transform.position
                             - transform.forward * 5f   // behind the kart
                             + transform.up * 1.0f;       // slightly above ground
        if (itemTier > 1)
        {

            Renderer cubeRenderer = this.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", Color.grey);
            rb = GetComponent<Rigidbody>();
            rb.AddForce(new Vector3(0.0f, 100.0f, -750.0f));
        }
        else
        {
            rb = GetComponent<Rigidbody>();
            //rBody.AddForce(new Vector3(0.0f, 100.0f, -750.0f));
            rb.AddForce(transform.forward * -750.0f + transform.up * 50.0f);
        }

    }

    // Update is called once per frame
    private new void Update()
    {
        RotateBox();
    }

    private void FixedUpdate()
    {
        DecreaseTimer();
    }

    public void RotateBox()
    {
        transform.rotation *= new Quaternion(0.0f, 2.0f * Time.deltaTime, 0.0f, 1.0f);
    }

    // private void OnDisable()
    // {
    //     // AudioSource.PlayClipAtPoint(hazardSound, transform.position);
    // 
    //     if (gameObject.scene.isLoaded)
    //     {
    //         // create temp game object at hazard location to make an audio source
    //         GameObject tempSoundObject = new GameObject("TempAudio");
    //         tempSoundObject.transform.position = transform.position;
    // 
    //         // add audio source to temporary game object
    //         AudioSource soundPlayer = tempSoundObject.AddComponent<AudioSource>();
    //         soundPlayer.clip = hazardSound;
    // 
    //         // start audio clip from half a second in
    //         soundPlayer.time = 0.5f;
    // 
    //         // lower volume
    //         soundPlayer.volume = 0.1f;
    // 
    //         // play from audio source
    //         soundPlayer.Play();
    // 
    //         // destroy temp sound object after 2.5 seconds
    //         Destroy(tempSoundObject, 2.0f);
    //     }
    // }
}
