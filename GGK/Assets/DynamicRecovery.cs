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
    private GameObject[] checkpoints;
    public KartCheckpoint kartCheckpointScript;
    private Transform currentCheckpoint;
    [SerializeField] private Transform normalTransform;

    private Rigidbody rb;
    private bool isRecovering = false;

    private ParticleSystem[] particleSystem;

    private NEWDriver kartMovementScript;

    [SerializeField]
    private NPCPhysics kartPhysicsNPC;

    public MiniMapHud miniMap;

    Transform kartVisual;
    Vector3 originalScale;
    Vector3 stretchedScale;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        
        //GameObject[] checkpointObjects = GameObject.FindGameObjectsWithTag("Checkpoint");
        //List<Transform> checkpointList = new List<Transform>();
        //
        //foreach (GameObject obj in checkpointObjects)
        //{
        //    checkpointList.Add(obj.transform);
        //}
        //
        //checkpoints = checkpointList.ToArray();

        //checkpoints = kartCheckpointScript.checkpointList.ToArray();
        //
        //// Optional: default to first
        //if (checkpoints.Length > 0)
        //    currentCheckpoint = checkpoints[0].transform;
        
        Transform kartRoot = transform.parent; // "Kart 1"
        Transform normal = kartRoot.GetChild(0);

        if (normal != null)
        {
            normalTransform = normal;
        }

        kartMovementScript = kartRoot.GetComponentInChildren<NEWDriver>();
        kartPhysicsNPC = kartRoot.GetComponentInChildren<NPCPhysics>();
        particleSystem = kartModel.GetComponentsInChildren<ParticleSystem>(true);


        kartVisual = kartModel;
        originalScale = kartVisual.localScale;
        stretchedScale = new Vector3(originalScale.x * 0.2f, originalScale.y * 2.5f, originalScale.z * 0.2f);

    }

    void Update()
    {
        if(checkpoints == null || checkpoints.Length == 0)
        {
            checkpoints = kartCheckpointScript.checkpointList.ToArray();            
        }

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
        if (miniMap)
        {
            //spins the player's icon if they need to be recovered
            StartCoroutine(miniMap.SpinIcon(transform.parent.GetComponentInChildren<ItemHolder>().gameObject, 5));
        }

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
        //float closestDistSqr = Mathf.Infinity;
        //Vector3 currentPosition = transform.position;
        //
        //foreach (GameObject checkpoint in checkpoints)
        //{
        //    //float distSqr = (checkpoint.position - currentPosition).sqrMagnitude;
        //    //if (distSqr < closestDistSqr)
        //    //{
        //    //    closestDistSqr = distSqr;
        //    //    closest = checkpoint;
        //    //}
        //
        //    
        //}

        closest = checkpoints[kartCheckpointScript.checkpointId].transform; // Default to first checkpoint
        return closest;

    }

    private IEnumerator Recover()
    {
        isRecovering = true;

        //fade to black
        yield return StartCoroutine(Fade(1));

        // Teleport at checkpoint
        Vector3 hoverPos = currentCheckpoint.position + Vector3.up * hoverHeight;
        rb.position = hoverPos;

        rb.velocity = Vector3.zero; // Reset first
        rb.angularVelocity = Vector3.zero;


        //face forward
        rb.rotation = currentCheckpoint.rotation;
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
        //Making rigidbody kinematic
        rb.isKinematic = true;

        Quaternion finalRot = Quaternion.Euler(0, targetRotation.eulerAngles.y - 90, 0);

        if (kartMovementScript != null)
        {
            kartMovementScript.enabled = false;
            
        }

        if (kartPhysicsNPC != null)
        {
            kartPhysicsNPC.enabled = false;
        }
        yield return new WaitForSeconds(1f);

        isRecovering = true;

        

        float duration = 0.3f;

        // STRETCH UP & FADE TO BLACK
        float t = 0f;
        while (t < duration)
        {
            float lerp = t / duration;
            kartVisual.localScale = Vector3.Lerp(originalScale, stretchedScale, lerp);
            if (fadeCanvas != null)
            {
                fadeCanvas.alpha = lerp; // screen fading to black
            }
            t += Time.deltaTime;
            yield return null;
        }

        kartVisual.localScale = stretchedScale;

        if (fadeCanvas != null)
        {

            fadeCanvas.alpha = 1f;
        }

        // DISAPPEAR
        kartVisual.gameObject.SetActive(false);



        // TELEPORT TO NEW POSITION 
        Vector3 drivingDirection = -(targetRotation * Vector3.right); // opposite of red arrow
        Vector3 spawnOffset = drivingDirection.normalized * 8f;
        Vector3 spawnPos = targetPosition + spawnOffset;

        rb.position = spawnPos;
        normalTransform.rotation = Quaternion.LookRotation(drivingDirection, Vector3.up);
        rb.isKinematic = false;
        ResetParticles();
        
        normalTransform.rotation = finalRot;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;

        if(kartMovementScript != null)
        {
            kartMovementScript.Recover();
        }


        yield return new WaitForSeconds(1.3f); // hold black screen for effect

        if (kartMovementScript != null)
        {
            kartMovementScript.enabled = true;
        }

        if (kartPhysicsNPC != null) 
        {
            kartPhysicsNPC.enabled = true;
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
            if(fadeCanvas != null)
            {

                fadeCanvas.alpha = 1f - lerp; // screen fading from black
            }
            t += Time.deltaTime;
            yield return null;
        }

        if (kartMovementScript != null)
        {
            kartMovementScript.StopParticles();
        }
        else if(kartPhysicsNPC != null)
        {
            kartPhysicsNPC.StopParticles();
        }
        kartVisual.localScale = originalScale;

        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 0f;
        }

        rb.useGravity = true;
        isRecovering = false;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeCanvas != null) 
        {
            float initialAlpha = fadeCanvas.alpha;
            float elapsed = 0f;

            while (elapsed < fadeTime)
            {
                fadeCanvas.alpha = Mathf.Lerp(initialAlpha, targetAlpha, elapsed / fadeTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            fadeCanvas.alpha = targetAlpha;
        }
        
    }
}
