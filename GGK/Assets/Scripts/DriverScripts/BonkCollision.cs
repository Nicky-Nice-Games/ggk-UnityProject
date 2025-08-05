using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonkCollision : MonoBehaviour
{
    public float bonkForce;
    public Rigidbody sphere;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        sphere.AddForce(collision.contacts[0].normal * bonkForce, ForceMode.Impulse);

        GameObject kart = sphere.transform.parent.transform.GetChild(0).gameObject;

        GameObject collisionParent = collision.transform.parent.gameObject;

        // track # of collisions with walls for playerinfo

        Debug.Log("Collision with wall");
        kart.GetComponent<NEWDriver>().playerInfo.collisionWithWalls++;
    }
}
