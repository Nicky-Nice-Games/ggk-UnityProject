using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class BoostTier3 : BaseItem
{
    [Header("Tier 3 Boost Settings")]
    public float length = 4.0f;
    private float lastHitDistance;
    public float strength = 4.0f;
    public float dampening = 40.0f;
    private VFXHandler vfxScript;

    uint canOpenID = 0;

    private void OnTriggerEnter(Collider collision)
    {
        // set variables for boost
        float boostMult;
        float boostMaxSpeed;
        float duration;

        NEWDriver driver = collision.gameObject.GetComponent<NEWDriver>();
        if (driver != null)
        {
            // disable collider so it doesnt interfere with other players in scene
            this.gameObject.GetComponent<BoxCollider>().enabled = false;

            // find all the wheel effects in the player prefab
            //effects.Add(driver.transform.
            //    Find("Normal/Parent/KartModel/VFXEffects/TornadoVFXGraph").GetComponent<VisualEffect>());
            //effects.Add(driver.transform.
            //    Find("Normal/Parent/KartModel/VFXEffects/TornadoVFXGraph (1)").GetComponent<VisualEffect>());
            //effects.Add(driver.transform.
            //    Find("Normal/Parent/KartModel/VFXEffects/TornadoVFXGraph (2)").GetComponent<VisualEffect>());
            //effects.Add(driver.transform.
            //    Find("Normal/Parent/KartModel/VFXEffects/TornadoVFXGraph (3)").GetComponent<VisualEffect>());

            // grab vfx script from driver for wheel effects
            vfxScript = driver.vfxHandler;

            boostMult = 1.75f;
            boostMaxSpeed = boostMult * 60.0f;
            duration = 3.0f;

            canOpenID = AkUnitySoundEngine.PostEvent("Play_Can_Open", gameObject);

            // start boost
            StartCoroutine(ApplyBoostUpward(driver, boostMult, duration, boostMaxSpeed));
        }

        // handles boosting for npcs
        NPCPhysics npcDriver = collision.gameObject.GetComponent<NPCPhysics>();
        if (npcDriver != null)
        {
            // disable collider so it doesnt interfere with other players in scene
            this.gameObject.GetComponent<BoxCollider>().enabled = false;

            vfxScript = npcDriver.gameObject.GetComponent<VFXHandler>();

            boostMult = 1.5f;
            boostMaxSpeed = boostMult * 60.0f;
            duration = 3.0f;

            StartCoroutine(ApplyBoostNPC(npcDriver, boostMult, duration, boostMaxSpeed));
        }
    }

    /// <summary>
    /// for level 3 boost, adds a speed boost forwards and upwards
    /// </summary>
    /// <param name="driver">the player</param>
    /// <param name="boostForce">amount of force to boost character</param>
    /// <param name="duration">how long boost should last</param>
    /// <returns></returns>
    IEnumerator ApplyBoostUpward(NEWDriver driver, float boostForce, float duration, float boostMaxSpeed)
    {
        // turn off drift and ground check in driver script
        driver.doGroundCheck = false;
        driver.canDrift = false;
        driver.turnWheels = false;

        // get a list of the player's wheels for raycasting
        List<GameObject> wheels = new List<GameObject>();
        wheels.Add(driver.backTireL);
        wheels.Add(driver.backTireR);
        wheels.Add(driver.frontTireL);
        wheels.Add(driver.frontTireR);

        for (int i = 0; i < wheels.Count; i++)
        {
            // rotate wheels 90 degrees
            wheels[i].transform.localRotation = Quaternion.Euler(0, 0, 90);

            // start wind effects
            //effects[i].SetFloat("Duration", duration + 0.5f);
            //effects[i].Play();

            vfxScript.PlayHoverVFX(duration + 0.5f);
        }

        // driver.sphere.AddForce(driver.transform.up * 2, ForceMode.Impulse);
        RaycastHit hit;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            // perform a raycast at each wheel on the player's kart
            for (int i = 0; i < 4; i++)
            {
                GameObject wheel = wheels[i];
                if (Physics.Raycast(wheel.transform.position, -driver.transform.up,
                    out hit, length))
                {
                    // determine spring force
                    float forceAmount = HooksLawDampen(hit.distance);

                    // add force at each wheel position
                    driver.sphere.AddForceAtPosition(driver.transform.up * forceAmount, wheel.transform.position);
                }
                else
                {
                    lastHitDistance = length * 1.1f;
                }
            }

            // gives forward boost
            Vector3 boostDirection = Vector3.zero;
            if (driver.sphere.velocity.magnitude < boostMaxSpeed)
            {
                boostDirection = driver.transform.forward * boostForce;
            }
            driver.sphere.AddForce(boostDirection, ForceMode.VelocityChange);
            yield return new WaitForFixedUpdate();
        }

        // return wheels to original rotation
        for (int i = 0; i < wheels.Count; i++)
        {
            wheels[i].transform.localRotation = Quaternion.identity;
        }

        // reenable drift and ground check
        driver.canDrift = true;
        driver.doGroundCheck = true;
        driver.turnWheels = true;

        Destroy(this.gameObject);
    }

    /// <summary>
    /// determines a spring force for tier three boost to create hover effect
    /// </summary>
    /// <param name="hitDistance">distance raycast is from ground</param>
    /// <returns>spring force</returns>
    private float HooksLawDampen(float hitDistance)
    {
        float forceAmount = strength * (length - hitDistance) + (dampening * (lastHitDistance - hitDistance));
        forceAmount = Mathf.Max(0f, forceAmount);
        lastHitDistance = hitDistance;

        return forceAmount;
    }

    IEnumerator ApplyBoostNPC(NPCPhysics driver, float boostForce, float duration, float boostMaxSpeed)
    {
        vfxScript.PlayItemBoostVFX(duration);
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            Vector3 boostDirection = Vector3.zero;
            if (driver.sphere.velocity.magnitude < boostMaxSpeed)
            {
                boostDirection = driver.kartNormal.forward * boostForce;
            }
            driver.sphere.AddForce(boostDirection, ForceMode.VelocityChange);
            Debug.Log("Applied npc boost");
            yield return new WaitForFixedUpdate();
        }
        vfxScript.StopItemEffects();
        Destroy(this.gameObject);
    }
}
