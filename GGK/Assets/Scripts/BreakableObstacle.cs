using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObstacle : MonoBehaviour
{
    // the time it takes the obstacle to respawn
    [SerializeField]
    private float respawnTimer;

    private bool active = true;

    private BoxCollider boxCollider;
    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        respawnTimer = 5.0f;
        boxCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // if the obstacle is inactive, decrease the timer
        if (!active)
        {
            DecreaseTimer();
        }

        if(respawnTimer <= 0)
        {
            // put the obstacle back as active after the timer goes down
            ToggleActivation();
            respawnTimer = 5.0f;
        }
    }

    public void DecreaseTimer()
    {
        // decrease the timer to 0
        if(respawnTimer > 0)
        {
            respawnTimer -= 1.0f * Time.deltaTime;
        }
    }

    // toggle if the obstacle is activated or not
    public void ToggleActivation()
    {
        boxCollider.enabled = !boxCollider.enabled;
        meshRenderer.enabled = !meshRenderer.enabled;
        if(boxCollider.enabled == true || meshRenderer.enabled == true)
        {
            active = true;
        }
        else
        {
            active = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // checks if the tag is projectile
        if(collision.gameObject.tag == "Projectile")
        {
            // remove the collider and renderer of the obstacle when it's broken
            ToggleActivation();
        }
    }
}
