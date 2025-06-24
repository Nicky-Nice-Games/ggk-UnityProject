using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NEWDriver : MonoBehaviour
{
    // Keep
    [Header("Do not Change")]
    public Vector3 acceleration; //How fast karts velocity changes        
    public Vector3 movementDirection;
    public Quaternion turning;


    [Header("Kart Settings")]
    //acceleration, decceleration
    public float accelerationRate, deccelerationRate, airDeccelerationRate;
    public float minSpeed = 5f;
    public float maxSpeed = 60f;
    public float turnSpeed = 40;   
    public float maxSteerAngle = 20f; //Multiplier for wheel turning speed    
    public Transform kartNormal;
    public float gravity = 20;
    public float tractionCoefficient = 6f;
    float controllerX;
    float controllerZ;
    

    [Header("Sphere Collider stuff")]
    public float colliderOffset = 1.69f; //Offset for the sphere collider to position kart correctly
    public Transform spherePosTransform; //Reference to the sphere collider transform
    public Rigidbody sphere;

    [Header("Drift Settings")]
    //To determine drifting stuff in update
    public bool isDrifting;
    bool driftMethodCaller = false;
    bool turboTwisting;
    public float turboTwistWindow;
    float driftTime = 0f;
    public float driftFactor = 2f;
    public float driftTurnMultiplier = 1.5f;
    public float driftFowardCompensation = 1.2f;
    public float minDriftTime = 200f;
    public float driftBoostForce = 1f;
    public float hopForce = 8f;
    public float minDriftSteer = 40f;
    public float boostMaxSpeed = 55f;
    public float driftMaxSpeed = 34f; //Maximum speed when drifting

    //To determine drifting direction
    bool isDriftingLeft;

    [Header("Wheel references")]
    //Front tires GO
    public GameObject frontTireR;
    
    public GameObject frontTireL;
    public GameObject steeringWheel;
    

    [Header("Reference to the kartModel transform for Animation")]
    public Transform kartModel;

    [Header("Particle Effects")]
    public List<ParticleSystem> particleSystemsBR;
    public List<ParticleSystem> particleSystemsBL;
    public List<ParticleSystem> TireScreechesLtoR;
    public List<ParticleSystem> transitionSparksLtoR;
    public List<Color> turboColors;
    public ParticleSystem driftSparksLeftBack;
    public ParticleSystem driftSparksLeftFront;
    public ParticleSystem driftSparksRightFront;
    public ParticleSystem driftSparksRightBack;
    public List<ParticleSystem> boostFlames;
    int driftTier;
    int currentDriftTier = 0; //To check if we are in the same drift tier or not, so we can change the color of the particles accordingly


    [Header("Raycast Settings")]
    public LayerMask groundLayer;        
    // Ground snapping variables
    public bool isGrounded;
    bool attemptingDrift;
    float airTime;
    public float groundCheckDistance = 1.05f;    
    public float rotationAlignSpeed = 0.05f;
    public float horizontalOffset = 0.2f; // Horizontal offset for ground check raycast

    //Tween stuff
    Tween driftRotationTween;
    Tween driftPositionTween;
    float driftVisualAngle = 10f;
    float driftTweenDuration = 0.4f;


    [Header("Sound Settings")]

    AudioSource soundPlayer;

    [SerializeField]
    AudioClip driveSound;

    public bool isDriving;

    private uint drivingSoundID;
    private uint driftingSoundID;

    // Start is called before the first frame update
    void Start()
    {
        sphere.drag = 0.5f;

        //-------------Particles----------------
        foreach (ParticleSystem ps in particleSystemsBR)
        {
            ps.Stop();
        }
        foreach (ParticleSystem ps in particleSystemsBL)
        {
            ps.Stop();
        }
        foreach (ParticleSystem ps in TireScreechesLtoR)
        {
            ps.Stop();
        }
        foreach (ParticleSystem ps in transitionSparksLtoR)
        {
            ps.Stop();
        }
        foreach (ParticleSystem ps in boostFlames)
        {
            ps.Stop();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleGroundCheck();
        ApplyWheelVisuals();

        //Follow Collider
        transform.position = 
            new Vector3(spherePosTransform.transform.position.x, 
            spherePosTransform.transform.position.y - colliderOffset, 
            spherePosTransform.transform.position.z);

        if (isDriving && drivingSoundID == 0)
        {
            drivingSoundID = AkUnitySoundEngine.PostEvent("Play_engine_high_full", this.gameObject);
        }
        else if (!isDriving && drivingSoundID != 0)
        {
            AkUnitySoundEngine.StopPlayingID(drivingSoundID, 500, AkCurveInterpolation.AkCurveInterpolation_Log1);
            drivingSoundID = 0;
        }


        switch(isDrifting)
        {
            case (true):
                {
                    switch (driftingSoundID)
                    {
                        case 0:
                            {
                                if (isGrounded)
                                {
                                    driftingSoundID = AkUnitySoundEngine.PostEvent("Play_driftScreech", this.gameObject);
                                }
                                break;
                            }

                        default:
                            {
                                if (!isGrounded)
                                {
                                    AkUnitySoundEngine.StopPlayingID(driftingSoundID, 50, AkCurveInterpolation.AkCurveInterpolation_Log3);
                                    driftingSoundID = 0;
                                }
                                break;
                            }
                    }
                    break;
                }

            case (false):
                {
                    AkUnitySoundEngine.StopPlayingID(driftingSoundID, 250, AkCurveInterpolation.AkCurveInterpolation_Log3);
                    driftingSoundID = 0;
                    break;
                }
        }




        //------------Movement stuff---------------------

        //Acceleration
        if (movementDirection.z != 0f && isGrounded)
        {              
            //Setting acceleration 
            if((sphere.velocity.magnitude > maxSpeed ) || (isDrifting && sphere.velocity.magnitude > driftMaxSpeed))
            {
                acceleration = Vector3.zero; //If we are going too fast, stop accelerating
            }
            else
            {
                acceleration = kartModel.forward * movementDirection.z * accelerationRate * Time.deltaTime;
            }
            
        }
        else if(isGrounded)
        {
            //Decceleration
            acceleration *= 1f - (deccelerationRate * Time.fixedDeltaTime);

            //Stop the vehicle once we reach a certain minimum speed
            if (sphere.velocity.magnitude < minSpeed)
            {
                sphere.velocity = Vector3.zero;
                acceleration = Vector3.zero;
            }
        }
        else
        {
            //In the air, decelerating bc of drag
            acceleration *= 1f - (airDeccelerationRate * Time.fixedDeltaTime);
        }



        //------------Turning stuff---------------------

        //to check if we are going backwards...
        float backwardsCheck = Vector3.Dot(transform.forward, sphere.velocity);

        //Applies a turn multiplier when drifting
        float driftTurnSpeed = isDrifting ? turnSpeed * driftTurnMultiplier : turnSpeed;

        //If we are not stationary
        if (!(sphere.velocity == Vector3.zero))
        {
            //Calculate the turning direction based on the movement direction input and multiply it by our turn speed
            float turningDirection = movementDirection.x * driftTurnSpeed;

            //drifting
            if (driftMethodCaller)
            {
                //Keep drifting
                Drift();

                //Recalculate our turningDirection value bc of drifting
                turningDirection = movementDirection.x * driftTurnSpeed;

                // turn influence to apply to turning variable
                turningDirection += isDriftingLeft ? -minDriftSteer : minDriftSteer;

                if (isGrounded && airTime < 1.5f)
                {
                    acceleration *= driftFowardCompensation * Time.deltaTime; //Compensate for the forward force when drifting
                }
                else
                {
                    EndDrift();
                }

            }

            //If we are going backwards, we need to turn in the opposite direction
            if (backwardsCheck < 0)
            {
                //Applying our calculated turning direction to the turning variable
                turning = Quaternion.Euler(0f, -(turningDirection * Time.fixedDeltaTime), 0f);

                EndDrift();
            }
            else
            {
                //Applying our calculated turning direction to the turning variable
                turning = Quaternion.Euler(0f, turningDirection * Time.fixedDeltaTime, 0f);
            }

            if (isGrounded && movementDirection.x != 0f)
            {
                Vector3 turnCompensationForce = kartModel.forward * (accelerationRate  * 0.0075f * Mathf.Abs(movementDirection.x));
                sphere.AddForce(turnCompensationForce, ForceMode.Acceleration);
                
            }

            acceleration = turning * acceleration;
        }
        //If we are not moving, we don't need to turn
        else
        {
            turning = Quaternion.Euler(0f, 0f, 0f);
            EndDrift();
        }

        //Falling down
        if (!isGrounded && !attemptingDrift)
        {       
            float airRotationSpeed = 2.0f;

            // Target upright rotation based on Yaw (keep current Y, reset pitch/roll)
            Quaternion targetUpright = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

            // Smoothly rotate the kart's visual and normal upright orientation back to upright
            kartNormal.rotation = Quaternion.Slerp(kartNormal.rotation, targetUpright, Time.deltaTime * airRotationSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetUpright, Time.deltaTime * airRotationSpeed);
        }

        // Apply extra downward force to fall faster
        sphere.AddForce(-kartNormal.up * gravity, ForceMode.Acceleration); 

        //Update the wheel rotations
        //float steerAngle = movementDirection.x * maxSteerAngle;
        //
        //frontTireL.transform.localRotation = Quaternion.Euler(0, steerAngle, 0f);
        //frontTireR.transform.localRotation = Quaternion.Euler(0, steerAngle, 0f);

        //rBody.Move(transform.position + velocity * Time.fixedDeltaTime, transform.rotation * turning);
        // Apply movement

        sphere.AddForce(acceleration, ForceMode.Acceleration);       
        transform.rotation = transform.rotation * turning;

        //------------Traction---------------------
        if (isGrounded)
        {
            Vector3 forward = kartNormal.forward;
            Vector3 right = kartNormal.right;

            Vector3 velocity = sphere.velocity;

            //Lateral (sideways) velocity
            float lateralSpeed = Vector3.Dot(velocity, right);
            Vector3 lateralVelocity = right * lateralSpeed;

            //Apply opposite force to simulate traction
            Vector3 tractionForce = -lateralVelocity * tractionCoefficient;

            sphere.AddForce(tractionForce, ForceMode.Acceleration);
        }

    }



    void HandleGroundCheck()
    {
        RaycastHit hitNear;


        if (Physics.Raycast(transform.position + (transform.up * .2f), -kartNormal.up, out hitNear, groundCheckDistance, groundLayer))
        {
            airTime = 0f; //Reset air time when grounded
            isGrounded = true;

            //Normal Rotation
            kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * rotationAlignSpeed);
            kartNormal.Rotate(0, transform.eulerAngles.y, 0);
        }
        else
        {
            airTime += Time.deltaTime; //Keeping track of air time
            isGrounded = false; 
        }
    }

    void ApplyWheelVisuals()
    {
        float steerAngle = movementDirection.x * maxSteerAngle;
        frontTireL.transform.localRotation = Quaternion.Euler(0, steerAngle, 0f);
        frontTireR.transform.localRotation = Quaternion.Euler(0, steerAngle, 0f);
        steeringWheel.transform.Rotate(0, steerAngle * 2f, 0f);
    }

    /// <summary>
    /// Begin to make an attempt at drifting
    /// If the conditions for drifiting are met,
    /// you'll move to the next drifting script
    /// </summary>
    public void AttemptDrift()
    {
        if (isGrounded && !attemptingDrift)
        {
            isDrifting = true;
            isGrounded = false;

            StartCoroutine(DriftHopEnabler());

            // Animate visual jump
            kartModel.DOLocalMoveY(0.8f, 0.2f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad);
            
            // Optional squash/stretch
            kartModel.parent.DOScale(new Vector3(1.1f, 0.9f, 1.1f), 0.1f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutSine);            
        }
    }

    /// <summary>
    /// Begin the drifting script once the conditions are met.
    /// Allows the kart to hit a sharper angle when turning
    /// </summary>
    public void Drift()
    {

        if (!isDrifting) { return; }
        


        //driftTime += Time.deltaTime;

        // If we are drifting to the direction of the drift we increase the drift time accordingly
        if (isDriftingLeft)
        {
            switch (movementDirection.x)
            {
                //Turning right
                case > 0.1f:
                    driftTime += Time.deltaTime + movementDirection.x;
                    break;
                //Turning left
                case < -0.1f:
                    driftTime += (Time.deltaTime + -movementDirection.x) * 5;
                    break;
                default:
                    driftTime += (Time.deltaTime + 1) * 2;
                    break;
            }

        }
        else
        {
            switch (movementDirection.x)
            {
                //Turning right
                case > 0.1f:
                    driftTime += (Time.deltaTime + movementDirection.x) * 5;
                    break;
                //Turning left
                case < -0.1f:
                    driftTime += Time.deltaTime + -movementDirection.x;
                    break;
                default:
                    driftTime += (Time.deltaTime + 1) * 2;
                    break;
            }
        }

        if(movementDirection.z == 0)
        {   
            if(turboTwisting)
            {
                
            }
            else
            {
                
                StartCoroutine(TurboTwist());
                turboTwisting = true;
            }
            
        }

        float direction = isDriftingLeft ? -1f : 1f;

        sphere.AddForce(kartModel.right * direction * driftFactor, ForceMode.Acceleration);

        //--------------------Particles----------------                        
        ColorDrift();

        if (isDriftingLeft && driftTier > 0)
        {
            if (!particleSystemsBL[0].isPlaying)
            {
                //Activate left drift sparks when we get the boost
                foreach (ParticleSystem ps in particleSystemsBL)
                {
                    if (!ps.isPlaying)
                    {
                        ps.Play();
                    }
                }
            }

            if (driftTier > currentDriftTier)
            {
                //Adding transition sparks
                transitionSparksLtoR[0].Play();
                transitionSparksLtoR[2].Play();
                transitionSparksLtoR[4].Play();
                transitionSparksLtoR[5].Play();
            }
        }
        else if (!isDriftingLeft && driftTier > 0)
        {

            if (!particleSystemsBR[0].isPlaying)
            {
                //Activate right drift sparks when we get the boost
                foreach (ParticleSystem ps in particleSystemsBR)
                {
                    if (!ps.isPlaying)
                    {
                        ps.Play();
                    }
                }
            }


            if (driftTier > currentDriftTier)
            {

                //Adding transition sparks
                transitionSparksLtoR[1].Play();
                transitionSparksLtoR[3].Play();
                transitionSparksLtoR[6].Play();
                transitionSparksLtoR[7].Play();
            }

        }
        //Tire Screech Particles
        if (isDriftingLeft && !TireScreechesLtoR[0].isPlaying)
        {
            TireScreechesLtoR[0].Play();
            TireScreechesLtoR[2].Play();

            if (TireScreechesLtoR[1].isPlaying)
            {
                TireScreechesLtoR[1].Stop();
                TireScreechesLtoR[3].Stop();
                particleSystemsBR.ForEach(ps => ps.Stop());

            }
        }
        else if (!isDriftingLeft && !TireScreechesLtoR[1].isPlaying)
        {
            TireScreechesLtoR[1].Play();
            TireScreechesLtoR[3].Play();

            if (TireScreechesLtoR[0].isPlaying)
            {
                TireScreechesLtoR[0].Stop();
                TireScreechesLtoR[2].Stop();
                particleSystemsBL.ForEach(ps => ps.Stop());
            }
        }

        currentDriftTier = driftTier;
    }

    /// <summary>
    /// Ends the specific drift state.
    /// Provides a boost after drifting for a short time
    /// </summary>
    public void EndDrift()
    {

        if (!isDrifting) return;


        //---------Boost Types----------------
        if (driftTime > minDriftTime * 3f)
        {
            StartCoroutine(Boost(driftBoostForce, 1.6f));            
        }
        else if(driftTime > minDriftTime * 2f)
        {
            StartCoroutine(Boost(driftBoostForce, 0.8f));
        }
        else if (driftTime > minDriftTime)
        {
            StartCoroutine(Boost(driftBoostForce, 0.4f));
        }

        isDrifting = false;
        driftMethodCaller = false;
        driftTime = 0f;

        // Reset visuals
        driftPositionTween?.Kill();
        driftRotationTween?.Kill();
        driftRotationTween = kartModel.DOLocalRotate(Vector3.zero, driftTweenDuration)
            .SetEase(Ease.InOutSine);
        driftPositionTween = kartModel.DOLocalMove(Vector3.zero, driftTweenDuration);

        particleSystemsBL.ForEach(ps => ps.Stop());
        particleSystemsBR.ForEach(ps => ps.Stop());
        TireScreechesLtoR.ForEach(ps => ps.Stop());
        transitionSparksLtoR.ForEach(ps => ps.Stop());

    }

    public void ColorDrift()
    {
        Color c = Color.clear;

        
        if (driftTime > minDriftTime * 3f)
        {
            //red
            c = turboColors[3];
            driftTier = 3; 
        }
        else if(driftTime > minDriftTime * 2f)
        {
            //Orange
            c = turboColors[2];
            driftTier = 2; 
        }
        else if (driftTime > minDriftTime)
        {
            //Red
            c = turboColors[1];
            driftTier = 1;
        }        
        else
        {
            //Default color
            c = turboColors[0];
            driftTier = 0;
        }

        foreach (ParticleSystem ps in particleSystemsBL)
        {
            var main = ps.main;
            main.startColor = c;
        }
        foreach (ParticleSystem ps in particleSystemsBR)
        {
            var main = ps.main;
            main.startColor = c;
        }
        foreach (ParticleSystem ps in TireScreechesLtoR)
        {
            var main = ps.main;
            main.startColor = c;
            
        }
        foreach (ParticleSystem ps in transitionSparksLtoR)
        {
            var main = ps.main;
            main.startColor = c;
        }

    }

    IEnumerator Boost(float boostForce, float duration)
    {
        foreach(ParticleSystem ps in boostFlames)
        {
            if (!ps.isPlaying)
            {
                ps.Play();
            }
        }
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            Vector3 boostDirection = Vector3.zero;
            if (sphere.velocity.magnitude < boostMaxSpeed)
            {
                boostDirection = kartNormal.forward * driftBoostForce;
            }            

            sphere.AddForce(boostDirection, ForceMode.VelocityChange);
            yield return new WaitForFixedUpdate();
        }

        foreach (ParticleSystem ps in boostFlames)
        {
            if (ps.isPlaying)
            {
                ps.Stop();
            }
        }
    }

    IEnumerator DriftHopEnabler()
    {
        //Disabling raycast
        attemptingDrift = true;

        int TurnCount = 0;
        

        for (int i = 0; i < 25; i++)
        {
            

            //Check if player wants to drift either direction
            if (Mathf.Abs(movementDirection.x) > 0.1f && Mathf.Abs(sphere.velocity.x) > 5 && isDrifting)
            {
                TurnCount++;
            }
            else if(!isDrifting)
            {
                TurnCount = 0;
                yield return null;
            }

            yield return new WaitForFixedUpdate();
        }
        //Check if player wants to drift either direction
        if (TurnCount > 2)
        {
            driftMethodCaller = true;

            isDriftingLeft = movementDirection.x < 0f;
            driftTime = 0f;

            Debug.Log("Started Drift: " + (isDriftingLeft ? "Left" : "Right"));

            //// Visual drift lean
            //float yRot = isDriftingLeft ? -driftVisualAngle : driftVisualAngle;
            //float zTilt = isDriftingLeft ? driftVisualAngle : -driftVisualAngle;
            //
            //driftRotationTween?.Kill();
            //driftRotationTween = kartModel.DOLocalRotate(new Vector3(0f, yRot * 2f, zTilt * 0.5f), driftTweenDuration)
            //    .SetEase(Ease.InOutSine);


            //--------------Drift Animation-----------------

            float yRot = isDriftingLeft ? -40f : 40f;
            float zTilt = isDriftingLeft ? 2f : -2f;
            float snapTilt = isDriftingLeft ? -20f : 20f;

            driftRotationTween?.Kill();

            // Main drift lean
            driftRotationTween = DOTween.Sequence()
                .Append(kartModel.DOLocalRotate(new Vector3(0f, yRot * 4f, zTilt), 0.7f).SetEase(Ease.OutBack))    //slide out
                .Join(kartModel.DOLocalRotate(new Vector3(0f, yRot, snapTilt), 0.7f).SetEase(Ease.OutQuart))            //tilt side mid drift
                .Join(kartModel.DOLocalMoveY(0.5f, 0.7f).SetEase(Ease.OutQuart))                                     //subtle elevation                       
                .Append(kartModel.DOLocalRotate(new Vector3(0f, yRot, zTilt), 0.15f).SetEase(Ease.InQuad))          //untilt
                .Join(kartModel.DOLocalMoveY(0f, 0.15f).SetEase(Ease.OutBack))                                    //sutble descend
                .Append(kartModel.DOLocalRotate(new Vector3(0f, yRot / 2f, zTilt), 2.0f).SetEase(Ease.OutSine));       //slide in slightly
                //.OnComplete(() =>
                //{
                //    //Snap-Back Tilt Mid Drift
                //    float snapTilt = isDriftingLeft ? -30f : 30f;
                //
                //    // Kick back the Z tilt temporarily, then reset to stable drift tilt
                //    driftRotationTween = DOTween.Sequence()
                //        .Append(kartModel.DOLocalRotate(new Vector3(0f, yRot, snapTilt), 1f).SetEase(Ease.InQuad))
                //        .Join(kartModel.DOLocalMoveY(1f, 1f).SetEase(Ease.InOutSine))
                //        .Append(kartModel.DOLocalRotate(new Vector3(0f, yRot, zTilt), 0.4f).SetEase(Ease.OutBack))
                //        .Join(kartModel.DOLocalMoveY(0f, 0.4f).SetEase(Ease.InOutSine));
                //
                //
                //});
        }
        else
        {
            EndDrift();
            

        }

        
        attemptingDrift = false;

    }

    IEnumerator TurboTwist()
    {
        int TurnCount = 0;

        //Checks for 25 frames if we want to turbo twist
        for (int i = 0; i < 25; i++)
        {
            //Check if player wants to drift the opposite direction
            if(isDriftingLeft && movementDirection.x > 0.1f && movementDirection.z == 0)
            {
                TurnCount++;
            }
            else if (!isDriftingLeft && movementDirection.x < -0.1f && movementDirection.z == 0)
            {
                TurnCount++;
            }

            if(TurnCount > 3)
            {
                
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        //The closer the value is to 1 from below the closer we are to the minimum drift time
        float recenBoostvalue = Mathf.Abs(driftTime - minDriftTime * currentDriftTier);

        if (TurnCount > 3)
        {
            isDriftingLeft = !isDriftingLeft; //Change the direction of the drift

            if(recenBoostvalue <= turboTwistWindow)
            {
                StartCoroutine(Boost(driftBoostForce, 0.4f));
            }


            //--------------Drift Animation-----------------

            float yRot = isDriftingLeft ? -40f : 40f;
            float zTilt = isDriftingLeft ? 2f : -2f;
            float snapTilt = isDriftingLeft ? -20f : 20f;

            driftRotationTween?.Kill();

            // Main drift lean
            driftRotationTween = DOTween.Sequence()
                .Append(kartModel.DOLocalRotate(new Vector3(0f, yRot * 4f, zTilt), 0.7f).SetEase(Ease.OutBack))    //slide out
                .Join(kartModel.DOLocalRotate(new Vector3(0f, yRot, snapTilt), 0.7f).SetEase(Ease.OutQuart))            //tilt side mid drift
                .Join(kartModel.DOLocalMoveY(0.5f, 0.7f).SetEase(Ease.OutQuart))                                     //subtle elevation                       
                .Append(kartModel.DOLocalRotate(new Vector3(0f, yRot, zTilt), 0.15f).SetEase(Ease.InQuad))          //untilt
                .Join(kartModel.DOLocalMoveY(0f, 0.15f).SetEase(Ease.OutBack))                                    //sutble descend
                .Append(kartModel.DOLocalRotate(new Vector3(0f, yRot / 2f, zTilt), 2.0f).SetEase(Ease.OutSine));       //slide in slightly

        }

        turboTwisting = false; //Reset the turbo twisting state
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();

        movementDirection.z = movementDirection.y;

        movementDirection.y = 0; //We are not gonna jump duh

        // determines when driving starts and when driving ends
        if (context.started)
        {
            isDriving = true;
        }
        else if (context.canceled)
        {
            isDriving = false;

            // // stops engine sound
            // if (soundPlayer.isPlaying)
            // {
            //     soundPlayer.Stop();
            // }
        }
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

    public void OnTurn(InputAction.CallbackContext context)
    {
        controllerX = context.ReadValue<float>();
        UpdateControllerMovement(context);
    }

    public void OnAcceleration(InputAction.CallbackContext context)
    {
        controllerZ = context.ReadValue<float>();
        UpdateControllerMovement(context);

        if (context.started)
        {
            isDriving = true;
        }
        else if (context.canceled)
        {
            isDriving = false;
        }
    }

    private void UpdateControllerMovement(InputAction.CallbackContext context)
    {
        Vector2 moveInput = new Vector2(controllerX, controllerZ);
        if (moveInput.y > 0)
        {
            moveInput.y = 1;
        }
        else if (moveInput.y < 0)
        {
            moveInput.y = -1;
        }
        else
        {
            moveInput.y = 0;
        }

        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        movementDirection.x = moveInput.x;
        movementDirection.z = moveInput.y;
    }

    private void OnDrawGizmosSelected()
    {        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, sphere.velocity);
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position + (transform.up * 0.2f), -kartNormal.up * groundCheckDistance);
    }

    
}
