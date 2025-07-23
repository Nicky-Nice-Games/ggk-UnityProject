using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHandler : MonoBehaviour
{
    Rigidbody rb;

    [HideInInspector]
    public Transform spawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(PositionCorrector());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator PositionCorrector()
    {
        rb.isKinematic = true;
        for (int i = 0; i < 20; i++)
        {
            if (rb.transform != spawnPoint)
            {
                rb.transform.position = spawnPoint.position;
                gameObject.transform.position = spawnPoint.position;

            }

            yield return new WaitForEndOfFrame();
        }

        
        rb.isKinematic = false;


    }
}
