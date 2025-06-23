using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{


    [SerializeField]
    protected List<BaseItem> items;   // List of items the box can give

    protected float respawnTimer = 5.0f;  // The seconds the box respawns after

    [SerializeField]
    AudioClip itemBoxSound;


    /// <summary>
    /// Read and write property for the respawn timer
    /// </summary>
    public float RespawnTimer { get { return respawnTimer; } set { respawnTimer = value; } }

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
    /// <returns>A random baseItem</returns>
    public BaseItem RandomizeItem()
    {
        Debug.Log("Collided!");
        BaseItem bItem = items[1];
        bItem.ItemTier = 1;
        return bItem;
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
        if (gameObject.scene.isLoaded)
        {
            AudioSource.PlayClipAtPoint(itemBoxSound, transform.position);
        }
    }

}