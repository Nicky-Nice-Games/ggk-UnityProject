using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Puck : BaseItem
{
    private Vector3 direction;                     // The direction and speed the puck travels
    [SerializeField] private int maxBounces;       // The max number the puck can bounce off walls
    private int bounceCount;                       // The times the puck bounced
    private GameObject kartTarget;                 // The kart the puck tracks to
    [SerializeField] private NavMeshAgent agent;   // The AI component of the puck
    private bool isTracking;                       // If the puck should track
    private bool goStraight;                       // If the puck goes straight
    private bool isTrackingFirst;                  // If the puck should track to first
    public GameObject[] karts;

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


            karts = GameObject.FindGameObjectsWithTag("Kart");

            // Starts the puck with 0 bounces
            bounceCount = 0;

            // Tracks the item tier
            switch (itemTier)
            {
                // Multi-puck (3 uses)
                case 2:
                    useCount = 1;
                    timer = 50;
                    FindClosestKart(karts);
                    break;
                // Puck tracks to the closest player and lasts longer
                case 3:
                    useCount = 3;
                    timer = 50;
                    FindClosestKart(karts);
                    break;
                // Puck tracks to first place
                case 4:
                    useCount = 1;
                    timer = 50;
                    isTrackingFirst = true;
                    break;
                // Normal puck, one use
                default:
                    useCount = 1;
                    break;
            }
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

            currentPos = new NetworkVariable<Vector3>(transform.position);
        }
    }

    void Update()
    {
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
        // Moves the puck towards the closest kart
        if (isTracking)
        {
            agent.SetDestination(kartTarget.transform.position);
        }
        // Moves the puck straight normally but still follows pathing
        else if (goStraight)
        {
            agent.SetDestination(transform.position + (transform.forward * 3));
        }
        // Puck moves toward the player in first place
        else if (isTrackingFirst)
        {
            agent.SetDestination(FindFirstPlace().position);
        }
        // Moves puck forward
        else
        {
            // if (cameraScript.isHoldingTab)
            // {
            //     direction= -1;
            // }

            agent.enabled = false;

            // Moves the kart forward at its normal speed
            if (itemTier > 1)
            {
                rb.velocity = direction;
            }
            else
            {
                rb.velocity = direction;
            }

            // Destroys puck if it bounced enough times
            if (bounceCount == maxBounces + 1)
            {
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
            if (isTrackingFirst)
            {
                if (collision.transform.root.gameObject == kartTarget)
                {
                    Destroy(this.gameObject);
                }
            }
            // Otherwise destroys puck regardless of kart hit
            else
            {
                Destroy(this.gameObject);
            }
        }
        // Pucks can destroy other pucks
        else if (collision.gameObject.CompareTag("Projectile"))
        {
            // Prevents puck from destroying tier 4 puck
            if (collision.gameObject.GetComponent<Puck>().itemTier != 4)
            {
                Destroy(collision.gameObject);
                Destroy(this.gameObject);
            }
        }

    }

    // If colliding with cracked brick wall
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<TrapItem>() && other.gameObject.GetComponent<TrapItem>().ItemTier == 2)
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
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
            isTracking = true;
        }
        // If there is no kart in front of user,
        // then puck goes straight
        else
        {
            goStraight = true;
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
