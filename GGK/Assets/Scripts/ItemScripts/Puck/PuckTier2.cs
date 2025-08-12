using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class PuckTier2 : BaseItem
{
    private GameObject kartTarget;                 // The kart the puck tracks to
    [SerializeField] private NavMeshAgent agent;   // The AI component of the puck
    private bool goStraight;                       // If the puck goes straight
    public GameObject[] karts;
    private float startTimer = 0;
    private int itemCount = 3;

    [SerializeField]
    SpeedCameraEffect cameraScript;   // Camera effect

    // Start is called before the first frame update
    void Start()
    {

        karts = GameObject.FindGameObjectsWithTag("Kart");

        // If the kart is looking backwards
        // sends puck backwards
        if (!MultiplayerManager.Instance.IsMultiplayer)
        {
            // The puck spawns 15 units in front of the kart
            transform.position = new Vector3(transform.position.x + transform.forward.x * 5f,
                            transform.position.y,
                            transform.position.z + transform.forward.z * 5f);
            kart.GetComponent<NEWDriver>().playerInfo.offenceUsage["puck2"]++;
        }
        else
        {
            // The puck spawns 15 units in front of the kart
            transform.position = new Vector3(transform.position.x + transform.forward.x * 5f,
                            transform.position.y,
                            transform.position.z + transform.forward.z * 5f);


            useCount = 3;

            if (IsServer)
            {
                currentPos.Value = transform.position;
            }
            if (IsSpawned) kart.gameObject.GetComponent<NEWDriver>().IncrementOffenseUsageTier2Rpc();
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
        FindClosestKart(karts);

        // Moves the puck towards the closest kart
        agent.SetDestination(kartTarget.transform.position);
        // Moves the puck straight normally but still follows pathing
        if (goStraight)
        {
            agent.SetDestination(transform.position + (transform.forward * 3));
        }



        // Counts down to despawn
        DecreaseTimer();
    }

    void OnCollisionEnter(Collision collision)
    {
        // If puck hits a kart
        if (collision.gameObject.CompareTag("Kart"))
        {
            // If puck hits a kart
            if (startTimer >= 0.1f)
            {

                if (collision.gameObject.transform.parent.GetChild(0).GetComponent<NEWDriver>() != null)
                {
                    NEWDriver playerKart = collision.gameObject.transform.parent.GetChild(0).GetComponent<NEWDriver>();
                    playerKart.Stun(2.0f);
                }

                if (collision.gameObject.transform.parent.GetChild(0).GetComponent<NPCPhysics>() != null)
                {
                    NPCPhysics npcKart = collision.gameObject.transform.parent.GetChild(0).GetComponent<NPCPhysics>();
                    npcKart.Stun(2.0f);
                }

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
            if (!collision.gameObject.GetComponent<PuckTier4>())
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
    /// Finds the kart closest to the player using the puck.
    /// Ignores the player who used it.
    /// Ignores karts behind player.
    /// </summary>
    /// <param name="karts">A list of every kart in the scene</param>
    public void FindClosestKart(GameObject[] karts)
    {
        GameObject closestKart = null;        // The starting empty closest kart
        float baseDistance = Mathf.Infinity;  // The minimum distance encloses every kart

        foreach (GameObject kart in karts)
        {
            // Checks each distance to the kart
            float distance = Vector3.Distance(transform.position, kart.transform.position);
            Vector3 direction = kart.transform.position - transform.position;
            // Returns if kart is in front or behind user kart
            float dp = Vector3.Dot(transform.forward, direction.normalized);

            // If kart is in front of the user kart
            if (dp > 0)
            {
                // Sets the closest kart to whatever is closest
                if (distance < baseDistance)
                {
                    baseDistance = distance;
                    closestKart = kart;
                }
            }
        }
        // Sets the puck to target the closest kart
        if (closestKart)
        {
            kartTarget = closestKart;
        }
        // If there is no kart in front of user,
        // then puck goes straight
        else
        {
            goStraight = true;
        }

    }
}
