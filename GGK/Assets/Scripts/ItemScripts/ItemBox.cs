using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{


    [SerializeField]
    protected List<BaseItem> items;   // List of items the box can give
    [SerializeField] protected string itemBoxType;

    protected float respawnTimer = 5.0f;  // The seconds the box respawns after

    // [SerializeField]
    // AudioClip itemBoxSound;


    /// <summary>
    /// Read and write property for the respawn timer
    /// </summary>
    public float RespawnTimer { get { return respawnTimer; } set { respawnTimer = value; } }
    public string ItemBoxType { get { return itemBoxType; } set { itemBoxType = value; } }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Rotates the box
        RotateBox();
    }

    /// <summary>
    /// Gives the driver a random item
    /// </summary>
    public void RandomizeItem(GameObject kart)
    {
        GiveItem(kart, Random.Range(0, items.Count));
    }

    public void GiveItem(GameObject kart, int index)
    {
        ItemHolder itemScript = kart.GetComponent<ItemHolder>();

        Debug.Log("Collided!");
        BaseItem bItem = Instantiate(items[Random.Range(0, items.Count)]);
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


    private void OnDisable()
    {
        // makes sure scene is loaded to not cause error
        // if (gameObject.scene.isLoaded)
        // {
        //     AudioSource.PlayClipAtPoint(itemBoxSound, transform.position);
        // }
    }

    /// <summary>
    /// Upgrades item one tier.
    /// </summary>
    /// <param name="kart">The kart that hit it</param>
    public void UpgradeItem(GameObject kart)
    {
        //BaseItem itemUpgraded = new BaseItem();
        ItemHolder itemScript = kart.GetComponent<ItemHolder>();

        // Gives driver a random item if they don't have one
        //if (itemScript.HeldItem == null)
        //{
        //    RandomizeItem(kart);
        //}

        // Upgrades the item and returns it
        if (itemScript.HeldItem.ItemTier < 4)
        {
            itemScript.HeldItem.ItemTier++;
        }
    }

}