using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDimensionalAnimController : MonoBehaviour
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
        turningValue = Mathf.Lerp(turningValue, driver.movementDirection.x, Time.deltaTime * lerpSpeed);


        //Smoothly interpolating input   
        animator.SetFloat("turningValue", turningValue);
    }

}
