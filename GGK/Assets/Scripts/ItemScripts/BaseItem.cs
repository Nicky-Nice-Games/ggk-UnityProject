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
            Destroy(this.gameObject);
        }
    }

}