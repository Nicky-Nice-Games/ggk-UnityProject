using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float xRange = 5.0f;
    [SerializeField] private float yRange = 0.0f;
    [SerializeField] private float zRange = 0.0f;
    [SerializeField] private AudioSource AudioClip;
    
    [SerializeField] private bool paused = false;
    [SerializeField] private bool moving = true;
    [SerializeField] private bool slowDown = true;

    private Vector3 startPosition;
    private float timeElapsed;

    private GameObject player;
    void Start()
    {
        startPosition = transform.position;
        player = GameObject.FindWithTag("Kart");

        string newTag = "slowDown";

        //if marked as slow then when the player collides the speed will temporarily change via the playerMove script
        //have to add "slowDown" in the tags within the Unity editor
        if (slowDown)
        {
            gameObject.tag = newTag;
        }
    }

    void Update()
    {
        if (paused) return;

        if (moving)
        {
            timeElapsed += Time.deltaTime * speed;
            float x = startPosition.x + Mathf.Sin(timeElapsed) * xRange;
            float y = startPosition.y + Mathf.Sin(timeElapsed) * yRange;
            float z = startPosition.z + Mathf.Sin(timeElapsed) * zRange;

            transform.position = new Vector3(x, y, z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Kart") && AudioClip != null)
        {
            StartCoroutine(PlayAudioForSeconds(3f)); // Adjust time as needed
        }
        
    }

    private void OnTriggerStay(Collider collision)
    {
        if (slowDown)
        {
            if (collision.gameObject.CompareTag("Kart"))
            {
                float slowFactor = 0.5f;
                Debug.Log("Slowing Down!");

                // gets the parent of the larger collider to get the different child with the driver script
                GameObject gameobj = null;
                NEWDriver driver = null;

                NPCDriver npc = null;
                // check the colliders to see if it's a box (NPC) or sphere (Player)
                if (collision.GetComponent<BoxCollider>() != null)
                {
                    // if there is a boxcollider it's an npc
                    npc = collision.GetComponent<NPCDriver>();
                }
                else if (collision.GetComponent<SphereCollider>() != null)
                {
                    // if there's a sphere collider it's a player
                    gameobj = collision.transform.parent.gameObject;
                    driver = gameobj.GetComponentInChildren<NEWDriver>();
                }


                if (driver != null)
                {
                    // applies a counter force to the kart when driving on the slowdown terrains
                    driver.sphere.AddForce(-driver.acceleration * slowFactor, ForceMode.Acceleration);
                }
                else if (npc != null)
                {
                    // create a Vector3 for the NPC's Acceleration then apply a counter force to slow it down
                    //Vector3 NPCacc = npc.kartModel.forward * (npc.followTarget.position.z - npc.transform.position.z) * (npc.accelerationRate * Time.fixedDeltaTime);
                    //npc.rBody.AddForce(-NPCacc * slowFactor, ForceMode.Acceleration);
                    npc.maxSpeed = 10f;
                }
            }
        }
    }

    private IEnumerator PlayAudioForSeconds(float duration)
    {
        AudioClip.Play();
        yield return new WaitForSeconds(3);
        AudioClip.Stop();
    }
}