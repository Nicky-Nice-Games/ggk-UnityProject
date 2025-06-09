using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : BaseItem
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Counts down to despawn
        DecreaseTimer();

        // Sets shield position to the karts position
        transform.position = new Vector3(kart.transform.position.x, kart.transform.position.y, kart.transform.position.z);
    }

    private void OnTriggerEnter(Collider collision)
    {
        // Destroys any game object or hazard the shield hits
        if (collision.gameObject.CompareTag("Projectile") || collision.gameObject.CompareTag("Hazard"))
        {
            Destroy(collision.gameObject);
        }
    }
}
