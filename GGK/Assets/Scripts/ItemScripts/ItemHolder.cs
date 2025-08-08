using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.VFX;

public class ItemHolder : NetworkBehaviour
{
    #region variables

    private bool canUpgrade = true;

    private int useCounter = 1;

    private GameObject thrownItem;

    [SerializeField] private NEWDriver thisDriver;

    [SerializeField] private NPCPhysics npcDriver;

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

    //the current coroutine animating spinning, to prevent double-ups
    private IEnumerator currentSpinCoroutine;
    public MiniMapHud miniMap;

    public List<VisualEffect> effects;

    public VisualEffect shieldEffect;

    public VisualEffect boostEffect;

    public BaseItem HeldItem { get { return heldItem; } set { heldItem = value; } }

    #endregion

//AUDIO1
    [Header("Wwise Sounds")]
    public KartSounds kartSounds;
    private uint shieldID = 0;
    public VoiceLines voiceLines;

    #region ItemTracking Variables
    [Header("Held Item")]
    [SerializeField] private int tier = 0;
    public int ItemTier
    {
        get { return tier; }
        set
        {
            if (tier + value > maxTier) {
                tier = maxTier;
            }
            else {
                tier = value;
            }
            if (type == ItemTypeEnum.NoItem) {
                ApplyItemTween(defaultItemDisplay);
            }
            else {
                if (thisDriver) // only drivers can apply item display
                {
                    ApplyItemTween(ItemImageArray[(int)type][tier]);
                }
            }
        }
    }
    const int maxTier = 3;
    public enum ItemTypeEnum { NoItem = -1, Boost = 0, Shield = 1, Hazard = 2, Puck = 3 }
    [SerializeField] private ItemTypeEnum type = ItemTypeEnum.NoItem;
    public ItemTypeEnum ItemType
    {
        get { return type; }
        set
        {
            type = value;
            if (value == ItemTypeEnum.NoItem) {
                ApplyItemTween(defaultItemDisplay);
            } else {
                if (thisDriver) // only drivers can apply item display
                {
                    ApplyItemTween(ItemImageArray[(int)type][tier]);
                }
            }
        }
    }
    #endregion

    #region ItemArray Variables
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
    #endregion
    
