using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using static UnityEngine.GraphicsBuffer;

public class Pathing : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform followTarget;
    float followDistance = 2f;
    Vector3 distance;
    float movementSpeed = 25f;
    float smoothTime = 0.2f;
    float roll;

    [SerializeField] bool drivable;
    bool wasDrivable = true;
    [SerializeField] bool returningToTarget = false;

    float blendTimer = 0f;
    float blendDuration = 0.5f; // Adjust for smoother/slower transition

    Vector3 velocity;
    void Start()
    {
        drivable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (drivable)
        {
            if (!wasDrivable)
            {
                velocity = Vector3.zero;
                wasDrivable = true;
                returningToTarget = true;
                
            }

            followTrack();

            if (!returningToTarget) 
            {
                followTarget.GetComponent<SplineAnimate>().enabled = true;
            }
        }
        else
        {
            if (wasDrivable)
            {
                followTarget.GetComponent<SplineAnimate>().enabled = false;
                wasDrivable = false;
            }
        }
    }

    private void followTrack()
    {
        if (returningToTarget)
        {
            transform.position = Vector3.MoveTowards(transform.position, followTarget.position, movementSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(followTarget.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180f * Time.deltaTime);

            if (Vector3.Distance(transform.position, followTarget.position) < 0.5f)
            {
                returningToTarget = false;
                blendTimer = blendDuration; // Start blending phase
            }
        }
        else if (blendTimer > 0)
        {
            // Blend phase: still smoothing movement & rotation
            transform.position = Vector3.Lerp(transform.position, followTarget.position, Time.deltaTime * movementSpeed);

            Quaternion targetRotation = followTarget.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            blendTimer -= Time.deltaTime;
        }
        else
        {
            // Fully synced mode
            transform.position = Vector3.Lerp(transform.position, followTarget.position, Time.deltaTime * movementSpeed);
            transform.rotation = followTarget.rotation;
        }
    }
}
