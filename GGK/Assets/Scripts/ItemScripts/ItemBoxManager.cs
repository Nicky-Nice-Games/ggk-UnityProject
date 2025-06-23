using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoxManager : MonoBehaviour
{

    [SerializeField]
    private List<ItemBox> itemBoxes;
    [SerializeField] 
    private GameObject itemBoxParent;


    // Start is called before the first frame update
    void Start()
    {
        // Automatically find all ItemBox components in the children of the parent
        itemBoxes.AddRange(itemBoxParent.GetComponentsInChildren<ItemBox>(true));
    }

    // Update is called once per frame
    void Update()
    {
        foreach (ItemBox item in itemBoxes)
        {
            if (item.gameObject.activeSelf == false)
            {
                // THIS SHOULD BE A METHOD IN THE ITEM BOX
                item.RespawnTimer -= 1.0f * Time.deltaTime;
                if (item.RespawnTimer < 0.0f)
                {
                    item.gameObject.SetActive(true);
                    item.RespawnTimer = 5.0f;
                }
            }
        }
    }
}
