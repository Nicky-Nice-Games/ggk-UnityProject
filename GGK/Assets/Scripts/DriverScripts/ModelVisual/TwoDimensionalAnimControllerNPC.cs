using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TwoDimensionalAnimControllerNPC : MonoBehaviour
{
    public Animator animator;
    public float turningValue = 0;
    public NPCPhysics npc;
    public float lerpSpeed;
    private bool isOwner;
    private bool isSpawned;
    private NetworkObject nObj;
    // Start is called before the first frame update
    void Start()
    {
        //Getting our animator :D
        animator = GetComponent<Animator>();
        nObj = npc.transform.parent.GetComponent<NetworkObject>();
        TwoDimensionalAnimMultiNPC multi = nObj.GetComponent<TwoDimensionalAnimMultiNPC>();
        if (multi)
        {
            multi.controller = this;
        }
        isOwner = nObj.IsOwner;
        isSpawned = nObj.IsSpawned;
    }

    // Update is called once per frame
    void Update()
    {
        //if in singleplayer or in multiplayer and the owner..
        if (!nObj.IsSpawned || (nObj.IsSpawned && isOwner))
        {
            turningValue = Mathf.Lerp(turningValue, npc.movementDirection.x, Time.deltaTime * lerpSpeed);
        }
        
        animator.SetFloat("turningValue", turningValue);
        
    }
}
