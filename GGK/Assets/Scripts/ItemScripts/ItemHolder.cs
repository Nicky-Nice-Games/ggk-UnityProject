using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Pipes;
using System.Linq;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using UnityEngine.UI;
using UnityEngine.VFX;

public class ItemHolder : NetworkBehaviour
{
    #region variables
    private bool holdingItem;
    private bool canUpgrade = true;

    private int useCounter = 1;

    private GameObject thrownItem;

    [SerializeField] private NEWDriver thisDriver;

    [SerializeField] private NPCDriver npcDriver;

    [SerializeField] private BaseItem heldItem;

    [SerializeField] private float timer = 5.0f;

    [SerializeField] private RawImage itemDisplay;

    private Vector3 itemDisplayScale;
    private Vector2 itemDisplayPosition;

    [SerializeField] private Texture defaultItemDisplay;

    private BaseItem item;
    private int driverItemTier;
    public int uses;
    public SpeedCameraEffect camera;

    public float gravityForce;

    //the current coroutine animating spinning, to prevent double-ups
    private IEnumerator currentSpinCoroutine;
    public MiniMapHud miniMap;

    [SerializeField] private ParticleSystem hoverEffect;

    private ParticleSystem hoverParticleInstance;

    public List<VisualEffect> effects;

    public VisualEffect shieldEffect;

    public VisualEffect boostEffect;

    // [SerializeField]
    // private TextMesh heldItemText;

    // audio variables
    //private AudioSource soundPlayer;

    // [SerializeField]
    //AudioClip throwSound;

    public BaseItem HeldItem { get { return heldItem; } set { heldItem = value; } }
    public bool HoldingItem { get { return holdingItem; } set { holdingItem = value; } }
    public int DriverItemTier { get { return driverItemTier; } set { driverItemTier = value; } }

    #endregion

