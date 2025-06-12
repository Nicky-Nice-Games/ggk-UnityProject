using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CloseOpenGate : MonoBehaviour
{
    //number of laps needed before the gate "unlocks"
    [SerializeField] private int lapsNeeded = 1;
    private int laps = 0;
    
    //private Material Material1;
    //private Rigidbody Rigidbody1;

    //in the editor this is what you would set as the object you want to change
    //[SerializeField] private GameObject Gate;


    // Start is called before the first frame update
    void Start()
    {
        //Gate.GetComponent<MeshRenderer>().material = Material1;

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            laps++;
            Debug.Log("laps: " + laps);
            Debug.Log("up key pressed");
        }

        if (laps >= lapsNeeded)
        {
            Destroy(gameObject);
        }
    }

    //can uncomment this for like a "break through" effect
    /*
    void OnCollisionEnter(Collision collision)
    {
        if(laps >= lapsNeeded)
        {
            Destroy(Gate);
        }
    }
    */

}
