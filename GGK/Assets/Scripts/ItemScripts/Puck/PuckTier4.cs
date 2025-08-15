using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

//REWORK FOR AUDIO MAIN

public class PuckTier4 : BaseItem
{
    private GameObject kartTarget;                 // The kart the puck tracks to
    [SerializeField] private NavMeshAgent agent;   // The AI component of the puck
    private float startTimer = 0;

    [SerializeField]
    SpeedCameraEffect cameraScript;   // Camera effect

    uint hitSpinoutID = 0;
    uint crashID = 0;

    // Start is called before the first frame update
    void Start()
    {
        agent.updateRotation = true;
        agent.angularSpeed = 750.0f;

        // If the kart is looking backwards
        // sends puck backwards
        if (!MultiplayerManager.Instance.IsMultiplayer)
        {
            if (kart.camera && kart.camera.IsHoldingTab && itemTier < 2)
            {
                // The puck spawns 15 units behind of the kart
                transform.position = new Vector3(transform.position.x - transform.forward.x * 10f,
                                transform.position.y,
                                transform.position.z - transform.forward.z * 10f);
            }
            // If the kart is looking forwards
            // sends puck forwards
            else
            {
                // The puck spawns 15 units in front of the kart
                transform.position = new Vector3(transform.position.x + transform.forward.x * 5f,
                                transform.position.y,
                                transform.position.z + transform.forward.z * 5f);
            }
            kart.GetComponent<NEWDriver>().playerInfo.offenceUsage["puck4"]++;
        }
        else
        {
            // The puck spawns 15 units in front of the kart
            transform.position = new Vector3(transform.position.x + transform.forward.x * 5f,
                            transform.position.y,
                            transform.position.z + transform.forward.z * 5f);

            useCount = 1;

            if (IsServer)
            {
                currentPos.Value = transform.position;
            }
            if (IsSpawned) kart.gameObject.GetComponent<NEWDriver>().IncrementOffenseUsageTier4Rpc();
        }
    }

    void Update()
    {
        startTimer += Time.deltaTime;

        if (!IsSpawned)
        {
            return;
        }

        if (NetworkManager.Singleton.IsHost)
        {
            currentPos.Value = transform.position;
        }
        else if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
        {
            transform.position = currentPos.Value;
        }

    }

    void FixedUpdate()
    {
        // Puck moves toward the player in first place
        agent.SetDestination(FindFirstPlace().position);

        // Counts down to despawn
        DecreaseTimer();

        transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);

        Debug.Log("Kart target is: " + kartTarget);
    }

    // If colliding with cracked brick wall
    void OnTriggerEnter(Collider other)
    {
        // If puck hits a kart
        if (other.gameObject.CompareTag("Kart"))
        {
            // If puck hits a kart
            if (startTimer >= 0.1f)
            {
                //plays a sound when puck hits any kart
                hitSpinoutID = AkUnitySoundEngine.PostEvent("Play_hit_spinout", gameObject);
                crashID = AkUnitySoundEngine.PostEvent("Play_crash", gameObject);

                if (other.gameObject.transform.parent.GetChild(0).GetComponent<NEWDriver>() != null)
                {
                    Debug.Log("Tier 4 Puck Hit Kart");
                    NEWDriver playerKart = other.gameObject.transform.parent.GetChild(0).GetComponent<NEWDriver>();
                    playerKart.Stun(2.0f);
                }

                if (other.gameObject.transform.parent.GetChild(0).GetComponent<NPCPhysics>() != null)
                {
                    NPCPhysics npcKart = other.gameObject.transform.parent.GetChild(0).GetComponent<NPCPhysics>();
                    npcKart.Stun(2.0f);
                }

                Debug.Log(other.gameObject.transform.parent.GetChild(0).GetComponent<ItemHolder>() == kartTarget.GetComponent<ItemHolder>());

                // Prevent puck from being destroyed until it hits the kart in first place.
                // destroy puck if single player, if multiplayer call rpc in base item to destroy and despawn
                if (other.gameObject.transform.parent.GetChild(0).GetComponent<ItemHolder>() == kartTarget.GetComponent<ItemHolder>())
                {
                    if (!MultiplayerManager.Instance.IsMultiplayer)
                    {
                        Destroy(this.gameObject);
                    }
                    else
                    {
                        DestroyItemRpc(this);
                    }
                }
            }
        }
    
        // Pucks can destroy other pucks
        else if (other.gameObject.CompareTag("Projectile"))
        {
            // Prevents puck from destroying tier 4 puck
            if (other.gameObject.GetComponent<PuckTier4>())
            {
                Destroy(other.gameObject);
                //if (!MultiplayerManager.Instance.IsMultiplayer)
                //{
                //    Destroy(this.gameObject);
                //}
                //else if (MultiplayerManager.Instance.IsMultiplayer && IsServer)
                //{
                //    this.NetworkObject.Despawn();
                //    Destroy(this.gameObject);
                //}

                // destroy puck if single player, if multiplayer call rpc in base item to destroy and despawn
                if (!MultiplayerManager.Instance.IsMultiplayer)
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    DestroyItemRpc(this);
                }
            }
        }
        else if (other.gameObject.GetComponent<TrapItem>() && other.gameObject.GetComponent<TrapItem>().ItemTier == 2)
        {
            Destroy(other.gameObject);
            //if (!MultiplayerManager.Instance.IsMultiplayer)
            //{
            //    Destroy(this.gameObject);
            //}
            //else if (MultiplayerManager.Instance.IsMultiplayer && IsServer)
            //{
            //    this.NetworkObject.Despawn();
            //    Destroy(this.gameObject);
            //}

            // destroy puck if single player, if multiplayer call rpc in base item to destroy and despawn
            if (!MultiplayerManager.Instance.IsMultiplayer)
            {
                Destroy(this.gameObject);
            }
            else
            {
                DestroyItemRpc(this);
            }
        }
    }

    /// <summary>
    /// Finds what kart is in first place.
    /// </summary>
    /// <returns>The kart in first place</returns>
    public Transform FindFirstPlace()
    {
        // Finds first place kart
        Transform firstPlaceKart = GameObject.Find("PlacementManager").GetComponent<PlacementManager>().sortedList[0].gameObject.transform;
        // if the kart in first place is the user, then track to the kart in second
        if (firstPlaceKart.gameObject.transform.parent.GetChild(0).GetComponent<ItemHolder>() == this.kart)
        {
            firstPlaceKart = GameObject.Find("PlacementManager").GetComponent<PlacementManager>().sortedList[1].gameObject.transform;
        }
        Debug.Log("Tracked kart is: " + firstPlaceKart.root.gameObject);
        // Sets the kart target
        kartTarget = firstPlaceKart.parent.GetChild(0).gameObject;
        return firstPlaceKart;
    }
}
