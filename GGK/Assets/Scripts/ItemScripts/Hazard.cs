using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hazard : BaseItem
{

    [SerializeField]
    AudioClip hazardSound;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 15, transform.position.z);

        // sends the hazard slightly up and behind the player before landing on the ground
        if (isUpgraded)
        {
            this.gameObject.transform.localScale += new Vector3(8.5f, 0.0f, 8.5f);
            rb.AddForce(transform.forward * -750.0f + transform.up * 50.0f);
        }
        else
        {
            rb.AddForce(transform.forward * -750.0f + transform.up * 50.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDisable()
    {
        // AudioSource.PlayClipAtPoint(hazardSound, transform.position);

        if (gameObject.scene.isLoaded)
        {
            // create temp game object at hazard location to make an audio source
            GameObject tempSoundObject = new GameObject("TempAudio");
            tempSoundObject.transform.position = transform.position;

            // add audio source to temporary game object
            AudioSource soundPlayer = tempSoundObject.AddComponent<AudioSource>();
            soundPlayer.clip = hazardSound;

            // start audio clip from half a second in
            soundPlayer.time = 0.5f;

            // lower volume
            soundPlayer.volume = 0.1f;

            // play from audio source
            soundPlayer.Play();

            // destroy temp sound object after 2.5 seconds
            Destroy(tempSoundObject, 2.0f);
        }
    }
}
