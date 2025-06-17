using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DynamicRecovery : MonoBehaviour
{
    public Transform respawnPoint;
    public float hoverHeight = 3f;
    public float hoverTime = 2f;
    public float fadeTime = 1f;
    public CanvasGroup fadeCanvas;

    private Transform[] checkpoints;
    private Transform currentCheckpoint;

    private Rigidbody rb;
    private bool isRecovering = false;

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

    public void StartRecovery()
    {
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

        // Teleport above checkpoint
        Vector3 hoverPos = currentCheckpoint.position + Vector3.up * hoverHeight;
        transform.position = hoverPos;
        rb.useGravity = false;

        yield return new WaitForSeconds(hoverTime);


        //fade back in
        yield return StartCoroutine(Fade(0));

        rb.velocity = new Vector3(0, -10f, 0);
        rb.angularVelocity = Vector3.zero;
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
