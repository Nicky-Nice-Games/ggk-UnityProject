using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;

public class BaseItem : NetworkBehaviour
{
    [SerializeField] protected float timer;    // Seconds until the item disappears
    [SerializeField] protected Rigidbody rb;   // The item's rigidbody
    protected ItemHolder kart;                 // The kart holding the item
    [SerializeField] protected string itemCategory;

    [SerializeField] protected int useCount;
    [SerializeField] public bool isTimed;
    protected int itemTier;

    [SerializeField]
    public Texture itemIcon;
    // item icons for each tier
    [SerializeField]
    public Texture tierOneItemIcon;
    [SerializeField]
    public Texture tierTwoItemIcon;
    [SerializeField]
    public Texture tierThreeItemIcon;
    [SerializeField]
    public Texture tierFourItemIcon;

    public VisualEffect shieldEffect;

    /// <summary>
    /// Read and write property for the upgrade tier
    /// </summary>
    public int ItemTier { get { return itemTier; } set { itemTier = value; } }
    /// <summary>
    /// Read and write property for the upgrade tier
    /// </summary>
    public int UseCount { get { return useCount; } set { useCount = value; } }
    /// <summary>
    /// Read and write property for the upgrade tier
    /// </summary>
    public float Timer { get { return timer; } set { timer = value; } }

    public string ItemCategory { get { return itemCategory; } set { itemCategory = value; } }

    public NetworkVariable<Vector3> currentPos = new NetworkVariable<Vector3>();


    /// <summary>
    /// Read and write property for the kart holding the item
    /// </summary>
    public ItemHolder Kart { get { return kart; } set { kart = value; } }

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    public void Update()
    {

        
    }

    /// <summary>
    /// Counts down 1 second until the item disappears
    /// </summary>
    public void DecreaseTimer()
    {
        // Subtracts the timer by 1 second
        if (timer >= 0)
        {
            timer -= 1.0f * Time.deltaTime;
        }
        // Destroys the item
        else
        {
            //if (itemCategory != "Shield")
            //{
                Destroy(this.gameObject);
            //}
        }
    }

    /// <summary>
    /// changes corresponding item functionality when upgraded
    /// </summary>
    /// <param name="level">current level of item</param>
    public void OnLevelUp(int level)
    {
        // update the icon based on the item tier
        switch (itemTier)
        {
            case 2:
                itemIcon = tierTwoItemIcon;
                break;
            case 3:
                itemIcon = tierThreeItemIcon;
                break;
            case 4:
                itemIcon = tierFourItemIcon;
                break;
            default:
                itemIcon = tierOneItemIcon;
                break;
        }
        if (itemCategory == "Puck")
        {
            // Tracks the item tier
            switch (itemTier)
            {
                // Multi-puck (3 uses)
                case 2:
                    useCount = 1;
                    timer = 50;
                    break;
                // Puck tracks to the closest player and lasts longer
                case 3:
                    useCount = 3;
                    timer = 50;
                    break;
                // Puck tracks to first place
                case 4:
                    useCount = 1;
                    timer = 50;
                    break;
                // Normal puck, one use
                default:
                    useCount = 1;
                    break;
            }
        }
        if (itemCategory == "Boost") // changes the use count for different boost levels
        {
            Debug.Log("Boost Upgraded.");
            switch (itemTier)
            {
                // use count should always be 1 change boost force
                case 2:
                    useCount = 1;
                    break;
                case 3:
                    useCount = 1;
                    break;
                case 4:
                    useCount = 1;
                    break;
                default:
                    useCount = 1;
                    break;
            }
            Boost boost = (Boost)this;
        }
        else if (itemCategory == "Shield") // changes timer on different shield levels
        {
            Debug.Log("Shield Upgraded.");
            switch (itemTier)
            {
                case 2:
                    timer = 6.0f;
                    break;
                case 3:
                    timer = 8.0f;
                    break;
                case 4:
                    timer = 10.0f;
                    break;
                default:
                    timer = 4.0f;
                    break;
            }
            Shield shield = (Shield)this;
            useCount = 1;
        }
        else if (itemCategory == "Hazard")
        {
            TrapItem temp = (TrapItem)this;
            switch (itemTier)
            {
                case 2:
                    temp.UpdateComponents(temp.tierTwoBody, tierTwoItemIcon);
                    break;
                case 3:
                    temp.UpdateComponents(temp.tierThreeBody, tierThreeItemIcon);
                    break;
                case 4:
                    temp.UpdateComponents(temp.tierFourBody, tierFourItemIcon);
                    break;
                default:
                    temp.UpdateComponents(temp.tierOneBody, tierOneItemIcon);
                    break;
            }
        }
    }

    //public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    //{
    //    serializer.SerializeValue(ref timer);
    //    serializer.SerializeValue(ref useCount);
    //    serializer.SerializeValue(ref itemTier);
    //}
}