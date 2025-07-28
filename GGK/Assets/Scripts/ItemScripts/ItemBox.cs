using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemBox : NetworkBehaviour
{
    //[SerializeField]
    //protected List<BaseItem> items;   // List of items the box can give
    //[SerializeField] protected string itemBoxType;
    //protected float respawnTimer = 5.0f;  // The seconds the box respawns after



    /// <summary>
    /// Read and write property for the respawn timer
    /// </summary>
    //public float RespawnTimer { get { return respawnTimer; } set { respawnTimer = value; } }
    //public string ItemBoxType { get { return itemBoxType; } set { itemBoxType = value; } }


    // Update is called once per frame
    void Update()
    {
        if (IsSpawned && !IsServer)
        {
            if (!isTimerActive)
            {
                RotateBox();
            }
        }
        else if (IsSpawned && IsServer)
        {
            if (isTimerActive)
            {
                HandleTimer(Time.deltaTime);
            }
            else
            {
                RotateBox();
            }
        }
        else
        {
            if (isTimerActive)
            {
                HandleTimer(Time.deltaTime);
            }
            else
            {
                RotateBox();
            }
        }
    }

    public void GiveItem(GameObject kart, int index)
    {
        ItemHolder itemScript = kart.GetComponent<ItemHolder>();

        Debug.Log("Collided!");
        // BaseItem bItem = Instantiate(items[index]);
        // itemScript.HeldItem = bItem;
        // itemScript.HeldItem.ItemTier = 1;
        // bItem.gameObject.SetActive(false);
    }

    /// <summary>
    /// Spins the item box around the y-axis
    /// </summary>
    public void RotateBox()
    {
        transform.rotation *= new Quaternion(0.0f, 2.0f * Time.deltaTime, 0.0f, 1.0f);
    }


    // new code
    [SerializeField] private ItemHolder.ItemTypeEnum itemType = ItemHolder.ItemTypeEnum.NoItem;
    public ItemHolder.ItemTypeEnum ItemType { get { return itemType; } private set { itemType = value; } }
    [SerializeField] private bool isActive = true;
    [SerializeField] private bool isTimerActive = false;
    [SerializeField] private float timerDuration = 0.0f;
    const float defaultTimerDuration = 5.0f;

    private void ShowAndEnable()
    {
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<BoxCollider>().enabled = true;
        isActive = true;
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    private void ShowAndEnableRpc()
    {
        ShowAndEnable();
    }

    private void HideAndDisable()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        isActive = false;
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    private void HideAndDisableRpc()
    {
        HideAndDisableRpc();
    }

    public void StartTimer(float duration = defaultTimerDuration)
    {
        timerDuration = duration;
        isTimerActive = true;
        HideAndDisable();
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    public void StartTimerRpc(float duration = defaultTimerDuration)
    {
        isTimerActive = true;
        if (IsServer)
        {
            timerDuration = duration;
            HideAndDisableRpc();
        }
    }

    private void HandleTimer(float delta)
    {
        timerDuration -= delta;
        if (timerDuration < 0.0f)
        {
            if (IsSpawned)
            {
                StopTimerRpc();
                ShowAndEnableRpc();
            }
            else
            {
                isTimerActive = false;
                ShowAndEnable();
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    private void StopTimerRpc()
    {
        isTimerActive = false;
    }
}