using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Driver : MonoBehaviour
{
    [SerializeField] private Vector3 acceleration; //How fast karts velocity changes
    [SerializeField] private Vector3 velocity; //How fast kart is moving

    //acceleration, decceleration
    [SerializeField] private float accelerationRate, deccelerationRate;
    [SerializeField] private float minSpeed, maxSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private Rigidbody rBody;

    //For input
    [SerializeField] private Vector3 movementDirection;

    //Fields For Turning
    [SerializeField] private Quaternion turning;

    //To determine drifting stuff in update
    bool isDrifting;
    float driftTime = 0f;
    [SerializeField] private float driftFactor = 0.8f;
    [SerializeField] private float driftTurnMultiplier = 1.5f;
    [SerializeField] private float minDriftTime = 1f;
    [SerializeField] private float driftBoostForce = 100f;

    [SerializeField] private float hopForce = 8f;


    // Start is called before the first frame update
    void Start()
    {
        /*
        rBody.drag = 0.5f;
        velocity = Vector3.zero;
        rBody.freezeRotation = true;

        accelerationRate = 3500f;
        deccelerationRate = 1f;
        minSpeed = 1;
        maxSpeed = 150;
        turnSpeed = 60;
        */

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (movementDirection.z != 0f)
        {
            acceleration = transform.forward * (movementDirection.z * accelerationRate * Time.deltaTime);

            velocity += acceleration * Time.fixedDeltaTime;

            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        }
        else
        {
            velocity *= 1f - (deccelerationRate * Time.fixedDeltaTime);

            if (velocity.magnitude < minSpeed)
            {
                velocity = Vector3.zero;
            }
        }

        //Turning stuff
        float backwardsCheck = Vector3.Dot(transform.forward, velocity);

        float newTurnSpeed = isDrifting ? turnSpeed * driftTurnMultiplier : turnSpeed;


        if (!(velocity == Vector3.zero))
        {
            if (backwardsCheck < 0)
            {
                turning = Quaternion.Euler(0f, -(movementDirection.x * newTurnSpeed * Time.fixedDeltaTime), 0f);

            }
            else
            {

                turning = Quaternion.Euler(0f, movementDirection.x * newTurnSpeed * Time.fixedDeltaTime, 0f);

            }
            velocity = turning * velocity;
        }

        else
        {
            turning = Quaternion.Euler(0f, 0f, 0f);

        }

        //drifting
        if (isDrifting)
        {
            Drift();
        }


        rBody.Move(transform.position + velocity * Time.fixedDeltaTime, transform.rotation * turning);
    }

    /// <summary>
    /// Begin to make an attempt at drifting
    /// If the conditions for drifiting are met,
    /// you'll move to the next drifting script
    /// </summary>
    public void AttemptDrift()
    {
        if (Mathf.Abs(movementDirection.x) > 0.1f)
        {
            rBody.AddForce(Vector3.up * hopForce, ForceMode.Impulse);

            isDrifting = true;
        }
    }

    /// <summary>
    /// Begin the drifting script once the conditions are met.
    /// Allows the kart to hit a sharper angle when turning
    /// </summary>
    public void Drift()
    {
        driftTime += Time.deltaTime;

        if (rBody.velocity.magnitude < 0.1f) return;

        Vector3 driftVelocity = transform.InverseTransformDirection(rBody.velocity);
        driftVelocity.x *= driftFactor;
        rBody.velocity = transform.TransformDirection(driftVelocity);

    }

    /// <summary>
    /// Ends the specific drift state.
    /// Provides a boost after drifting for a short time
    /// </summary>
    public void EndDrift()
    {
        if (!isDrifting) return;
        if (driftTime > minDriftTime)
        {
            Vector3 boost = transform.forward * driftBoostForce;
            velocity += boost * 0.3f;
            rBody.AddForce(boost, ForceMode.VelocityChange);
        }

        isDrifting = false;
        driftTime = 0f;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();

        movementDirection.z = movementDirection.y;

        movementDirection.y = 0; //We are not gonna jump duh
    }

    public void OnDrift(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            AttemptDrift();
        }
        else if (context.canceled)
        {
            EndDrift();
        }
    }

    public void OnAcceleration(InputAction.CallbackContext context)
    {
        movementDirection.z = context.ReadValue<float>();
    }

    public void OnTurn(InputAction.CallbackContext context)
    {
        movementDirection.x = context.ReadValue<float>();
    }
}

