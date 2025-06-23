using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goody : MonoBehaviour
{
    //i'm dumb the counter would have to go in a separate manger object/script
    //[SerializeField] int goodyCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        //goodyCounter++;
        Destroy(gameObject);
        Debug.Log("goody hit!");
        //Debug.Log("Goody Counter: " + goodyCounter);
        Debug.Log("play sound?");

        /*
        if (goodyCounter >= 10)
        {
            Debug.Log("MK rules cap it at ten");
            goodyCounter = 10;
        }
        */
    }
}