    // variables for multiplayer
    #region NetworkVariables
    public NetworkVariable<ItemType> currentItemType =
        new NetworkVariable<ItemType>(
            ItemType.NoItem,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public NetworkVariable<int> currentItemTier =
        new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public NetworkVariable<bool> currentCanUpgrade =
        new NetworkVariable<bool>(
            true,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
    #endregion

    public override void OnNetworkSpawn()
    {
        currentItemType.OnValueChanged += OnItemTypeChange;
        currentItemTier.OnValueChanged += OnItemTierChange;
        currentCanUpgrade.OnValueChanged += OnCanUpgradeChange;
    }

    public override void OnNetworkDespawn()
    {
        currentItemType.OnValueChanged -= OnItemTypeChange;
        currentItemTier.OnValueChanged -= OnItemTierChange;
        currentCanUpgrade.OnValueChanged -= OnCanUpgradeChange;
    }

    // Start is called before the first frame update
    void Start()
    {

        if (thisDriver)
        {
            // STOP
            foreach (VisualEffect vs in effects)
            {
                vs.Stop();
            }
            shieldEffect.Stop();
            boostEffect.Stop();
        }

        DOTween.Init();
        holdingItem = IsHoldingItem();

        timer = UnityEngine.Random.Range(5, 8);

        uses = 0;

        itemDisplay = CanvasHandler.instance.ItemDisplay;

        if (thisDriver)
        {
            itemDisplay.texture = defaultItemDisplay;
            itemDisplayScale = itemDisplay.rectTransform.localScale;
            itemDisplayPosition = itemDisplay.rectTransform.position;
        }

        //soundPlayer = GetComponent<AudioSource>();
        driverItemTier = 1;

        // new code 
        InitItemArray();
        ItemTier = 0;
        itemType = ItemType.NoItem;
        InitItemImageArray();
        ApplyItemTween(defaultItemDisplay);
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
            if (item.isTimed) // for shield
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
                    driverItemTier = 1;

                    // new code
                    ItemTier = 0;
                    itemType = ItemType.NoItem;
                    ApplyItemTween(defaultItemDisplay);
                }
            }
            else if (item.UseCount == 1 && !item.isTimed) // hazards, lv1,lv3,lv4 puck
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
                    driverItemTier = 1;

                    // new code
                    ItemTier = 0;
                    itemType = ItemType.NoItem;
                    ApplyItemTween(defaultItemDisplay);
                }
            }
            else if (item.UseCount > 1 && item.isTimed) //
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
                    driverItemTier = 1;

                    // new code
                    ItemTier = 0;
                    itemType = ItemType.NoItem;
                    ApplyItemTween(defaultItemDisplay);
                }
            }
            else if (item.UseCount >= 1 && !item.isTimed) //
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
                    driverItemTier = 1;

                    // new code
                    ItemTier = 0;
                    itemType = ItemType.NoItem;
                    ApplyItemTween(defaultItemDisplay);
                }
            }
        }
    }

    private bool IsHoldingItem()
    {
        return itemType != ItemType.NoItem;
        // if (heldItem != null)
        // {
        //     // heldItemText.text = $"Held Item: {heldItem}";
        //     return true;
        // }
        // else
        // {
        //     // heldItemText.text = $"Held Item:";
        //     return false;
        // }
    }

    private void ClearItem()
    {
        if (!IsSpawned)
        {
            itemType = ItemType.NoItem;
            ItemTier = 0;
            ApplyItemTween(defaultItemDisplay);
            canUpgrade = true;
            useCounter = 1;
        }
        else
        {
            currentItemType.Value = ItemType.NoItem;
            currentItemTier.Value = 0;
            currentCanUpgrade.Value = true;
        }
    }

    public void OnThrow(InputAction.CallbackContext context) // for players
    {
        if (!holdingItem) return;

        if (context.performed) // make sure input is only being read once
        {
            if (!IsSpawned || itemType == ItemType.Boost)
            {
                thrownItem = Instantiate(ItemArray[(int)itemType][ItemTier], transform.position, transform.rotation).gameObject;

                // get the baseitem script from the thrown item and set proper variables
                BaseItem thrownItemScript = thrownItem.GetComponent<BaseItem>();
                thrownItemScript.Kart = this;
                thrownItemScript.UseCount -= useCounter;
                thrownItemScript.timerEndCallback = ClearItem;

                if (thrownItemScript.UseCount == 0 && !thrownItemScript.isTimed) // get rid of item if use count is 0
                {
                    ClearItem();
                }
                else // disable upgrading if use count is more than one and the item has already been used
                {
                    canUpgrade = false;
                    useCounter++;
                }
            }
            else
            {
                if (!IsOwner) return;

                SpawnItemRpc(this, itemType, ItemTier, transform.position, transform.rotation);
            }
        }
    }

    /// <summary>
    /// Rpc for client to ask the network to spawn an item for it
    /// </summary>
    [Rpc(SendTo.Server, RequireOwnership = true)]
    private void SpawnItemRpc(NetworkBehaviourReference itemHolder, ItemType itemType, int itemTier, Vector3 position, Quaternion rotation, RpcParams rpcParams = default)
    {
        GameObject thrownItem = Instantiate(ItemArray[(int)itemType][itemTier], position, rotation).gameObject;
        NetworkObject thrownItemNetworkObject = thrownItem.GetComponent<NetworkObject>();
        thrownItemNetworkObject.Spawn();

        ulong senderClientID = rpcParams.Receive.SenderClientId;

        // ItemHolder kartScript = PlayerSpawner.instance.kartAndID[senderClientID];
        if (itemHolder.TryGet(out ItemHolder kartScript))
        {
            // get the baseitem script from the thrown item and set proper variables
            BaseItem thrownItemScript = thrownItem.GetComponent<BaseItem>();
            thrownItemScript.Kart = kartScript;
            thrownItemScript.UseCount -= useCounter;

            // shield subscribes to timer end callback while other items remove from inventory right away
            if (thrownItemScript.ItemCategory == "Shield")
            {
                thrownItemScript.timerEndCallback = kartScript.ClearItem;
            }
            else
            {
                currentItemType.Value = ItemType.NoItem;
            }

            //currentItemType.Value = ItemType.NoItem;
            //currentItemTier.Value = 0;
        }
    }

    public void OnThrow() // for npcs
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
        timer = UnityEngine.Random.Range(5, 8);

        Debug.Log(item.ItemTier);

    }

    public void OnTriggerEnter(Collider collision)
    {
        // Checks if kart hits an item box
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
                        // Get projectile and intialize level
                        // projBrick.GiveProjectile(this.gameObject);
                        // heldItem.ItemTier = driverItemTier;
                        // heldItem.OnLevelUp(heldItem.ItemTier);
                        // uses = heldItem.UseCount;

                        // new code
                        itemType = ItemType.Puck;

                        if (MultiplayerManager.Instance.IsMultiplayer)
                        {
                            currentItemType.Value = ItemType.Puck;
                        }
                        else
                        {
                            // Item Box Shake
                            if (thisDriver)
                            {
                                //ApplyItemTween(heldItem.itemIcon);
                                // new code
                                ApplyItemTween(ItemImageArray[(int)itemType][ItemTier]);

                            }
                        }

                    }
                    else if (itemType == ItemType.Puck)
                    {
                        // Increase item tier & apply upgrades
                        if (ItemTier < 4)
                        {
                            // driverItemTier++;
                            // heldItem.ItemTier = driverItemTier;
                            // heldItem.OnLevelUp(heldItem.ItemTier);
                            // uses = heldItem.UseCount;

                            // new code
                            if (MultiplayerManager.Instance.IsMultiplayer && canUpgrade)
                            {
                                currentItemTier.Value++;
                            }
                            else if (!MultiplayerManager.Instance.IsMultiplayer && canUpgrade)
                            {
                                ItemTier++;
                            }
                        }
                        // Item Box Shake
                        if (thisDriver)
                        {
                            //ApplyItemTween(heldItem.itemIcon);
                            // new code
                            ApplyItemTween(ItemImageArray[(int)itemType][ItemTier]);
                        }
                    }
                    break;
                case "Boost":
                    BoostBrick boostBrick = collision.gameObject.GetComponent<BoostBrick>();
                    if (!holdingItem)
                    {
                        // Get boost and intialize level
                        // boostBrick.GiveBoost(this.gameObject);
                        // heldItem.ItemTier = driverItemTier;
                        // heldItem.OnLevelUp(heldItem.ItemTier);
                        // uses = heldItem.UseCount;

                        // new code
                        itemType = ItemType.Boost;

                        if (MultiplayerManager.Instance.IsMultiplayer)
                        {
                            currentItemType.Value = ItemType.Boost;
                        }
                        else
                        {
                            // Item Box Shake
                            if (thisDriver)
                            {
                                //ApplyItemTween(heldItem.itemIcon);
                                // new code
                                ApplyItemTween(ItemImageArray[(int)itemType][ItemTier]);

                            }
                        }

                    }
                    else if (itemType == ItemType.Boost)
                    {
                        // Increase item tier & apply upgrades
                        if (ItemTier < 4)
                        {
                            // driverItemTier++;
                            // heldItem.ItemTier = driverItemTier;
                            // heldItem.OnLevelUp(heldItem.ItemTier);
                            // uses = heldItem.UseCount;

                            // new code
                            if (MultiplayerManager.Instance.IsMultiplayer && canUpgrade)
                            {
                                currentItemTier.Value++;
                            }
                            else if (!MultiplayerManager.Instance.IsMultiplayer && canUpgrade)
                            {
                                ItemTier++;
                            }
                        }
                        // Item Box Shake
                        if (thisDriver)
                        {
                            //ApplyItemTween(heldItem.itemIcon);
                            // new code
                            ApplyItemTween(ItemImageArray[(int)itemType][ItemTier]);
                        }
                    }
                    break;
                case "Defense":
                    DefenseBrick defBrick = collision.gameObject.GetComponent<DefenseBrick>();
                    if (!holdingItem)
                    {
                        // Get shield and intialize level
                        // defBrick.GiveShield(this.gameObject);
                        // heldItem.ItemTier = driverItemTier;
                        // heldItem.OnLevelUp(heldItem.ItemTier);
                        // uses = heldItem.UseCount;

                        // new code
                        itemType = ItemType.Shield;

                        if (MultiplayerManager.Instance.IsMultiplayer)
                        {
                            currentItemType.Value = ItemType.Shield;
                        }
                        else
                        {
                            // Item Box Shake
                            if (thisDriver)
                            {
                                //ApplyItemTween(heldItem.itemIcon);
                                // new code
                                ApplyItemTween(ItemImageArray[(int)itemType][ItemTier]);

                            }
                        }

                    }
                    else if (itemType == ItemType.Shield)
                    {
                        // Do not upgrade if shield is being used
                        if (uses > 0)
                        {
                            // Increase item tier & apply upgrades
                            if (ItemTier < 4)
                            {
                                // driverItemTier++;
                                // heldItem.ItemTier = driverItemTier;
                                // heldItem.OnLevelUp(heldItem.ItemTier);
                                // uses = heldItem.UseCount;

                                // new code
                                if (MultiplayerManager.Instance.IsMultiplayer && canUpgrade)
                                {
                                    currentItemTier.Value++;
                                }
                                else if (!MultiplayerManager.Instance.IsMultiplayer && canUpgrade)
                                {
                                    ItemTier++;
                                }
                            }
                            // Item Box Shake
                            if (thisDriver)
                            {
                                //ApplyItemTween(heldItem.itemIcon);
                                // new code
                                ApplyItemTween(ItemImageArray[(int)itemType][ItemTier]);
                            }
                        }
                    }
                    break;
                case "Hazard":
                    HazardBrick hazBrick = collision.gameObject.GetComponent<HazardBrick>();
                    if (!holdingItem)
                    {
                        // Get hazard and intialize level
                        // hazBrick.GiveHazard(this.gameObject);
                        // heldItem.ItemTier = driverItemTier;
                        // heldItem.OnLevelUp(heldItem.ItemTier);
                        // uses = heldItem.UseCount;

                        // new code
                        itemType = ItemType.Hazard;

                        if (MultiplayerManager.Instance.IsMultiplayer)
                        {
                            currentItemType.Value = ItemType.Hazard;
                        }
                        else
                        {
                            // Item Box Shake
                            if (thisDriver)
                            {
                                //ApplyItemTween(heldItem.itemIcon);
                                // new code
                                ApplyItemTween(ItemImageArray[(int)itemType][ItemTier]);

                            }
                        }

                    }
                    else if (itemType == ItemType.Hazard)
                    {
                        // Increase item tier & apply upgrades
                        if (ItemTier < 4)
                        {
                            // driverItemTier++;
                            // heldItem.ItemTier = driverItemTier;
                            // heldItem.OnLevelUp(heldItem.ItemTier);
                            // uses = heldItem.UseCount;

                            // new code
                            if (MultiplayerManager.Instance.IsMultiplayer && canUpgrade)
                            {
                                currentItemTier.Value++;
                            }
                            else if (!MultiplayerManager.Instance.IsMultiplayer && canUpgrade)
                            {
                                ItemTier++;
                            }
                        }
                        // Item Box Shake
                        if (thisDriver)
                        {
                            //ApplyItemTween(heldItem.itemIcon);
                            // new code
                            ApplyItemTween(ItemImageArray[(int)itemType][ItemTier]);
                        }
                    }
                    break;
                case "Upgrade":
                    if (!holdingItem)
                    {
                        // Increase item tier if not max
                        if (ItemTier < 4)
                        {
                            driverItemTier++;
                        }

                        // new code
                        if (canUpgrade) ItemTier++;
                    }
                    else
                    {
                        // shield can't be upgraded while being used
                        if (itemType != ItemType.Shield)
                        {
                            // Increase item tier if not max & apply upgrades
                            if (ItemTier < 4)
                            {
                                // driverItemTier++;
                                // heldItem.ItemTier = driverItemTier;
                                // heldItem.OnLevelUp(heldItem.ItemTier);
                                // uses = heldItem.UseCount;

                                // new code
                                if (MultiplayerManager.Instance.IsMultiplayer && canUpgrade)
                                {
                                    currentItemTier.Value++;
                                }
                                else if (!MultiplayerManager.Instance.IsMultiplayer && canUpgrade)
                                {
                                    ItemTier++;
                                }
                            }
                            // Item Box Shake
                            if (thisDriver)
                            {
                                //ApplyItemTween(heldItem.itemIcon);
                                // new code
                                ApplyItemTween(ItemImageArray[(int)itemType][ItemTier]);
                            }
                        }
                        else if (uses == 1)
                        {
                            // Increase item tier if not max & apply upgrades
                            if (ItemTier < 4)
                            {
                                // driverItemTier++;
                                // heldItem.ItemTier = driverItemTier;
                                // heldItem.OnLevelUp(heldItem.ItemTier);
                                // uses = heldItem.UseCount;

                                // new code
                                if (MultiplayerManager.Instance.IsMultiplayer && canUpgrade)
                                {
                                    currentItemTier.Value++;
                                }
                                else if (!MultiplayerManager.Instance.IsMultiplayer && canUpgrade)
                                {
                                    ItemTier++;
                                }
                            }
                            // Item Box Shake
                            if (thisDriver)
                            {
                                //ApplyItemTween(heldItem.itemIcon);
                                // new code
                                ApplyItemTween(ItemImageArray[(int)itemType][ItemTier]);
                            }
                        }
                    }
                    break;
                default:
                    // Gives kart an item if they don't already have one
                    if (!holdingItem)
                    {
                        //itemBox.RandomizeItem(this.gameObject);
                        itemType = RandomItemType();

                        // Initialize use count if first use
                        // if (uses == 0)
                        // {
                        //     uses = heldItem.UseCount;
                        // }
                        Debug.Log(uses);
                        if (thisDriver)
                        {
                            //ApplyItemTween(heldItem.itemIcon);
                            // new code
                            ApplyItemTween(ItemImageArray[(int)itemType][ItemTier]);
                        }
                    }
                    break;
            }
            // Disables the item box
            itemBox.gameObject.SetActive(false);
        }
        if (collision.gameObject.CompareTag("Projectile"))
        {
            if (thisDriver != null)
                thisDriver.sphere.velocity /= 8;
            Destroy(collision.gameObject);
            ApplyIconSpin(gameObject, 1);
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
        if (IsSpawned && !IsOwner) return;
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

    private void OnCanUpgradeChange(bool previousValue, bool newValue)
    {
        canUpgrade = newValue;
    }

    #region New Item Variables
    [Header("ItemArray")]
    [SerializeField] private Transform[] BoostArray;
    [SerializeField] private Transform[] ShieldArray;
    [SerializeField] private Transform[] HazardArray;
    [SerializeField] private Transform[] PuckArray;
    private List<Transform[]> ItemArray = new List<Transform[]>();


    [Header("ItemImageArray")]
    [SerializeField] private Texture[] BoostImageArray;
    [SerializeField] private Texture[] ShieldImageArray;
    [SerializeField] private Texture[] HazardImageArray;
    [SerializeField] private Texture[] PuckImageArray;
    private List<Texture[]> ItemImageArray = new List<Texture[]>();

    [Header("Held Item")]
    [SerializeField] private int tier = 0;
    public int ItemTier
    {
        get { return tier; }
        set
        {
            if (tier + value > maxTier) tier = maxTier;
            else tier = value;
        }
    }
    const int maxTier = 3;
    public enum ItemType { NoItem = -1, Boost = 0, Shield = 1, Hazard = 2, Puck = 3 }
    [SerializeField] private ItemType itemType = ItemType.NoItem;
    #endregion

    private void InitItemArray()
    {
        ItemArray.Add(BoostArray);
        ItemArray.Add(ShieldArray);
        ItemArray.Add(HazardArray);
        ItemArray.Add(PuckArray);
    }

    private void InitItemImageArray()
    {
        ItemImageArray.Add(BoostImageArray);
        ItemImageArray.Add(ShieldImageArray);
        ItemImageArray.Add(HazardImageArray);
        ItemImageArray.Add(PuckImageArray);

    }

    private ItemType RandomItemType()
    {
        return (ItemType)UnityEngine.Random.Range(0, (int)Enum.GetValues(typeof(ItemType)).Cast<ItemType>().Max() + 1);
    }

    private void OnItemTypeChange(ItemType previousValue, ItemType newValue)
    {
        // make sure only the client who changes an item calls this
        if (!IsOwner) return;
        itemType = newValue;
        if (newValue == ItemType.NoItem)
        {
            ApplyItemTween(defaultItemDisplay);

            // makes sure clear item doesn't get called infinitely
            if (previousValue != ItemType.NoItem)
            {
                ClearItem();
            }
        }
        else
        {
            ApplyItemTween(ItemImageArray[(int)currentItemType.Value][currentItemTier.Value]);
        }
    }

    private void OnItemTierChange(int previousValue, int newValue)
    {
        // make sure only the client who changes an item calls this
        if (!IsOwner) return;
        ItemTier = newValue;
        if (itemType == ItemType.NoItem)
        {
            ApplyItemTween(defaultItemDisplay);
        }
        else
        {
            ApplyItemTween(ItemImageArray[(int)currentItemType.Value][currentItemTier.Value]);
        }
        
    }
}
