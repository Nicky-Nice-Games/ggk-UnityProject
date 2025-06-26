using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DynamicRecovery : MonoBehaviour
{
    //public Transform respawnPoint;
    public float hoverHeight = 1.4f;
    public float hoverTime = 2f;
    public float fadeTime = 2f;
    public CanvasGroup fadeCanvas;

    public Transform kartModel;
    private Transform[] checkpoints;
    private Transform currentCheckpoint;
    private Transform normalTransform;

    private Rigidbody rb;
    private bool isRecovering = false;

    private ParticleSystem[] particleSystem;

    private NEWDriver kartMovementScript;


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

        Transform kartRoot = transform.parent; // "Kart 1"
        Transform normal = kartRoot.Find("Kart");

        if (normal != null)
        {
            normalTransform = normal;
        }

        kartMovementScript = kartRoot.GetComponentInChildren<NEWDriver>();
        particleSystem = kartModel.GetComponentsInChildren<ParticleSystem>(true);

    }

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.red); // your kart
        if (currentCheckpoint != null)
            Debug.DrawRay(currentCheckpoint.position, currentCheckpoint.forward * 5, Color.green); // the checkpoint
    }

    private void ResetParticles()
    {
        foreach (ParticleSystem ps in particleSystem)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }



    public void StartRecovery()
    {
        if (!isRecovering)
        {
            currentCheckpoint = FindClosestCheckpoint();
            //StartCoroutine(Recover());
            StartCoroutine(Teleport(currentCheckpoint.position, currentCheckpoint.rotation));

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
        transform.rotation = currentCheckpoint.rotation;
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

    private IEnumerator Teleport(Vector3 targetPosition, Quaternion targetRotation)
    { 

        if(kartMovementScript != null)
        {
            kartMovementScript.enabled = false;
            
        }
        yield return new WaitForSeconds(1f);

        isRecovering = true;

        Transform kartVisual = kartModel;
        Vector3 originalScale = kartVisual.localScale;
        Vector3 stretchedScale = new Vector3(originalScale.x * 0.2f, originalScale.y * 2.5f, originalScale.z * 0.2f);

        float duration = 0.3f;

        // STRETCH UP & FADE TO BLACK
        float t = 0f;
        while (t < duration)
        {
            float lerp = t / duration;
            kartVisual.localScale = Vector3.Lerp(originalScale, stretchedScale, lerp);
            fadeCanvas.alpha = lerp; // screen fading to black
            t += Time.deltaTime;
            yield return null;
        }

        kartVisual.localScale = stretchedScale;
        fadeCanvas.alpha = 1f;

        // DISAPPEAR
        kartVisual.gameObject.SetActive(false);

        // TELEPORT TO NEW POSITION 
        transform.position = targetPosition;
        ResetParticles();
        normalTransform.rotation = targetRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;

        yield return new WaitForSeconds(1.3f); // hold black screen for effect

        if (kartMovementScript != null)
        {
            kartMovementScript.enabled = true;
        }

        // REAPPEAR IN STRETCHED FORM
        kartVisual.localScale = stretchedScale;
        kartVisual.gameObject.SetActive(true);

        // SQUASH BACK & FADE IN 
        t = 0f;
        while (t < duration)
        {
            float lerp = t / duration;
            kartVisual.localScale = Vector3.Lerp(stretchedScale, originalScale, lerp);
            fadeCanvas.alpha = 1f - lerp; // screen fading from black
            t += Time.deltaTime;
            yield return null;
        }

        kartMovementScript.StopParticles();
        kartVisual.localScale = originalScale;
        fadeCanvas.alpha = 0f;

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
