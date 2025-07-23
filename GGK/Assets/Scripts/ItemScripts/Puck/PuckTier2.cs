using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class PuckTier2 : BaseItem
{
    private Vector3 direction;                     // The direction and speed the puck travels
    [SerializeField] private int maxBounces;       // The max number the puck can bounce off walls
    private int bounceCount;                       // The times the puck bounced
    private float startTimer = 0;
    private int itemCount = 3;

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

                // The speed of the puck times 200
                // Keeps the player from hitting it during use regardless of speed
                direction = -(transform.forward * 200.0f);
            }
            // If the kart is looking forwards
            // sends puck forwards
            else
            {
                // The puck spawns 15 units in front of the kart
                transform.position = new Vector3(transform.position.x + transform.forward.x * 5f,
                                transform.position.y,
                                transform.position.z + transform.forward.z * 5f);

                // The speed of the puck times 200
                // Keeps the player from hitting it during use regardless of speed
                direction = transform.forward * 200.0f;
            }


            // Starts the puck with 0 bounces
            bounceCount = 0;
        }
        else
        {
            // The puck spawns 15 units in front of the kart
            transform.position = new Vector3(transform.position.x + transform.forward.x * 5f,
                            transform.position.y,
                            transform.position.z + transform.forward.z * 5f);

            // The speed of the puck times 200
            // Keeps the player from hitting it during use regardless of speed
            direction = transform.forward * 200.0f;

            bounceCount = 0;

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

        // Acts as stronger gravity to bring the puck down
        rb.AddForce(Vector3.down * 40.0f, ForceMode.Acceleration);

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
        rb.velocity = direction;

        // Destroys puck if it bounced enough times
        if (bounceCount == maxBounces + 1)
        {
            if (!MultiplayerManager.Instance.IsMultiplayer)
            {
                Destroy(this.gameObject);
            }
            else if (MultiplayerManager.Instance.IsMultiplayer && IsServer)
            {
                this.NetworkObject.Despawn();
                Destroy(this.gameObject);
            }
        }



        // Counts down to despawn
        DecreaseTimer();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Checks each point of contact in the collision
        foreach (ContactPoint contact in collision.contacts)
        {
            // Checks if the contact point is on the sides of the puck
            // and only a vertical wall (protects against slopes)
            if (Vector3.Dot(contact.normal, Vector3.up) < 0.3f)
            {
                // Reflects the direction and increases the bounce count
                direction = Vector3.Reflect(direction, contact.normal);
                bounceCount++;

                // Stops loop at the first side contact point
                break;
            }
        }

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
                if (!MultiplayerManager.Instance.IsMultiplayer)
                {
                    Destroy(this.gameObject);
                }
                else if (MultiplayerManager.Instance.IsMultiplayer && IsServer)
                {
                    this.NetworkObject.Despawn();
                    Destroy(this.gameObject);
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
                if (!MultiplayerManager.Instance.IsMultiplayer)
                {
                    Destroy(this.gameObject);
                }
                else if (MultiplayerManager.Instance.IsMultiplayer && IsServer)
                {
                    this.NetworkObject.Despawn();
                    Destroy(this.gameObject);
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
            if (!MultiplayerManager.Instance.IsMultiplayer)
            {
                Destroy(this.gameObject);
            }
            else if (MultiplayerManager.Instance.IsMultiplayer && IsServer)
            {
                this.NetworkObject.Despawn();
                Destroy(this.gameObject);
            }
        }
    }
}
