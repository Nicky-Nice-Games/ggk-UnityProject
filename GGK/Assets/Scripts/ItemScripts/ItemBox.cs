using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemBox : NetworkBehaviour
{
    // Change from ItemBox Script: Save reference to other 2 components
    // So they can be hidden and unhidden
    [SerializeField]
    private MeshRenderer boxColor;
    [SerializeField]
    private GameObject boxIcon; // The Icon does not have a mesh renderer, made it a game object

    private void Start()
    {
        if (!IsSpawned && randomizeOnStart)
        {
            SetItemBoxType(ItemHolder.RandomItemType());
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer && randomizeOnStart)
        {
            SetItemBoxTypeRpc(ItemHolder.RandomItemType());
        }
    }

    private void Update()
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
    [Header("Type")]
    [SerializeField] private ItemHolder.ItemTypeEnum itemType = ItemHolder.ItemTypeEnum.NoItem;
    public ItemHolder.ItemTypeEnum ItemType { get { return itemType; } private set { itemType = value; } }
    [Header("Respawning")]
    [SerializeField] private bool isActive = true;
    [SerializeField] private bool isTimerActive = false;
    [SerializeField] private float timerDuration = 0.0f;
    const float defaultTimerDuration = 2.5f;
    [Header("Randomization")]
    [SerializeField] private bool randomRespawnType = false;
    [SerializeField] private bool randomizeOnStart = false;
    [SerializeField] private List<Material> ItemBrickMaterials;

    private void ShowAndEnable()
    {
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<BoxCollider>().enabled = true;
        // Level Design Addition: enable Color and Icon MeshRenderers
        boxColor.enabled = true;
        boxIcon.SetActive(true);
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
        // Level Design Addition: disable Color and Icon MeshRenderers
        boxColor.enabled = false;
        boxIcon.SetActive(false);
        isActive = false;
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    private void HideAndDisableRpc()
    {
        HideAndDisable();
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
                if (randomRespawnType)
                {
                    SetItemBoxTypeRpc(ItemHolder.RandomItemType());
                }
                StopTimerRpc();
                ShowAndEnableRpc();
            }
            else
            {
                if (randomRespawnType)
                {
                    SetItemBoxType(ItemHolder.RandomItemType());
                }
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

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    private void SetItemBoxTypeRpc(ItemHolder.ItemTypeEnum typeEnum)
    {
        SetItemBoxType(typeEnum);
    }

    private void SetItemBoxType(ItemHolder.ItemTypeEnum typeEnum)
    {
        ItemType = typeEnum;
        GetComponent<MeshRenderer>().SetMaterials(new List<Material> { ItemBrickMaterials[(int)ItemType + 1] });
    }
}