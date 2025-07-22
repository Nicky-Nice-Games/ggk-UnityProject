using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostTier1 : BaseItem
{
    private void OnTriggerEnter(Collider collision)
    {
        NEWDriver driver = collision.gameObject.GetComponent<NEWDriver>();
        if (driver != null)
        {
            // disable collider so it doesnt interfere with other players in scene
            this.gameObject.GetComponent<BoxCollider>().enabled = false;

            // set variables for boost
            float boostMult;
            float boostMaxSpeed;
            float duration;

            boostMult = 1.25f;
            boostMaxSpeed = boostMult * 60.0f;
            duration = 3.0f;

            // start boost
            StartCoroutine(ApplyBoost(driver, boostMult, duration, boostMaxSpeed));
        }
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
        //boostEffect.Play();
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
        //warpBoostEffect.SetActive(false);
        Destroy(this.gameObject);
    }
}
