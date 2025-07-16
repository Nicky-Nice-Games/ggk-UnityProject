using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Unity.Netcode;

public class TwoDimensionalAnimController : NetworkBehaviour
{
    Animator animator;
    float turningValue = 0;
    public NEWDriver driver;
    public float lerpSpeed;
    // Start is called before the first frame update
    void Start()
    {
        //Getting our animator :D
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        turningValue = Mathf.Lerp(turningValue, driver.movementDirection.x, Time.deltaTime * lerpSpeed);


        //Smoothly interpolating input   
        animator.SetFloat("turningValue", turningValue);
    }

}
