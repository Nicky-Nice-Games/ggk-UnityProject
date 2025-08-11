using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class PuckTier4 : BaseItem
{
    private GameObject kartTarget;                 // The kart the puck tracks to
    [SerializeField] private NavMeshAgent agent;   // The AI component of the puck
    private float startTimer = 0;

    [SerializeField]
    SpeedCameraEffect cameraScript;   // Camera effect

    // Start is called before the first frame update
    void Start()
    {
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
            if (IsSpawned) kart.gameObject.GetComponent<NEWDriver>().IncrementOffenseUsageTier4Rpc();
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
    }

    void OnCollisionEnter(Collision collision)
    {

        // If puck hits a kart
        if (collision.gameObject.CompareTag("Kart"))
        {
            // Detects if puck hit an NPC or player
            NEWDriver playerKart = collision.transform.root.GetChild(0).GetComponent<NEWDriver>();
            NPCDriver npcKart = collision.gameObject.GetComponent<NPCDriver>();

            // Stops player
            if (playerKart)
            {
                playerKart.acceleration = new Vector3(0.0f, 0.0f, 0.0f);
                playerKart.sphere.velocity = new Vector3(0.0f, 0.0f, 0.0f);
                collision.transform.root.GetChild(0).GetComponent<ItemHolder>().ApplyIconSpin(collision.transform.root.GetChild(0).gameObject, 1);
                Debug.Log(collision.transform.root.GetChild(0).gameObject);
            }
            // Stops NPC and starts recovery
            else if (npcKart)
            {
                npcKart.velocity = new Vector3(0.0f, 0.0f, 0.0f);
                npcKart.StartRecovery();
                collision.gameObject.GetComponent<ItemHolder>().ApplyIconSpin(collision.gameObject, 1);
            }

            // Destroys puck only if it is tier 4 and it hits the first
            // place kart
            if (collision.transform.root.gameObject == kartTarget)
            {
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
        // Pucks can destroy other pucks
        else if (collision.gameObject.CompareTag("Projectile"))
        {
            // Prevents puck from destroying tier 4 puck
            if (collision.gameObject.GetComponent<PuckTier4>())
            {
                Destroy(collision.gameObject);
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

    }

    // If colliding with cracked brick wall
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<TrapItem>() && other.gameObject.GetComponent<TrapItem>().ItemTier == 2)
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
    /// Finds what kart is in first place
    /// </summary>
    /// <returns>The kart in first place</returns>
    public Transform FindFirstPlace()
    {
        // Finds first place kart
        Transform firstPlaceKart = GameObject.Find("PlacementManager").GetComponent<PlacementManager>().sortedList[0].gameObject.transform;
        // Sets the kart target
        kartTarget = firstPlaceKart.root.gameObject;
        return firstPlaceKart;
    }
}
