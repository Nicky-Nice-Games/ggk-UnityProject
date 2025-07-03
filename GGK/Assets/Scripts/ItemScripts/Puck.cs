using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puck : BaseItem
{
    private Vector3 direction;                 // The direction and speed the puck travels
    [SerializeField] private int maxBounces;   // The max number the puck can bounce off walls
    private int bounceCount;                   // The times the puck bounced

    [SerializeField]
    SpeedCameraEffect cameraScript;   // Camera effect

    // Start is called before the first frame update
    void Start()
    {
        // The puck spawns 15 units in front of the kart
        transform.position = new Vector3(transform.position.x + transform.forward.x * 15f,
                        transform.position.y,
                        transform.position.z + transform.forward.z * 15f);

        // The speed of the puck times 200
        // Keeps the player from hitting it during use regardless of speed

        direction = transform.forward * 280.0f;

        bounceCount = 0;

        switch (itemTier)
        {
            case 2:
                useCount = 3;
                break;
            case 3:
                break;
            case 4:
                break;
            default:
                useCount = 1;
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
    }
}
