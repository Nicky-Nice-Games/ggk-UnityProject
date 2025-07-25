using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;

public class ItemBox : NetworkBehaviour
{


    [SerializeField]
    protected List<BaseItem> items;   // List of items the box can give

    protected float respawnTimer = 5.0f;  // The seconds the box respawns after
    public float RespawnTimer { get { return respawnTimer; } set { respawnTimer = value; } }

    const float defaultDuration = 5.0f;

    private bool active = true;
    public bool Active
    {
        get
        {
            return active;
        }
        set
        {
            active = value;
            if (value) ShowAndEnable();
            else
            {
                HideAndDisable();
                StartTimer();
            }
        }
    }
    public NetworkVariable<bool> currentActiveState = new NetworkVariable<bool>();

    [SerializeField] protected ItemHolder.ItemTypeEnum itemBoxType;
    public ItemHolder.ItemTypeEnum ItemBoxType { get { return itemBoxType; } set { itemBoxType = value; } }

    public override void OnNetworkSpawn()
    {
        currentActiveState.OnValueChanged += OnCurrentActiveStateChanged;
    }

    public override void OnNetworkDespawn()
    {
        currentActiveState.OnValueChanged -= OnCurrentActiveStateChanged;
    }

    private void OnCurrentActiveStateChanged(bool previousValue, bool newValue)
    {
        Active = newValue;
    }

    private void HideAndDisable()
    {
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void ShowAndEnable()
    {
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
    }

    private void Update()
    {
        if(IsSpawned && !IsServer){
            return;
        } else if (IsSpawned && IsServer) {
            HandleBoxTimer();
        } else {
            HandleBoxTimer();
        }
        // Rotates the box
        if (Active) RotateBox();
    }

    /// <summary>
    /// Gives the driver a random item
    /// </summary>
    // public void RandomizeItem(GameObject kart)
    // {
    //     GiveItem(kart, Random.Range(0, items.Count));
    // }

    public void GiveItem(GameObject kart, int index)
    {
        ItemHolder itemScript = kart.GetComponent<ItemHolder>();

        Debug.Log("Collided!");
        BaseItem bItem = Instantiate(items[index]);
        itemScript.HeldItem = bItem;
        itemScript.HeldItem.ItemTier = 1;
        bItem.gameObject.SetActive(false);
    }

    /// <summary>
    /// Spins the item box around the y-axis
    /// </summary>
    public void RotateBox()
    {
        transform.rotation *= new Quaternion(0.0f, 2.0f * Time.deltaTime, 0.0f, 1.0f);
    }

    /// <summary>
    /// Upgrades item one tier.
    /// </summary>
    /// <param name="kart">The kart that hit it</param>
    // public void UpgradeItem(GameObject kart)
    // {
    //     //BaseItem itemUpgraded = new BaseItem();
    //     ItemHolder itemScript = kart.GetComponent<ItemHolder>();

    //     // Gives driver a random item if they don't have one
    //     //if (itemScript.HeldItem == null)
    //     //{
    //     //    RandomizeItem(kart);
    //     //}

    //     // Upgrades the item and returns it
    //     if (itemScript.HeldItem.ItemTier < 4)
    //     {
    //         itemScript.HeldItem.ItemTier++;
    //     }
    // }

    public void StartTimer(float duration = defaultDuration)
    {
        respawnTimer = duration;
    }

    private void HandleBoxTimer()
    {
        if (respawnTimer > 0)
        {
            respawnTimer -= Time.deltaTime;
        }
        else
        {
            if(IsSpawned && !IsServer){
                return;
            } else if (IsSpawned && IsServer) {
                currentActiveState.Value = true;
            } else {
                Active = true;
            }
        }
    }
    private bool IsTimerFinished()
    {
        return respawnTimer < 0;
    }
}