    // variables for multiplayer
    #region NetworkVariables
    public NetworkVariable<ItemTypeEnum> currentItemType =
        new NetworkVariable<ItemTypeEnum>(
            ItemTypeEnum.NoItem,
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

    public NetworkVariable<int> currentUseCounter =
        new NetworkVariable<int>(
            1,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
    #endregion
    
    #region Initialization functions
    // Start is called before the first frame update
    private void Start()
    {
        // get the npc physics script off npcs
        if (this.gameObject.GetComponent<NPCPhysics>() != null)
        {
            npcDriver = this.gameObject.GetComponent<NPCPhysics>();
        }

        if (thisDriver)
        {
            // STOP
            // foreach (VisualEffect vs in effects)
            // {
                // vs.Stop();
            // }
            // shieldEffect.Stop();
            // boostEffect.Stop();
        }

        DOTween.Init();

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

        // new code 
        InitItemArray();
        InitItemImageArray();
        ItemTier = 0;
        ItemType = ItemTypeEnum.NoItem;
        ApplyItemTween(defaultItemDisplay);
    }

    public override void OnNetworkSpawn()
    {
        currentItemType.OnValueChanged += OnItemTypeChange;
        currentItemTier.OnValueChanged += OnItemTierChange;
        currentCanUpgrade.OnValueChanged += OnCanUpgradeChange;
        currentUseCounter.OnValueChanged += OnUseCounterChange;
    }

    public override void OnNetworkDespawn()
    {
        currentItemType.OnValueChanged -= OnItemTypeChange;
        currentItemTier.OnValueChanged -= OnItemTierChange;
        currentCanUpgrade.OnValueChanged -= OnCanUpgradeChange;
        currentUseCounter.OnValueChanged -= OnUseCounterChange;
    }

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
    #endregion

    // Update is called once per frame
    private void Update()
    {
        if (npcDriver != null && IsHoldingItem())
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


        if (IsHoldingItem() && item)
        {
            if (item.isTimed) // for shield
            {
                if (item.Timer <= 0.0f)
                {
                    if (thisDriver)
                    {
                        itemDisplay.texture = defaultItemDisplay;
                    }
                    Destroy(item.gameObject);
                    Destroy(heldItem.gameObject);
                    driverItemTier = 1;

                    // new code
                    ItemTier = 0;
                    ItemType = ItemTypeEnum.NoItem;
                    ApplyItemTween(defaultItemDisplay);
                }
            }
            else if (item.UseCount == 1 && !item.isTimed) // hazards, lv1,lv3,lv4 puck
            {
                if (uses == 0)
                {
                    if (thisDriver)
                    {
                        itemDisplay.texture = defaultItemDisplay;
                    }
                    Destroy(heldItem.gameObject);
                    driverItemTier = 1;

                    // new code
                    ItemTier = 0;
                    ItemType = ItemTypeEnum.NoItem;
                    ApplyItemTween(defaultItemDisplay);
                }
            }
            else if (item.UseCount > 1 && item.isTimed) //
            {
                if (item.Timer <= 0.0f || uses == 0)
                {
                    if (thisDriver)
                    {
                        itemDisplay.texture = defaultItemDisplay;
                    }
                    Destroy(item.gameObject);
                    Destroy(heldItem.gameObject);
                    driverItemTier = 1;

                    // new code
                    ItemTier = 0;
                    ItemType = ItemTypeEnum.NoItem;
                    ApplyItemTween(defaultItemDisplay);
                }
            }
            else if (item.UseCount >= 1 && !item.isTimed) //
            {
                if (uses == 0)
                {
                    if (thisDriver)
                    {
                        itemDisplay.texture = defaultItemDisplay;
                    }
                    Destroy(heldItem.gameObject);
                    driverItemTier = 1;

                    // new code
                    ItemTier = 0;
                    ItemType = ItemTypeEnum.NoItem;
                    ApplyItemTween(defaultItemDisplay);
                }
            }
        }
    }

    private bool IsHoldingItem()
    {
        return ItemType != ItemTypeEnum.NoItem;
    }

    private void ClearItem()
    {
        if (!IsSpawned)
        {
            //Debug.Log("single player clear item");
            ItemType = ItemTypeEnum.NoItem;
            ItemTier = 0;
            canUpgrade = true;
            useCounter = 1;
        }
        else
        {
            //Debug.Log("network clear item");
            currentItemType.Value = ItemTypeEnum.NoItem;
            currentItemTier.Value = 0;
            currentCanUpgrade.Value = true;
            currentUseCounter.Value = 1;
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void ClearItemRpc(){
        ClearItem();
    }

    public void OnThrow(InputAction.CallbackContext context) // for players
    {
        if (!IsHoldingItem()) return;

        // prevent player from spamming shield while it's active
        if (useCounter > 1 && ItemType == ItemTypeEnum.Shield) return;

        if (context.performed) // make sure input is only being read once
        {
            if (voiceLines != null && (ItemType == ItemTypeEnum.Puck || ItemType == ItemTypeEnum.Hazard))
            {
                voiceLines.PlayItemThrown();
            }

            if (!IsSpawned) {
                
                thrownItem = Instantiate(ItemArray[(int)ItemType][ItemTier], transform.position, transform.rotation).gameObject;

                // get the baseitem script from the thrown item and set proper variables
                BaseItem thrownItemScript = thrownItem.GetComponent<BaseItem>();
                thrownItemScript.Kart = this;
                thrownItemScript.UseCount -= useCounter;
                thrownItemScript.timerEndCallback = ClearItem;

//AUDIO2
                if (thrownItemScript.UseCount <= 0 && !thrownItemScript.isTimed) // get rid of item if use count is 0
                {
                    ClearItem();
                }
                else // disable upgrading if use count is more than one and the item has already been used
                {
                    canUpgrade = false;
                    useCounter++;
                }
            } else if(IsSpawned && ItemType == ItemTypeEnum.Boost){
                thrownItem = Instantiate(ItemArray[(int)ItemType][ItemTier], transform.position, transform.rotation).gameObject;

                // get the baseitem script from the thrown item and set proper variables
                BaseItem thrownItemScript = thrownItem.GetComponent<BaseItem>();
                thrownItemScript.Kart = this;
                thrownItemScript.UseCount -= useCounter;
                thrownItemScript.timerEndCallback = ClearItemRpc;

                if (thrownItemScript.UseCount == 0 && !thrownItemScript.isTimed) // get rid of item if use count is 0
                {
                    ClearItemRpc();
                }
                else // disable upgrading if use count is more than one and the item has already been used
                {
                    canUpgrade = false;
                    useCounter++;
                }
            } else {
                if (!IsOwner) return;

                SpawnItemRpc(this, ItemType, ItemTier, transform.position, transform.rotation);
            }
        }
    }

    /// <summary>
    /// Rpc for client to ask the network to spawn an item for it
    /// </summary>
    [Rpc(SendTo.Server, RequireOwnership = true)]
    private void SpawnItemRpc(NetworkBehaviourReference itemHolder, ItemTypeEnum itemType, int itemTier, Vector3 position, Quaternion rotation, RpcParams rpcParams = default)
    {
        // ItemHolder kartScript = PlayerSpawner.instance.kartAndID[senderClientID];
        if (itemHolder.TryGet(out ItemHolder kartScript))
        {
            GameObject thrownItem = Instantiate(ItemArray[(int)itemType][itemTier], position, rotation).gameObject;
            NetworkObject thrownItemNetworkObject = thrownItem.GetComponent<NetworkObject>();
            thrownItemNetworkObject.Spawn();
            
            // get the baseitem script from the thrown item and set proper variables
            BaseItem thrownItemScript = thrownItem.GetComponent<BaseItem>();
            thrownItemScript.Kart = kartScript;
            thrownItemScript.UseCount -= kartScript.currentUseCounter.Value;
            thrownItemScript.timerEndCallback = kartScript.ClearItem;

            if (thrownItemScript.UseCount <= 0 && !thrownItemScript.isTimed) // get rid of item if use count is 0
            {
                ClearItem();
            }
            else // disable upgrading if use count is more than one and the item has already been used
            {
                kartScript.currentCanUpgrade.Value = false;
                kartScript.currentUseCounter.Value++;
            }
        }
    }

//AUDIO3
    public void OnThrow() // for npcs
    {
        // handles item usage
        if (!IsSpawned)
        {
            thrownItem = Instantiate(ItemArray[(int)ItemType][ItemTier], transform.position, transform.rotation).gameObject;

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
        else if (IsSpawned && ItemType == ItemTypeEnum.Boost)
        {
            thrownItem = Instantiate(ItemArray[(int)ItemType][ItemTier], transform.position, transform.rotation).gameObject;

            // get the baseitem script from the thrown item and set proper variables
            BaseItem thrownItemScript = thrownItem.GetComponent<BaseItem>();
            thrownItemScript.Kart = this;
            thrownItemScript.UseCount -= useCounter;
            thrownItemScript.timerEndCallback = ClearItemRpc;

            if (thrownItemScript.UseCount == 0 && !thrownItemScript.isTimed) // get rid of item if use count is 0
            {
                ClearItemRpc();
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

            thrownItem = Instantiate(ItemArray[(int)ItemType][ItemTier], transform.position, transform.rotation).gameObject;

            NetworkObject thrownItemNetworkObject = thrownItem.GetComponent<NetworkObject>();
            thrownItemNetworkObject.Spawn();

            // get the baseitem script from the thrown item and set proper variables
            BaseItem thrownItemScript = thrownItem.GetComponent<BaseItem>();
            thrownItemScript.Kart = this;
            thrownItemScript.UseCount -= this.currentUseCounter.Value;
            thrownItemScript.timerEndCallback = this.ClearItem;

            if (thrownItemScript.UseCount == 0 && !thrownItemScript.isTimed) // get rid of item if use count is 0
            {
                ClearItem();
            }
            else // disable upgrading if use count is more than one and the item has already been used
            {
                this.currentCanUpgrade.Value = false;
                this.currentUseCounter.Value++;
            }
        }
        timer = UnityEngine.Random.Range(5, 8);

    }

    public void OnTriggerEnter(Collider collision)
    {
        // Checks if kart hits an item box
        if (collision.gameObject.CompareTag("ItemBox"))
        {
            Debug.Log("Collided with ItemBox");
            if(kartSounds != null)
            {
                kartSounds.PlayItemPickup();
            }
            else
            {
                Debug.Log("No KartSounds script found.");
            }
            ItemBox itemBox = collision.gameObject.GetComponent<ItemBox>();
            //Debug.Log(itemBox.ItemBoxType);
            switch (itemBox.ItemType)
            {
                case ItemTypeEnum.Puck:
                    ProjectileBrick projBrick = collision.gameObject.GetComponent<ProjectileBrick>();
                    if (!IsHoldingItem())
                    {
                        if (IsSpawned && !IsServer)
                        {
                            return;
                        }
                        else if (IsSpawned && IsServer)
                        {
                            currentItemType.Value = ItemTypeEnum.Puck;
                        }
                        else
                        {
                            ItemType = ItemTypeEnum.Puck;
                        }
                        // ItemType = ItemTypeEnum.Puck;
                        // if (MultiplayerManager.Instance.IsMultiplayer)
                        // {
                        //     currentItemType.Value = ItemTypeEnum.Puck;
                        // }
                    }
                    else if (ItemType == ItemTypeEnum.Puck)
                    {
                        if (ItemTier < 4)
                        {
                            if (IsSpawned && !IsServer)
                            {
                                return;
                            }
                            else if (IsSpawned && IsServer)
                            {
                                if (currentCanUpgrade.Value) currentItemTier.Value++;
                            }
                            else
                            {
                                if (canUpgrade) ItemTier++;
                            }
                        }
                    }
                    break;
                case ItemTypeEnum.Boost:
                    BoostBrick boostBrick = collision.gameObject.GetComponent<BoostBrick>();
                    if (!IsHoldingItem())
                    {
                        if (IsSpawned && !IsServer)
                        {
                            return;
                        }
                        else if (IsSpawned && IsServer)
                        {
                            currentItemType.Value = ItemTypeEnum.Boost;
                        }
                        else
                        {
                            ItemType = ItemTypeEnum.Boost;
                        }
                    }
                    else if (ItemType == ItemTypeEnum.Boost)
                    {
                        if (ItemTier < 4)
                        {
                            if (IsSpawned && !IsServer)
                            {
                                return;
                            }
                            else if (IsSpawned && IsServer)
                            {
                                if (currentCanUpgrade.Value) currentItemTier.Value++;
                            }
                            else
                            {
                                if (canUpgrade) ItemTier++;
                            }
                        }
                    }
                    break;
                case ItemTypeEnum.Shield:
                    DefenseBrick defBrick = collision.gameObject.GetComponent<DefenseBrick>();
                    if (!IsHoldingItem())
                    {
                        if (IsSpawned && !IsServer)
                        {
                            return;
                        }
                        else if (IsSpawned && IsServer)
                        {
                            currentItemType.Value = ItemTypeEnum.Shield;
                        }
                        else
                        {
                            ItemType = ItemTypeEnum.Shield;
                        }
                    }
                    else if (ItemType == ItemTypeEnum.Shield)
                    {
                        if (ItemTier < 4)
                        {
                            if (IsSpawned && !IsServer)
                            {
                                return;
                            }
                            else if (IsSpawned && IsServer)
                            {
                                if (currentCanUpgrade.Value) currentItemTier.Value++;
                            }
                            else
                            {
                                if (canUpgrade) ItemTier++;
                            }
                        }
                        // // Do not upgrade if shield is being used
                        // if (useCounter == 1)
                        // {
                        //     // Increase item tier & apply upgrades
                        //     if (ItemTier < 4)
                        //     {
                        //         // new code
                        //         if (MultiplayerManager.Instance.IsMultiplayer && canUpgrade)
                        //         {
                        //             currentItemTier.Value++;
                        //         }
                        //         else if (!MultiplayerManager.Instance.IsMultiplayer && canUpgrade)
                        //         {
                        //             ItemTier++;
                        //         }
                        //     }
                        // }
                    }
                    break;
                case ItemTypeEnum.Hazard:
                    HazardBrick hazBrick = collision.gameObject.GetComponent<HazardBrick>();
                    if (!IsHoldingItem())
                    {
                        if (IsSpawned && !IsServer)
                        {
                            return;
                        }
                        else if (IsSpawned && IsServer)
                        {
                            currentItemType.Value = ItemTypeEnum.Hazard;
                        }
                        else
                        {
                            ItemType = ItemTypeEnum.Hazard;
                        }
                    }
                    else if (ItemType == ItemTypeEnum.Hazard)
                    {
                        if (ItemTier < 4)
                        {
                            if (IsSpawned && !IsServer)
                            {
                                return;
                            }
                            else if (IsSpawned && IsServer)
                            {
                                if (currentCanUpgrade.Value) currentItemTier.Value++;
                            }
                            else
                            {
                                if (canUpgrade) ItemTier++;
                            }
                        }
                    }
                    break;
                case ItemTypeEnum.NoItem:
                    if (!IsHoldingItem())
                    {
                        // Increase item tier if not max
                        if (canUpgrade)
                        {
                            if (IsSpawned && !IsServer)
                            {
                                return;
                            }
                            else if (IsSpawned && IsServer)
                            {
                                currentItemTier.Value++;
                            }
                            else
                            {
                                ItemTier++;
                            }
                        }

                    }
                    else
                    {
                        // shield can't be upgraded while being used
                        if (ItemType != ItemTypeEnum.Shield)
                        {
                            // Increase item tier if not max & apply upgrades
                            if (ItemTier < 4)
                            {
                                if (IsSpawned && !IsServer)
                                {
                                    return;
                                }
                                else if (IsSpawned && IsServer)
                                {
                                    currentItemTier.Value++;
                                }
                                else
                                {
                                    ItemTier++;
                                }
                            }

                        }
                        else if (useCounter == 1)
                        {
                            // Increase item tier if not max & apply upgrades
                            if (ItemTier < 4)
                            {
                                if (IsSpawned && !IsServer)
                                {
                                    return;
                                }
                                else if (IsSpawned && IsServer)
                                {
                                    currentItemTier.Value++;
                                }
                                else
                                {
                                    ItemTier++;
                                }
                            }
                        }
                    }
                    break;
                default:
                    if (!IsHoldingItem())
                    {
                    // Gives kart an item if they don't already have one
                        if (IsSpawned && !IsServer)
                        {
                            return;
                        }
                        else if (IsSpawned && IsServer)
                        {
                            currentItemType.Value = RandomItemType();
                        }
                        else
                        {
                            ItemType = RandomItemType();
                        }
                    }
                    break;
            }
//AUDIO4
            // Disables the item box and respawns it after some time
            if (IsSpawned && !IsServer)
            {
                return;
            }
//AUDIO5
            else if (IsSpawned && IsServer)
            {
                itemBox.StartTimerRpc();
            }
            else
            {
                itemBox.StartTimer();
            }
//AUDIO6
        }
    }

    public void ApplyItemTween(Texture item)
    {
        if (IsSpawned && !IsOwner) return;
        if (npcDriver) return;
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
//AUDIO7

    public static ItemTypeEnum RandomItemType()
    {
        return (ItemTypeEnum)UnityEngine.Random.Range(0, (int)Enum.GetValues(typeof(ItemTypeEnum)).Cast<ItemTypeEnum>().Max() + 1);
    }

    private void OnItemTypeChange(ItemTypeEnum previousValue, ItemTypeEnum newValue)
    {
        Debug.Log("OnItemTypeChange");
        ItemType = newValue;
    }

    private void OnItemTierChange(int previousValue, int newValue)
    {
        Debug.Log("OnItemTierChange");
        ItemTier = newValue;
    }

    private void OnCanUpgradeChange(bool previousValue, bool newValue)
    {
        Debug.Log("OnCanUpgradeChange");
        canUpgrade = newValue;
    }
    
    private void OnUseCounterChange(int previousValue, int newValue)
    {
        Debug.Log("OnUseCounterChange");
        useCounter = newValue;
    }
}

/* AUDIO2
            item = Instantiate(heldItem, transform.position, transform.rotation);
            if (heldItem.ItemCategory == "Shield")
            {
                shieldID = AkUnitySoundEngine.PostEvent("Play_Shield", this.gameObject);

                StartCoroutine(StopPlayingShieldNoise(heldItem.ItemTier));
            }
            //soundPlayer.PlayOneShot(throwSound);
            item.gameObject.SetActive(true);
            item.Kart = this;
            item.ItemTier = heldItem.ItemTier;
*/

/* AUDIO3
    IEnumerator StopPlayingShieldNoise(int itemTier)
    {
        int activeSeconds = 0;

        if(itemTier == 1)
        {
            activeSeconds = 4;
        }
        else if(itemTier == 2)
        {
            activeSeconds = 6;
        }
        else if(itemTier == 3)
        {
            activeSeconds = 8;
        }
        else if(itemTier == 4)
        {
            activeSeconds = 10;
        }

        yield return new WaitForSeconds(activeSeconds);

        AkUnitySoundEngine.StopPlayingID(shieldID);
    }

    public void OnThrow()
*/

/*AUDIO4
            // Disables the item box
            itemBox.gameObject.SetActive(false);
        }
        if (collision.gameObject.CompareTag("Projectile"))
        {
            if (thisDriver != null)
                thisDriver.sphere.velocity /= 8;
            Destroy(collision.gameObject);
            ApplyIconSpin(gameObject, 1);
            if (kartSounds != null)
            {
                kartSounds.PlayHitSpinout();
                kartSounds.PlayCrash();
            }
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
*/

/* AUDIO5
            duration = 3.0f;
                if (thisDriver != null)
                {
                    if (kartSounds != null)
                    {
                        kartSounds.PlayCanOpen();
                    }
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

                            // Makes game object with wormhole effect appear
                            warpBoostEffect.SetActive(true);

                            // Waits a certain number of seconds, and then activates the warp boost
                            StartCoroutine(WaitThenBoost(warpCheckpoint, kartCheck, warpCheckpointId,
                                           boostMult, duration, boostMaxSpeed));

                            //// set the kart's position to 3 checkpoints ahead
                            //thisDriver.sphere.transform.position = warpCheckpoint.transform.position;
                            //thisDriver.transform.rotation = Quaternion.Euler(0, warpCheckpoint.transform.eulerAngles.y - 90, 0);
                            //kartCheck.checkpointId = warpCheckpointId;
                            //StartCoroutine(ApplyBoost(thisDriver, boostMult, duration, boostMaxSpeed));
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
                ApplyIconSpin(gameObject, 3);
*/

/* AUDIO6
            ApplyIconSpin(gameObject, 1);
            if (kartSounds != null) { kartSounds.PlayHitSpinout(); }
            Destroy(collision.gameObject);
        }

    }
    
    //Waits a certain number of seconds then activates the warp boost
    IEnumerator WaitThenBoost(GameObject warpCheckpoint, KartCheckpoint kartCheck, int warpCheckpointId,
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
            yield return new WaitForSeconds(warpWaitTime);
            thisDriver.sphere.velocity = originalVelocity;
            thisDriver.movementDirection = originalMovement;
*/

/* AUDIO7
    IEnumerator ApplyBoost(NEWDriver driver, float boostForce, float duration, float boostMaxSpeed)
    {
        boostEffect.Play();
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
        boostEffect.Stop();
        warpBoostEffect.SetActive(false);
    }


    /// <summary>
    /// for level 3 boost, adds a speed boost forwards and upwards
    /// </summary>
    /// <param name="driver">the player</param>
    /// <param name="boostForce">amount of force to boost character</param>
    /// <param name="duration">how long boost should last</param>
    /// <returns></returns>
    IEnumerator ApplyBoostUpward(NEWDriver driver, float boostForce, float duration, float boostMaxSpeed)
*/