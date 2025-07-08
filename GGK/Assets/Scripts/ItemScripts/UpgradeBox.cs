using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeBox : ItemBox
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Rotates the itembox
        RotateBox();
    }

    /// <summary>
    /// Upgrades item to its second tier.
    /// Gives driver a tier 2 item if they don't have an item.
    /// </summary>
    /// <param name="kart">The kart that hit it</param>
    /// <returns>An upgraded baseItem</returns>
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