using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoostPanel : MonoBehaviour
{
    [SerializeField]
    private float boostForce;

    [SerializeField]
    private float boostTime;

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
            GameObject parent = other.gameObject.transform.parent.gameObject;
            NEWDriver kart = parent.transform.GetChild(0).GetComponent<NEWDriver>();
            StartCoroutine(kart.Boost(boostForce, boostTime));
        }
    }
}
