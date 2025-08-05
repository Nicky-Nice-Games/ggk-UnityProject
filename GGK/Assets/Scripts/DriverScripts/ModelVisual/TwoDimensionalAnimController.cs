using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class TwoDimensionalAnimController : NetworkBehaviour
{
    Animator animator;
    public float turningValue = 0;
    public NEWDriver driver;
    public float lerpSpeed;

    NetworkObject playerObject;
    // Start is called before the first frame update\s
    void Start()
    {
        //Getting our animator :D
        animator = GetComponent<Animator>();
        playerObject = transform.root.GetComponent<NetworkObject>();
        TwoDimensionalAnimMultiplayer multi = playerObject.GetComponent<TwoDimensionalAnimMultiplayer>();
        if (multi)
        {
            multi.controller = this;
        }
        if (IsServer) GetComponent<NetworkObject>().SpawnAsPlayerObject(playerObject.OwnerClientId);
    }

    // Update is called once per frame
    void Update()
    {
        //if in singleplayer or in multiplayer and the owner..
        if (!playerObject.IsSpawned || (playerObject.IsSpawned && (IsOwner || playerObject.IsOwner)))
        {
            float turningDirection = driver.movementDirection.x;

            if (driver.isDrifting) { turningDirection *= 2f; }

            turningValue = Mathf.Lerp(turningValue, turningDirection, Time.deltaTime * lerpSpeed);
            

            
        }
        
        animator.SetFloat("turningValue", turningValue);
    }
}
