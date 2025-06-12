using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : BaseItem
{

   
    // Start is called before the first frame update
    void Start()
    {
        // starts the hazard slightly behind the player
        Vector3 behindPos = transform.position - transform.forward * 6;
        transform.position = behindPos;
        
        // sends the hazard slightly up and behind the player before landing on the ground
        if (isUpgraded)
        {
            this.gameObject.transform.localScale += new Vector3(8.5f, 0.0f, 8.5f);
            rb.AddForce(transform.forward * -750.0f + transform.up * 50.0f);
        }
        else
        {
            rb.AddForce(transform.forward * -750.0f + transform.up * 50.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

}
