using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DynamicRecovery : MonoBehaviour
{
    //public Transform respawnPoint;
    public float hoverHeight = 1.4f;
    public float hoverTime = 2f;
    public float fadeTime = 1f;
    public CanvasGroup fadeCanvas;

    private Transform[] checkpoints;
    private Transform currentCheckpoint;

    private Rigidbody rb;
    private bool isRecovering = false;

    public Transform kartModel;

    public MiniMapHud miniMap;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        GameObject[] checkpointObjects = GameObject.FindGameObjectsWithTag("Checkpoint");
        List<Transform> checkpointList = new List<Transform>();

        foreach (GameObject obj in checkpointObjects)
        {
            checkpointList.Add(obj.transform);
        }

        checkpoints = checkpointList.ToArray();

        // Optional: default to first
        if (checkpoints.Length > 0)
            currentCheckpoint = checkpoints[0];
    }

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.red); // your kart
        if (currentCheckpoint != null)
            Debug.DrawRay(currentCheckpoint.position, currentCheckpoint.forward * 5, Color.green); // the checkpoint
    }


    public void StartRecovery()
    {
        Debug.LogError(miniMap);
        if (miniMap)
        {
            //spins the player's icon if they need to be recovered
            StartCoroutine(miniMap.SpinIcon(transform.parent.GetComponentInChildren<ItemHolder>().gameObject, 5));
        }

        if (!isRecovering)
        {
            currentCheckpoint = FindClosestCheckpoint();
            StartCoroutine(Recover());
        }
    }

    private Transform FindClosestCheckpoint()
    {
        Transform closest = null;
        float closestDistSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Transform checkpoint in checkpoints)
        {
            float distSqr = (checkpoint.position - currentPosition).sqrMagnitude;
            if (distSqr < closestDistSqr)
            {
                closestDistSqr = distSqr;
                closest = checkpoint;
            }
        }

        return closest;

    }

    private IEnumerator Recover()
    {
        isRecovering = true;

        //fade to black
        yield return StartCoroutine(Fade(1));

        // Teleport at checkpoint
        Vector3 hoverPos = currentCheckpoint.position + Vector3.up * hoverHeight;
        transform.position = hoverPos;

        rb.velocity = Vector3.zero; // Reset first
        rb.angularVelocity = Vector3.zero;


        //face forward
        kartModel.rotation = currentCheckpoint.rotation;

        rb.useGravity = false;

        yield return new WaitForSeconds(hoverTime);


        //fade back in
        yield return StartCoroutine(Fade(0));

        //rb.velocity = new Vector3(0, -10f, 0);
        rb.AddForce(Vector3.down * 10f, ForceMode.VelocityChange);

        rb.useGravity = true;
        isRecovering = false;


    }

    private IEnumerator Fade(float targetAlpha)
    {
        float initialAlpha = fadeCanvas.alpha;
        float elapsed = 0f;

        while(elapsed < fadeTime)
        {
            fadeCanvas.alpha = Mathf.Lerp(initialAlpha, targetAlpha, elapsed/fadeTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        fadeCanvas.alpha = targetAlpha;
    }
}
