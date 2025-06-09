using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoostPanel : MonoBehaviour
{
    [SerializeField]
    private float boostForce;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Kart"))
        {
            StartCoroutine(Boost(boostForce, 1.5f, other));
        }
    }


    IEnumerator Boost(float boostForce, float duration, Collider rBody)
    {
        Rigidbody kart = rBody.gameObject.GetComponent<Rigidbody>();
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            Vector3 boostDirection = kart.gameObject.transform.forward * boostForce;

            kart.AddForce(boostDirection, ForceMode.VelocityChange);
            yield return new WaitForFixedUpdate();
        }
    }
}
