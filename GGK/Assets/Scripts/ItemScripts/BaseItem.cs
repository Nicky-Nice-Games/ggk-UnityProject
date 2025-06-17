using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : MonoBehaviour
{
    protected bool isUpgraded;                 // If the item is upgraded (tier 2)
    [SerializeField] protected float timer;    // Seconds until the item disappears
    [SerializeField] protected Rigidbody rb;   // The item's rigidbody
    protected ItemHolder kart;                 // The kart holding the item

    [SerializeField]
    public Texture itemIcon;

    /// <summary>
    /// Read and write property for the upgrade tier
    /// </summary>
    public bool IsUpgraded { get { return isUpgraded; } set { isUpgraded = value; } }
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