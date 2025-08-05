using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.VFX;

public class BoostTier4 : BaseItem
{
    [Header("Tier 4 Boost Settings")]
    [SerializeField] GameObject warpBoostEffect;
    [SerializeField] float warpWaitTime;
    private VFXHandler vfxScript;
    private VisualEffect warpEffectBottom;
    private VisualEffect warpEffectTop;
    private Vector3 originalScale;
    private Transform driverTransform;


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

            // keep reference to original scale
            driverTransform = driver.transform.parent.Find("Collider").GetComponent<DynamicRecovery>().kartModel;
            originalScale = driverTransform.localScale;

            // grab boost effect from driver prefab
            //boostEffect = driver.transform.
            //    Find("Normal/Parent/KartModel/VFXEffects/EnergyDrinkBoost").GetComponent<VisualEffect>();
            vfxScript = driver.vfxHandler;

            // get warp effects from script
            warpEffectBottom = vfxScript.warpBoostBottom;
            warpEffectTop = vfxScript.warpBoostTop;

            GameObject kartParent = driver.transform.parent.gameObject;
            KartCheckpoint kartCheck = kartParent.GetComponentInChildren<KartCheckpoint>();

            // set boost variables
            boostMult = 1.25f;
            boostMaxSpeed = boostMult * 60;
            duration = 3.0f;

            // find wormhole effect from player prefab
            warpBoostEffect = driver.transform.Find("Wormhole Effect").gameObject;

            int currentCheckpointId = kartCheck.checkpointId;
            int warpCheckpointId = currentCheckpointId + 3;

            // check if the checkpoint is past the count and adjust
            int checkpointMax = kartCheck.checkpointList.Count - 1;
            if (warpCheckpointId > checkpointMax)
            {
                // check if the warp checkpoint passes the last checkpoint by 1, 2 or 3
                if (checkpointMax + 1 == warpCheckpointId)
                {
                    warpCheckpointId = 0;
                }
                else if (checkpointMax + 2 == warpCheckpointId)
                {
                    warpCheckpointId = 1;
                }
                else if (checkpointMax + 3 == warpCheckpointId)
                {
                    warpCheckpointId = 2;
                }
                kartCheck.lap++;
                kartCheck.PassedWithWarp = true;
            }

            GameObject warpCheckpoint = kartCheck.checkpointList[warpCheckpointId];

            // Play warp vfx
            warpEffectTop.SetFloat("Duration", warpWaitTime);
            warpEffectTop.Play();

            // Waits a certain number of seconds, and then activates the warp boost
            StartCoroutine(WaitThenBoost(driver, warpCheckpoint, kartCheck, warpCheckpointId,
                           boostMult, duration, boostMaxSpeed));
        }

        // handles boosting for npcs
        NPCPhysics npcDriver = collision.gameObject.GetComponent<NPCPhysics>();
        if (npcDriver != null)
        {
            // disable collider so it doesnt interfere with other players in scene
            this.gameObject.GetComponent<BoxCollider>().enabled = false;

            vfxScript = npcDriver.gameObject.GetComponent<VFXHandler>();

            boostMult = 1.5f;
            boostMaxSpeed = boostMult * 60;
            duration = 3.0f;

            StartCoroutine(ApplyBoostNPC(npcDriver, boostMult, duration, boostMaxSpeed));
        }
    }

    IEnumerator WaitThenBoost(NEWDriver thisDriver, GameObject warpCheckpoint, KartCheckpoint kartCheck, int warpCheckpointId,
                                      float boostMult, float duration, float boostMaxSpeed)
    {
        //This is the code for the player.
        if (thisDriver != null)
        {
            //Movement and Velocity vectors are set to 0,
            //we wait a certain amount of time,
            //reset the vectors, and then boost!
            Vector3 originalMovement = thisDriver.movementDirection;
            Vector3 originalVelocity = thisDriver.sphere.velocity;
            thisDriver.movementDirection = Vector3.zero;
            thisDriver.sphere.velocity = Vector3.zero;

            // gives the car a stretch effect
            StartCoroutine(Stretch(warpWaitTime));

            yield return new WaitForSeconds(warpWaitTime);
            thisDriver.sphere.velocity = originalVelocity;
            thisDriver.movementDirection = originalMovement;
        }
        // set the kart's position to 3 checkpoints ahead
        thisDriver.sphere.transform.position = warpCheckpoint.transform.position;
        thisDriver.transform.rotation = Quaternion.Euler(0, warpCheckpoint.transform.eulerAngles.y - 90, 0);
        kartCheck.checkpointId = warpCheckpointId;
        StartCoroutine(ApplyBoost(thisDriver, boostMult, duration, boostMaxSpeed));
        yield return new WaitForFixedUpdate();
    }

    /// <summary>
    /// applies a boost to the driver
    /// </summary>
    /// <param name="driver">player who owns the boost</param>
    /// <param name="boostForce">amount of force to be applied to the player</param>
    /// <param name="duration">how long the boost should last</param>
    /// <param name="boostMaxSpeed">the max speed the player can reach from the boost</param>
    /// <returns></returns>
    IEnumerator ApplyBoost(NEWDriver driver, float boostForce, float duration, float boostMaxSpeed)
    {
        // play warp and boost vfx
        warpEffectTop.SetFloat("Duration", duration / 2);
        warpEffectTop.Play();
        vfxScript.PlayItemBoostVFX(duration);

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            Vector3 boostDirection = Vector3.zero;
            if (driver.sphere.velocity.magnitude < boostMaxSpeed)
            {
                boostDirection = driver.transform.forward * boostForce;
            }
            driver.sphere.AddForce(boostDirection, ForceMode.VelocityChange);
            yield return new WaitForFixedUpdate();
        }
        //boostEffect.Stop();
        vfxScript.StopItemEffects();
        //warpBoostEffect.SetActive(false);

        // add boost usage
        driver.playerInfo.boostUsage["speedBoost4"]++;
        Destroy(this.gameObject);
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

    /// <summary>
    /// stretches out the kart for the warp effect
    /// </summary>
    /// <returns>amount of time to wait before going again</returns>
    IEnumerator Stretch(float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            Vector3 currentScale = driverTransform.localScale;

            // make kart taller and skinnier
            currentScale.y *= 1.01f;
            currentScale.x /= 1.01f;

            driverTransform.localScale = currentScale;
            yield return new WaitForFixedUpdate();
        }

        // go back to original scale
        driverTransform.localScale = originalScale;
    }
}
