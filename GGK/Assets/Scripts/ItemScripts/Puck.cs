using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Puck : BaseItem
{
    private Vector3 direction;                 // The direction and speed the puck travels
    [SerializeField] private int maxBounces;   // The max number the puck can bounce off walls
    private int bounceCount;                   // The times the puck bounced
    private GameObject kartTarget;
    [SerializeField] private NavMeshAgent agent;
    private bool isTracking;
    private bool goStraight;
    private bool isTrackingFirst;

    [SerializeField]
    SpeedCameraEffect cameraScript;   // Camera effect

    // Start is called before the first frame update
    void Start()
    {
        // The puck spawns 15 units in front of the kart
        transform.position = new Vector3(transform.position.x + transform.forward.x * 5f,
                        transform.position.y,
                        transform.position.z + transform.forward.z * 5f);

        // The speed of the puck times 200
        // Keeps the player from hitting it during use regardless of speed

        direction = transform.forward * 120.0f;

        GameObject[] karts = GameObject.FindGameObjectsWithTag("Kart");

        bounceCount = 0;
        agent.speed = 80.0f;

        switch (itemTier)
        {
            case 2:
                useCount = 3;
                break;
            case 3:
                useCount = 1;
                timer = 50;
                FindClosestKart(karts);
                break;
            case 4:
                useCount = 1;
                timer = 50;
                isTrackingFirst = true;
                break;
            default:
                useCount = 1;
                isTrackingFirst = true;
                break;
        }
    }

    void Update()
    {
        // Acts as stronger gravity to bring the puck down
        rb.AddForce(Vector3.down * 40.0f, ForceMode.Acceleration);
    }

    void FixedUpdate()
    {

        if (isTracking)
        {
            agent.SetDestination(kartTarget.transform.position);
        }
        else if (goStraight)
        {
            agent.SetDestination(transform.position + (transform.forward * 3));
        }
        else if (isTrackingFirst)
        {
            agent.SetDestination(FindFirstPlace().position);
        }
        else
        {
            // if (cameraScript.isHoldingTab)
            // {
            //     direction= -1;
            // }

            // Moves puck in a straight line by its forward vector
            // RIGHT NOW UPGRADED FUNCTIONALITY IS NOT IMPLEMENTED
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

        // ELI-ZORD WILL COMMENT THIS LATER

        if (collision.gameObject.CompareTag("Kart"))
        {

            NEWDriver playerKart = collision.transform.root.GetChild(0).GetComponent<NEWDriver>();
            NPCDriver npcKart = collision.gameObject.GetComponent<NPCDriver>();

            if (playerKart)
            {
                playerKart.acceleration = new Vector3(0.0f, 0.0f, 0.0f);
                playerKart.sphere.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            }
            else if (npcKart)
            {
                collision.gameObject.GetComponent<NPCDriver>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
                collision.gameObject.GetComponent<NPCDriver>().StartRecovery();
            }

            Debug.Log(collision.transform.root.gameObject);
            Debug.Log(kartTarget);

            if (isTrackingFirst)
            {
                if (collision.transform.root.gameObject == kartTarget)
                {
                    Destroy(this.gameObject);
                }
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }



    public void FindClosestKart(GameObject[] karts)
    {
        GameObject closestKart = null;
        float baseDistance = Mathf.Infinity;

        foreach (GameObject kart in karts)
        {
            float distance = Vector3.Distance(transform.position, kart.transform.position);
            Vector3 direction = kart.transform.position - transform.position;
            float dp = Vector3.Dot(transform.forward, direction.normalized);

            if (dp > 0)
            {
                if (distance < baseDistance)
                {
                    baseDistance = distance;
                    closestKart = kart;
                }
            }
        }
        if (closestKart)
        {
            kartTarget = closestKart;
            isTracking = true;
        }
        else
        {
            goStraight = true;
        }

    }

    public Transform FindFirstPlace()
    {
        Transform firstPlaceKart = GameObject.Find("PlacementManager").GetComponent<PlacementManager>().sortedList[0].gameObject.transform;
        kartTarget = firstPlaceKart.root.gameObject;
        return firstPlaceKart;
    }
}
