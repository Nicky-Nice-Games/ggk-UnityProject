using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : MonoBehaviour
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
            boost.LevelUp();
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
            shield.LevelUp();
            useCount = 1;
        }
        else if (itemCategory == "Hazard")
        {
            TrapItem temp = (TrapItem)this;
            temp.LevelUp();
        }
    }

}