using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boost : BaseItem
{

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // DecreaseTimer();
    }

    // the boost spawns on the user as an empty gameobject and applies a force to the kart
    // commented out to implement the boost as a trigger (through ItemHolder) rather than OnCollisionEnter

    //public void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Kart")
    //    {
    //        NEWDriver driver = null;
    //        NPCDriver npcDriver = null;
    //        if (collision.gameObject.GetComponent<NEWDriver>() != null)
    //        {
    //            driver = collision.gameObject.GetComponent<NEWDriver>();
    //        }
    //        else if (collision.gameObject.GetComponent<NPCDriver>() != null)
    //        {
    //            npcDriver = collision.gameObject.GetComponent<NPCDriver>();
    //        }
    //
    //        float boostMult = 0f;
    //        Debug.Log("COLLIDED!");
    //
    //        if (isUpgraded)
    //        {
    //            Vector3 boost = driver.transform.forward * 2000f * 5;
    //            if (driver != null)
    //            {
    //                driver.sphere.velocity += boost * 0.3f;
    //                driver.sphere.AddForce(boost, ForceMode.VelocityChange);
    //            }
    //            else if (npcDriver != null)
    //            {
    //                npcDriver.velocity += boost * 0.3f;
    //                npcDriver.rBody.AddForce(boost, ForceMode.VelocityChange);
    //            }
    //        }
    //        else
    //        {
    //
    //            Vector3 boost = transform.forward * 1150f * 5;
    //            if (driver != null)
    //            {
    //                driver.sphere.velocity += boost * 0.3f;
    //                driver.sphere.AddForce(boost, ForceMode.VelocityChange);
    //            }
    //            else if (npcDriver != null)
    //            {
    //                npcDriver.velocity += boost * 0.3f;
    //                npcDriver.rBody.AddForce(boost, ForceMode.VelocityChange);
    //            }
    //
    //        }
    //        StartCoroutine(ApplyBoost(driver, boostMult, 5.0f));
    //        Debug.Log("Applying Boost Item!");
    //    }
    //}

    //IEnumerator ApplyBoost(NEWDriver driver, float boostForce, float duration)
    //{
    //    for (float t = 0; t < duration; t += Time.deltaTime)
    //    {
    //        Vector3 boostDirection = driver.transform.forward * boostForce;
    //
    //        driver.sphere.AddForce(boostDirection, ForceMode.VelocityChange);
    //        yield return new WaitForFixedUpdate();
    //    }
    //}
}
