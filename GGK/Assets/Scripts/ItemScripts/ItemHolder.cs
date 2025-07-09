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

    private Vector3 itemDisplayScale;
    private Vector2 itemDisplayPosition;

    [SerializeField]
    private Texture defaultItemDisplay;

    private BaseItem item;
    private int driverItemTier;
    public int uses;

    public float gravityForce;

    //the current coroutine animating spinning, to prevent double-ups
    private IEnumerator currentSpinCoroutine;
    public MiniMapHud miniMap;

    // [SerializeField]
    // private TextMesh heldItemText;

    // audio variables
    //private AudioSource soundPlayer;

    // [SerializeField]
    //AudioClip throwSound;

    // tier 3 boost variables
    [Header("Tier 3 Boost Settings")]
    public float length = 4.0f;
    private float lastHitDistance;
    public float strength = 5.0f;
    public float dampening = 20.0f;

    public BaseItem HeldItem { get { return heldItem; } set { heldItem = value; } }
    public bool HoldingItem { get { return holdingItem; } set { holdingItem = value; } }
    public int DriverItemTier { get { return driverItemTier; } set { driverItemTier = value; } }

    // Start is called before the first frame update
    void Start()
    {
        DOTween.Init();
        holdingItem = IsHoldingItem();

        timer = Random.Range(5, 8);

        uses = 0;

        if (thisDriver)
        {
            itemDisplay.texture = defaultItemDisplay;
            itemDisplayScale = itemDisplay.rectTransform.localScale;
            itemDisplayPosition = itemDisplay.rectTransform.position;
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
            // for shield
            if (item.isTimed)
            {
                if (item.Timer <= 0.0f)
                {
                    // heldItem = null;
                    holdingItem = false;
                    if (thisDriver)
                    {
                        itemDisplay.texture = defaultItemDisplay;
                    }
                    Destroy(item.gameObject);
                    Destroy(heldItem.gameObject);
                }
            }
            else if (item.UseCount == 1 && !item.isTimed)
            {
                if (uses == 0)
                {
                    // heldItem = null;
                    holdingItem = false;
                    if (thisDriver)
                    {
                        itemDisplay.texture = defaultItemDisplay;
                    }
                    Destroy(heldItem.gameObject);
                }
            }
            else if (item.UseCount > 1 && item.isTimed)
            {
                if (item.Timer <= 0.0f || uses == 0)
                {
                    // heldItem = null;
                    holdingItem = false;
                    if (thisDriver)
                    {
                        itemDisplay.texture = defaultItemDisplay;
                    }
                    Destroy(item.gameObject);
                    Destroy(heldItem.gameObject);
                }
            }
            else if (item.UseCount >= 1 && !item.isTimed)
            {
                if (uses == 0)
                {
                    // heldItem = null;
                    holdingItem = false;
                    if (thisDriver)
                    {
                        itemDisplay.texture = defaultItemDisplay;
                    }
                    Destroy(heldItem.gameObject);
                }
            }
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
            itemDisplay.rectTransform.position = itemDisplayPosition;
            itemDisplay.rectTransform.DOPunchPosition(new Vector3(0, 30, 0), 0.5f);

            item = Instantiate(heldItem, transform.position, transform.rotation);
            //soundPlayer.PlayOneShot(throwSound);
            item.gameObject.SetActive(true);
            item.Kart = this;
            item.ItemTier = heldItem.ItemTier;

            if (heldItem.ItemTier > 1)
            {
                item.OnLevelUp(item.ItemTier);
            }

            uses--; // Decrease after successful use

            Debug.Log(item.ItemTier);

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
            item.gameObject.SetActive(true);
            item.Kart = this;
            item.ItemTier = heldItem.ItemTier;
        }
        timer = Random.Range(5, 8);

        Debug.Log(item.ItemTier);

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
                itemBox.RandomizeItem(this.gameObject);
              
                // Initialize use count if first use
                if (uses == 0)
                {
                    uses = heldItem.UseCount;
                }
                Debug.Log(uses);
                if (thisDriver)
                {
                    ApplyItemTween(heldItem.itemIcon);
                }
            }
            // Disables the item box
            itemBox.gameObject.SetActive(false);
            //heldItemText.text = $"Held Item: {baseItem}";
        }
        //else if (collision.gameObject.CompareTag("UpgradeBox"))
        //{
        //    UpgradeBox upgradeBox = collision.gameObject.GetComponent<UpgradeBox>();
        //    // itemDisplay.texture = heldItem.itemIcon;

        //    // if player missing item, gives random level 2 item or upgrades current item
        //    upgradeBox.UpgradeItem(this.gameObject);
        //    heldItem.OnLevelUp(heldItem.ItemTier);
        //    uses = heldItem.UseCount;

        //    // Either upgrades the current item or gives the kart a random upgraded item
        //    //baseItem = upgradeBox.UpgradeItem(this);

        //    // displays item in the HUD
        //    if (thisDriver)
        //    {
        //        ApplyItemTween(heldItem.itemIcon);
        //    }

        //    // Disables the upgrade box
        //    upgradeBox.gameObject.SetActive(false);
        //    //heldItemText.text = $"Held Item: {baseItem}+";
        //}
        if (collision.gameObject.CompareTag("ItemBox"))
        {
            Debug.Log("Collided with ItemBox");
            ItemBox itemBox = collision.gameObject.GetComponent<ItemBox>();
            Debug.Log(itemBox.ItemBoxType);
            switch (itemBox.ItemBoxType)
            {
                case "Projectile":
                    ProjectileBrick projBrick = collision.gameObject.GetComponent<ProjectileBrick>();
                    if (!holdingItem)
                    {
                        projBrick.GiveProjectile(this.gameObject);
                        // Initialize use count if first use
                        if (uses == 0)
                        {
                            uses = heldItem.UseCount;
                        }
                        if (thisDriver)
                        {
                            ApplyItemTween(heldItem.itemIcon);
                        }
                    }
                    else if (heldItem.ItemCategory == "Puck")
                    {
                        // if player missing item, gives random level 2 item or upgrades current item
                        projBrick.UpgradeItem(this.gameObject);
                        heldItem.OnLevelUp(heldItem.ItemTier);
                        uses = heldItem.UseCount;

                        // displays item in the HUD
                        if (thisDriver)
                        {
                            ApplyItemTween(heldItem.itemIcon);
                        }

                        // Disables the upgrade box
                        projBrick.gameObject.SetActive(false);
                    }
                    break;
                case "Boost":
                    BoostBrick boostBrick = collision.gameObject.GetComponent<BoostBrick>();
                    if (!holdingItem)
                    {
                        boostBrick.GiveBoost(this.gameObject);
                        // Initialize use count if first use
                        if (uses == 0)
                        {
                            uses = heldItem.UseCount;
                        }
                        if (thisDriver)
                        {
                            ApplyItemTween(heldItem.itemIcon);
                        }
                    }
                    else if (heldItem.ItemCategory == "Boost")
                    {
                        // if player missing item, gives random level 2 item or upgrades current item
                        boostBrick.UpgradeItem(this.gameObject);
                        heldItem.OnLevelUp(heldItem.ItemTier);
                        uses = heldItem.UseCount;

                        // displays item in the HUD
                        if (thisDriver)
                        {
                            ApplyItemTween(heldItem.itemIcon);
                        }

                        // Disables the upgrade box
                        boostBrick.gameObject.SetActive(false);
                    }
                    break;
                case "Defense":
                    DefenseBrick defBrick = collision.gameObject.GetComponent<DefenseBrick>();
                    if (!holdingItem)
                    {
                        defBrick.GiveShield(this.gameObject);
                        // Initialize use count if first use
                        if (uses == 0)
                        {
                            uses = heldItem.UseCount;
                        }
                        if (thisDriver)
                        {
                            ApplyItemTween(heldItem.itemIcon);
                        }
                    }
                    else if (heldItem.ItemCategory == "Shield")
                    {
                        // if player missing item, gives random level 2 item or upgrades current item
                        defBrick.UpgradeItem(this.gameObject);
                        heldItem.OnLevelUp(heldItem.ItemTier);
                        uses = heldItem.UseCount;

                        // displays item in the HUD
                        if (thisDriver)
                        {
                            ApplyItemTween(heldItem.itemIcon);
                        }

                        // Disables the upgrade box
                        defBrick.gameObject.SetActive(false);
                    }
                    break;
                case "Hazard":
                    HazardBrick hazBrick = collision.gameObject.GetComponent<HazardBrick>();
                    if (!holdingItem)
                    {
                        hazBrick.GiveHazard(this.gameObject);
                        // Initialize use count if first use
                        if (uses == 0)
                        {
                            uses = heldItem.UseCount;
                        }
                        if (thisDriver)
                        {
                            ApplyItemTween(heldItem.itemIcon);
                        }
                    }
                    else if (heldItem.ItemCategory == "Hazard")
                    {
                        // if player missing item, gives random level 2 item or upgrades current item
                        hazBrick.UpgradeItem(this.gameObject);
                        heldItem.OnLevelUp(heldItem.ItemTier);
                        uses = heldItem.UseCount;

                        // displays item in the HUD
                        if (thisDriver)
                        {
                            ApplyItemTween(heldItem.itemIcon);
                        }

                        // Disables the upgrade box
                        hazBrick.gameObject.SetActive(false);
                    }
                    break;
                default:
                    break;
            }
            itemBox.gameObject.SetActive(false);
        }

        if (collision.gameObject.CompareTag("Projectile"))
        {
            if (thisDriver != null)
                thisDriver.sphere.velocity /= 8;
            Destroy(collision.gameObject);
            ApplyIconSpin(gameObject, 1);
        }
        // kart uses a boost and is given the boost through a force
        if (collision.gameObject.CompareTag("Boost"))
        {
            Boost boost = collision.gameObject.GetComponent<Boost>();
            float boostMult;
            float boostMaxSpeed;
            float duration;
            if (boost.ItemTier == 2 && npcDriver == null)
            {
                // thisDriver.sphere.velocity /= 8;
            }
            else if (npcDriver != null)
            {
                //npcDriver.DisableDriving();
                //npcDriver.velocity /= 8;
                //npcDriver.maxSpeed = 100;
                //npcDriver.accelerationRate = 500;
                //npcDriver.followTarget.GetComponent<SplineAnimate>().enabled = false;
            }

            duration = 3.0f;
                if (thisDriver != null)
                {
                    // different values and functionality for different levels of boosts
                    switch (boost.ItemTier)
                    {
                        default: // level 1
                            boostMult = 1.25f;
                            boostMaxSpeed = boostMult * 60;
                            StartCoroutine(ApplyBoost(thisDriver, boostMult, duration, boostMaxSpeed));
                            break;
                        case 2: // level 2
                            boostMult = 1.5f;
                            boostMaxSpeed = boostMult * 60;
                            StartCoroutine(ApplyBoost(thisDriver, boostMult, duration, boostMaxSpeed));
                            break;
                        case 3: // level 3
                            boostMult = 1.75f;
                            boostMaxSpeed = boostMult * 60;
                            StartCoroutine(ApplyBoostUpward(thisDriver, boostMult, duration, boostMaxSpeed));
                            break;
                        case 4: // level 4
                                // get the checkpoint from the kart's collider child to cross 3 checkpoints
                            GameObject kartParent = transform.parent.gameObject;
                            KartCheckpoint kartCheck = kartParent.GetComponentInChildren<KartCheckpoint>();
                            boostMult = 1.25f;
                            boostMaxSpeed = boostMult * 60;

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
                            // set the kart's position to 3 checkpoints ahead
                            thisDriver.sphere.transform.position = warpCheckpoint.transform.position;
                            thisDriver.transform.rotation = Quaternion.Euler(0, warpCheckpoint.transform.eulerAngles.y - 90, 0);
                            kartCheck.checkpointId = warpCheckpointId;
                            StartCoroutine(ApplyBoost(thisDriver, boostMult, duration, boostMaxSpeed));
                            break;
                    }
                    Debug.Log("Applying Boost Item!");
                }
                else if (npcDriver != null) // boost for npcs
                {
                    boostMult = 1.5f;
                    StartCoroutine(ApplyBoostNPC(npcDriver, boostMult, duration));
                    Debug.Log("Applying Boost Item!");
                }
                Destroy(collision.gameObject);
        }

        // checks if the kart drives into a hazard and drops the velocity to 1/8th of the previous value
        if (collision.gameObject.transform.tag == "Hazard")
        {
            if (thisDriver != null)
            {
                thisDriver.sphere.velocity /= 8000;

                // Checks if hazard is Confused Ritchie
                if (collision.gameObject.GetComponent<TrapItem>().ItemTier == 3)
                {
                    thisDriver.confusedTimer = 10;
                    thisDriver.isConfused = true;
                    thisDriver.movementDirection *= -1; // Just here to forces confusion to activate even if you don't change movement input
                }
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
            ApplyIconSpin(gameObject, 1);
            Destroy(collision.gameObject);
        }

    }
    public void ApplyItemTween(Texture item)
    {
        itemDisplay.rectTransform.DOKill();
        itemDisplay.rectTransform.localScale = itemDisplayScale;
        itemDisplay.texture = item;
        itemDisplay.rectTransform.DOShakeScale(0.5f);
    }

    public void ApplyIconSpin(GameObject obj, int times)
    {
        if (miniMap.spinInstances.Contains(currentSpinCoroutine) && currentSpinCoroutine != null)
        {
            miniMap.spinInstances.Remove(currentSpinCoroutine);
            StopCoroutine(currentSpinCoroutine);
            currentSpinCoroutine = null;
        }

        currentSpinCoroutine = miniMap.SpinIcon(obj, times);
        miniMap.spinInstances.Add(currentSpinCoroutine);
        StartCoroutine(currentSpinCoroutine);
    }

    IEnumerator ApplyBoost(NEWDriver driver, float boostForce, float duration, float boostMaxSpeed)
    {
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

        // get a list of the player's wheels for raycasting
        List<GameObject> wheels = new List<GameObject>();
        wheels.Add(GameObject.Find("Kart 1/Kart/Normal/Parent/KartModel/formulaCar_model1/SteerPivotBL"));
        wheels.Add(GameObject.Find("Kart 1/Kart/Normal/Parent/KartModel/formulaCar_model1/SteerPivotFL"));
        wheels.Add(GameObject.Find("Kart 1/Kart/Normal/Parent/KartModel/formulaCar_model1/SteerPivotFR"));
        wheels.Add(GameObject.Find("Kart 1/Kart/Normal/Parent/KartModel/formulaCar_model1/SteerPivotBR"));


        // driver.sphere.AddForce(driver.transform.up * 2, ForceMode.Impulse);
        RaycastHit hit;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            // perform a raycast at each wheel on the player's kart
            foreach (GameObject wheel in wheels)
            {
                if (Physics.Raycast(wheel.transform.position, wheel.transform.TransformDirection(-Vector3.up), 
                    out hit, length))
                {
                    // determine spring force
                    float forceAmount = HooksLawDampen(hit.distance);

                    // add force at each wheel position
                    driver.sphere.AddForceAtPosition(wheel.transform.up * forceAmount, wheel.transform.position);
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

        // reenable drift and ground check
        driver.canDrift = true;
        driver.doGroundCheck = true;

        // aligns the kart with the ground
        driver.AttemptDrift();
        driver.EndDrift();
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

    IEnumerator ApplyBoostNPC(NPCDriver driver, float boostForce, float duration)
    {
        SplineAnimate spline = driver.followTarget.gameObject.GetComponent<SplineAnimate>();

        // modify variables to increase balls max speed
        float originalSpeed = spline.MaxSpeed;
        float boostedSpeed = originalSpeed * boostForce;
        float progress = spline.NormalizedTime;

        // increase balls max speed
        spline.MaxSpeed = boostedSpeed;
        spline.NormalizedTime = progress;

        // apply boost to npc
        driver.maxSpeed = driver.TopMaxSpeed * boostForce;

        yield return new WaitForSeconds(duration);

        // set max speeds back to original values
        driver.maxSpeed = driver.TopMaxSpeed;
        spline.MaxSpeed = originalSpeed;
        spline.NormalizedTime = spline.NormalizedTime;

        driver.boosted = false;
    }
}
