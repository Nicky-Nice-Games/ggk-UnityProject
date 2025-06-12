using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class FakeItemBox : BaseItem
{

    [SerializeField]


    // Start is called before the first frame update
    void Start()
    {

        // sends the hazard slightly up and behind the player before landing on the ground
        Transform kartTransform = kart.transform; // assuming BaseItem has `kart` reference set by ItemHolder

        transform.position = kartTransform.position
                             - kartTransform.forward * 30f   // behind the kart
                             + kartTransform.up * 1.0f;       // slightly above ground
        if (isUpgraded)
        {

            Renderer cubeRenderer = this.GetComponent<Renderer>();
            cubeRenderer.material.SetColor("_Color", Color.grey);
            rb = GetComponent<Rigidbody>();
            rb.AddForce(new Vector3(0.0f, 100.0f, -750.0f));
        }
        else
        {
            rb = GetComponent<Rigidbody>();
            //rBody.AddForce(new Vector3(0.0f, 100.0f, -750.0f));
            rb.AddForce(transform.forward * -750.0f + transform.up * 50.0f);
        }

    }

    // Update is called once per frame
    void Update()
    {

        RotateBox();
        // stop the box from falling at the specified height (same as regular item box)
        if (transform.position.y <= kart.transform.position.y + 2.5)
        {
            Destroy(rb);
        }
    }

    private void FixedUpdate()
    {
        DecreaseTimer();
    }

    public void RotateBox()
    {
        transform.rotation *= new Quaternion(0.0f, 2.0f * Time.deltaTime, 0.0f, 1.0f);
    }
}
