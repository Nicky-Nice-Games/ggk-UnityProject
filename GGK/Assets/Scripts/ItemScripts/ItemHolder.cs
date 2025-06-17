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
        // handles item usage
        if (holdingItem)
        {
            //soundPlayer.PlayOneShot(throwSound);
            BaseItem item = Instantiate(heldItem, transform.position, transform.rotation);
            item.Kart = this;
            item.IsUpgraded = heldItem.IsUpgraded;
            heldItem = null;
            itemDisplay.texture = defaultItemDisplay;
            holdingItem = false;
        }
    }

    public void OnThrow()
    {
        // handles item usage
        if (holdingItem)
        {
            // sound effect when item thrown
            //soundPlayer.PlayOneShot(throwSound);

            BaseItem item = Instantiate(heldItem, transform.position, transform.rotation);
            item.Kart = this;
            item.IsUpgraded = heldItem.IsUpgraded;
            heldItem = null;
            holdingItem = false;
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
            if (heldItem == null)
            {
                heldItem = itemBox.RandomizeItem();
                if (itemDisplay != null)
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
            itemDisplay.texture = heldItem.itemIcon;

            // Either upgrades the current item or gives the kart a random upgraded item
            //baseItem = upgradeBox.UpgradeItem(this);
            // Disables the upgrade box
            upgradeBox.gameObject.SetActive(false);
            //heldItemText.text = $"Held Item: {baseItem}+";
        }

        // checks if the kart hits a projectile and drops the velocity to 1/8th of the previous value
        if (collision.gameObject.transform.tag == "Projectile")
        {
            Destroy(collision.gameObject);
            thisDriver.sphere.velocity /= 8;
        }

        // kart uses a boost and is given the boost through a force
        if (collision.gameObject.CompareTag("Boost"))
        {
            Boost boost = collision.gameObject.GetComponent<Boost>();
            float boostMult;
            float duration = 2.5f;
            if (boost.IsUpgraded)
            {
                boostMult = 2.0f;
            }
            else
            {
                boostMult = 1.5f;
            }
            StartCoroutine(ApplyBoost(thisDriver, boostMult, duration));
            Debug.Log("Applying Boost Item!");
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

}