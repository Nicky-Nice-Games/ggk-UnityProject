using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapItem : BaseItem
{
    // Sound
    // public AudioClip hazardSound;

    // Different Items for Different Tiers
    [SerializeField] public GameObject tierOneBody;
    [SerializeField] public GameObject tierTwoBody;
    [SerializeField] public GameObject tierThreeBody;
    [SerializeField] public GameObject tierFourBody;

    // Start is called before the first frame update
    void Start()
    {
        if (!MultiplayerManager.Instance.IsMultiplayer)
        {
            // starts the hazard slightly behind the player
            if (itemTier == 1)
            {
                Vector3 behindPos = transform.position - transform.forward * 6;
                transform.position = behindPos;
            }
            if (itemTier > 1)
            {
                Vector3 behindPos = transform.position - transform.forward * 6 + transform.up * 3;
                transform.position = behindPos;
            }

            // freeze the fake item box's Y position
            if (itemTier == 4)
            {
                rb.constraints = RigidbodyConstraints.FreezePositionY;
            }

            // sends the hazard slightly up and behind the player before landing on the ground
            // rb.AddForce(transform.forward * -750.0f + transform.up * 50.0f);
        }
        else
        {
            Vector3 behindPos = transform.position - transform.forward * 6;
            transform.position = behindPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (itemTier > 2)
        {
            RotateBox();
        }

        if (!IsSpawned)
        {
            return;
        }

        if (IsServer)
        {
            currentPos.Value = transform.position;
        }
        else if (IsClient)
        {
            transform.position = currentPos.Value;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        // stop the trap from falling when they reach the ground/road
        // for every tier except fake item box (it naturally floats a little)
        if(itemTier < 4 && (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Road")))
        {
            rb.constraints = RigidbodyConstraints.FreezePositionY;
        }
    }

    // Code for fake item box & Confused Ritchie
    public void RotateBox()
    {
        transform.rotation *= new Quaternion(0.0f, 2.0f * Time.deltaTime, 0.0f, 1.0f);
    }
    
    // So I don't have to write this out 4 times in LevelUp(), just swap out the tier body and icon for different levels
    public void UpdateComponents(GameObject body, Texture icon)
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
        itemIcon = icon;

        Destroy(tempObject);
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
