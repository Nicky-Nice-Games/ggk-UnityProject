using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapItem : BaseItem
{
    // Sound
    public AudioClip hazardSound;

    // Different Items for Different Tiers
    public GameObject tierOneBody;
    public Texture tierOneIcon;
    public GameObject tierTwoBody;
    public Texture tierTwoIcon;
    public GameObject tierThreeBody;
    public Texture tierThreeIcon;
    public GameObject tierFourBody;
    public Texture tierFourIcon;

    // Start is called before the first frame update
    void Start()
    {
        // starts the hazard slightly behind the player
        Vector3 behindPos = transform.position - transform.forward * 6;
        transform.position = behindPos;

        // sends the hazard slightly up and behind the player before landing on the ground
        rb.AddForce(transform.forward * -750.0f + transform.up * 50.0f);
    }

    public void LevelUp()
    {
        switch (itemTier)
        {
            case 2:
                UpdateComponents(tierTwoBody, tierTwoIcon);
                break;
            case 3:
                UpdateComponents(tierThreeBody, tierThreeIcon);
                break;
            case 4:
                UpdateComponents(tierFourBody, tierFourIcon);
                break;
            default:
                UpdateComponents(tierOneBody, tierOneIcon);
                break;
        }
    }

    public void UpdateComponents(GameObject body, Texture Icon)
    {
        GameObject tempObject = Instantiate(body);

        // Overwriting MeshFilter values
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        MeshFilter otherMeshFilter = tempObject.GetComponent<MeshFilter>();
        meshFilter.mesh = otherMeshFilter.mesh;

        // Overwriting MeshRenderer values
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        MeshRenderer otherMeshRenderer = tempObject.GetComponent<MeshRenderer>();
        meshRenderer.material = otherMeshRenderer.material;

        // Overwriting Scale Values
        transform.localScale = tempObject.transform.localScale;

        // Overwriting Icon
        itemIcon = Icon;

        Destroy(tempObject);
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
