using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using UnityEngine.UI;

public class ItemHolder : MonoBehaviour
{
    private bool holdingItem;

    [SerializeField]
    private NEWDriver thisDriver;

    [SerializeField]
    private NPCDriver npcDriver;

    [SerializeField]
    private BaseItem heldItem;

    [SerializeField]
    private float timer = 5.0f;

    [SerializeField]
    private RawImage itemDisplay;

    [SerializeField]
    private Texture defaultItemDisplay;

    private BaseItem item;
    private int uses;

    // [SerializeField]
    // private TextMesh heldItemText;

    // audio variables
    //private AudioSource soundPlayer;

    [SerializeField]
    //AudioClip throwSound;
    public BaseItem HeldItem { get { return heldItem; } set { heldItem = value; } }
    public bool HoldingItem { get { return holdingItem; } set { holdingItem = value; } }
    // Start is called before the first frame update
    void Start()
    {
        holdingItem = IsHoldingItem();

        timer = Random.Range(1, 6);

        uses = 0;

        if (thisDriver)
        {
            itemDisplay.texture = defaultItemDisplay;
        }

        //soundPlayer = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        holdingItem = IsHoldingItem();


        if (npcDriver != null && holdingItem)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                OnThrow();
            }
        }

        if (timer < 0)
        {
            timer = 0;
        }


        if (holdingItem && item)
        {
            if (item.UseCount == 1 && item.isTimed)
            {
                if (item.Timer <= 0.0f)
                {
                    heldItem = null;
                    holdingItem = false;
                    if (thisDriver)
                    {
                        itemDisplay.texture = defaultItemDisplay;
                    }
                    Destroy(item.gameObject);
                }
            }
            else if (item.UseCount == 1 && !item.isTimed)
            {
                if (uses == 0)
                {
                    heldItem = null;
                    holdingItem = false;
                    if (thisDriver)
                    {
                        itemDisplay.texture = defaultItemDisplay;
                    }
                }
            }
            else if (item.UseCount > 1 && item.isTimed)
            {
                if (item.Timer <= 0.0f || uses == 0)
                {
                    heldItem = null;
                    holdingItem = false;
                    if (thisDriver)
                    {
                        itemDisplay.texture = defaultItemDisplay;
                    }
                    Destroy(item.gameObject);
                }
            }
            else if (item.UseCount >= 1 && !item.isTimed)
            {
                if (uses == 0)
                {
                    heldItem = null;
                    holdingItem = false;
                    if (thisDriver)
                    {
                        itemDisplay.texture = defaultItemDisplay;
                    }
                }
            }
        }

        if (thisDriver)
        {
            Debug.Log(uses);
        }


    }

    private bool IsHoldingItem()
    {
        if (heldItem != null)
        {
            // heldItemText.text = $"Held Item: {heldItem}";
            return true;
        }
        else
        {
            // heldItemText.text = $"Held Item:";
            return false;
        }
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (!holdingItem) return;

        if (uses > 0 && context.phase == InputActionPhase.Performed)
        {
            item = Instantiate(heldItem, transform.position, transform.rotation);
            //soundPlayer.PlayOneShot(throwSound);
            item.Kart = this;
            item.ItemTier = heldItem.ItemTier;

            uses--; // Decrease after successful use
        }
    }

    public void OnThrow()
    {
        // handles item usage
        if (holdingItem)
        {
            if (uses == 0)
            {
                uses = heldItem.UseCount;
            }
            else
            {
                uses -= 1;
            }
            // sound effect when item thrown
            //soundPlayer.PlayOneShot(throwSound);

            item = Instantiate(heldItem, transform.position, transform.rotation);
            item.Kart = this;
            item.ItemTier = heldItem.ItemTier;
        }
        timer = Random.Range(1, 6);

    }

    public void OnCollisionEnter(Collision collision)
    {
        // checks if the kart hits a projectile and drops the velocity to 1/8th of the previous value
        if (collision.gameObject.transform.tag == "Projectile")
        {
            Destroy(collision.gameObject);
            if (thisDriver != null)
            {
                thisDriver.sphere.velocity /= 8000;
            }
            else if (npcDriver != null)
            {
                //npcDriver.DisableDriving();
                //npcDriver.velocity /= 8000;
                //npcDriver.maxSpeed = 100;
                //npcDriver.accelerationRate = 500;
                //npcDriver.followTarget.GetComponent<SplineAnimate>().enabled = false;
                npcDriver.StartRecovery();
            }
        }

    }

    public void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collided");
        // Checks if kart hits an item box
        if (collision.gameObject.CompareTag("ItemBox"))
        {
            ItemBox itemBox = collision.gameObject.GetComponent<ItemBox>();

            // Gives kart an item if they don't already have one
            if (!holdingItem)
            {
                heldItem = itemBox.RandomizeItem();
                
                // Initialize use count if first use
                if (uses == 0)
                {
                    uses = heldItem.UseCount;
                }
                Debug.Log(uses);
                if (thisDriver)
                {
                    itemDisplay.texture = heldItem.itemIcon;
                }
            }
            // Disables the item box
            itemBox.gameObject.SetActive(false);
            //heldItemText.text = $"Held Item: {baseItem}";
        }
        else if (collision.gameObject.CompareTag("UpgradeBox"))
        {
            UpgradeBox upgradeBox = collision.gameObject.GetComponent<UpgradeBox>();
            // itemDisplay.texture = heldItem.itemIcon;

            if (heldItem != null)
            {
                if (heldItem.ItemTier < 4)
                {
                    heldItem.ItemTier++;
                }
                heldItem.OnLevelUp(heldItem.ItemTier);
                uses = heldItem.UseCount;
            }

            // Either upgrades the current item or gives the kart a random upgraded item
            //baseItem = upgradeBox.UpgradeItem(this);
            // Disables the upgrade box
            upgradeBox.gameObject.SetActive(false);
            //heldItemText.text = $"Held Item: {baseItem}+";
        }

        // checks if the kart hits a projectile and drops the velocity to 1/8th of the previous value
        if (collision.gameObject.transform.tag == "Projectile")
        {
            if (thisDriver != null)
            thisDriver.sphere.velocity /= 8;
            Destroy(collision.gameObject);
        }

        // kart uses a boost and is given the boost through a force
        if (collision.gameObject.CompareTag("Boost"))
        {
            Boost boost = collision.gameObject.GetComponent<Boost>();
            float boostMult;
            float duration = 2.5f;
            if (boost.ItemTier == 2)
            {
                thisDriver.sphere.velocity /= 8;
            }
            else if (npcDriver != null)
            {
                //npcDriver.DisableDriving();
                //npcDriver.velocity /= 8;
                //npcDriver.maxSpeed = 100;
                //npcDriver.accelerationRate = 500;
                //npcDriver.followTarget.GetComponent<SplineAnimate>().enabled = false;
            }
                boostMult = 1.5f;
            
            if(thisDriver != null)
            {
                switch (boost.ItemTier)
                {
                    default:
                        StartCoroutine(ApplyBoost(thisDriver, boostMult, duration));
                        break;
                    case 3:
                        StartCoroutine(ApplyBoostUpward(thisDriver, boostMult, duration));
                        break;
                    case 4:
                        break;

                }
                Debug.Log("Applying Boost Item!");
            }
            else if(npcDriver != null)
            {
                StartCoroutine(ApplyBoostNPC(npcDriver, boostMult, duration));
                Debug.Log("Applying Boost Item!");
            }
            Destroy(collision.gameObject);
        }

        // checks if the kart drives into a hazard and drops the velocity to 1/8th of the previous value
        if (collision.gameObject.transform.tag == "Hazard")
        {
            Destroy(collision.gameObject);
            if (thisDriver != null)
            {

                thisDriver.sphere.velocity /= 8000;
            }
            else if (npcDriver != null)
            {
                //npcDriver.DisableDriving();
                //npcDriver.velocity /= 8;
                //npcDriver.maxSpeed = 100;
                //npcDriver.accelerationRate = 500;
                //npcDriver.followTarget.GetComponent<SplineAnimate>().enabled = false;

                npcDriver.StartRecovery();
            }
        }

    }

    IEnumerator ApplyBoost(NEWDriver driver, float boostForce, float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            Vector3 boostDirection = driver.transform.forward * boostForce;

            driver.sphere.AddForce(boostDirection, ForceMode.VelocityChange);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator ApplyBoostUpward(NEWDriver driver, float boostForce, float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            Vector3 boostDirection = driver.transform.forward * boostForce;
            boostDirection.y = boostForce;

            driver.sphere.AddForce(boostDirection, ForceMode.VelocityChange);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator ApplyBoostNPC(NPCDriver driver, float boostForce, float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            Vector3 boostDirection = driver.transform.forward * boostForce;

            driver.rBody.AddForce(boostDirection, ForceMode.VelocityChange);
            yield return new WaitForFixedUpdate();
        }
    }
}
