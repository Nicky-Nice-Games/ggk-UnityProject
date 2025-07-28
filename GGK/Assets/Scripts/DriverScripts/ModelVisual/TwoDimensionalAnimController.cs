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
        TwoDAnimMultiplayer multi = playerObject.GetComponent<TwoDAnimMultiplayer>();
        if (multi)
        {
            multi.controller = this;
        }
        if (IsServer) GetComponent<NetworkObject>().SpawnAsPlayerObject(playerObject.OwnerClientId);
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerObject.IsSpawned || (playerObject.IsSpawned && (IsOwner || playerObject.IsOwner)))
        {
            turningValue = Mathf.Lerp(turningValue, driver.movementDirection.x, Time.deltaTime * lerpSpeed);
        }
        
        animator.SetFloat("turningValue", turningValue);
    }

    [ServerRpc]
    private void HandleAnimationServerRpc(float value)
    {
        animator.SetFloat("turningValue", value);
    }
    
    [Rpc(SendTo.ClientsAndHost,  RequireOwnership = false)]
    private void HandleAnimationRpc(float value, ulong testid)
    {
        print("hello from client " + testid + ", " + value);
        animator.SetFloat("turningValue", value);
    }
}
