using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class PuckTier3 : BaseItem
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
            if (startTimer >= 0.1f)
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

                // Otherwise destroys puck regardless of kart hit
                else
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
                        DestroyItemRpc(this.gameObject.GetComponent<BaseItem>());
                    }
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
                    DestroyItemRpc(this.gameObject.GetComponent<BaseItem>());
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
                DestroyItemRpc(this.gameObject.GetComponent<BaseItem>());
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
