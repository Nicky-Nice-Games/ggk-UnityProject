using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnHandler : NetworkBehaviour
{
    private Rigidbody rb;

    public NetworkVariable<int> spawnIndex = new NetworkVariable<int>(-1); // Default to -1

    public List<Transform> spawnPoints; // You must assign this list elsewhere (e.g., in the scene or via manager)

    void Start()
    {
        //transform.parent.position = spawnPoints[spawnIndex.Value].position;
        //transform.parent.rotation = spawnPoints[spawnIndex.Value].rotation;
        //rb = GetComponent<Rigidbody>();
        //StartCoroutine(PositionCorrector());
    }


    public override void OnNetworkSpawn()
    {
        transform.parent.position = spawnPoints[spawnIndex.Value].position;
        transform.parent.rotation = spawnPoints[spawnIndex.Value].rotation;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(PositionCorrector());
    }

    private IEnumerator PositionCorrector()
    {
        // Wait until we get a valid spawn index
        while (spawnIndex.Value < 0 || spawnIndex.Value >= spawnPoints.Count)
        {
            spawnIndex.Value = 0; // Default to the first spawn point if invalid
            Debug.Log("SPAWNINDEX IS INVALID, SETTING TO 0");
            yield return new WaitForEndOfFrame();


        }

        // Temporarily disable physics to snap to position
        rb.isKinematic = true;

        for (int i = 0; i < 20; i++)
        {
            Vector3 targetPos = spawnPoints[spawnIndex.Value].position;
            Quaternion targetRot = spawnPoints[spawnIndex.Value].rotation;

            // Snap position and rotation
            rb.transform.SetPositionAndRotation(targetPos, targetRot);
            yield return new WaitForEndOfFrame();
        }

        rb.isKinematic = false;
    }
}
