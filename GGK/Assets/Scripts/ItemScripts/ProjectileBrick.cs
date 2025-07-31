using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBrick : ItemBox
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Rotates Box
        RotateBox(); 
    }

    public void GiveProjectile(GameObject kart)
    {
        ItemHolder itemScript = kart.GetComponent<ItemHolder>();

        Debug.Log("Collided!");
        // BaseItem bItem = Instantiate(items[0]);
        // itemScript.HeldItem = bItem;
        // itemScript.HeldItem.ItemTier = 1;
        // bItem.gameObject.SetActive(false);
    }
}
