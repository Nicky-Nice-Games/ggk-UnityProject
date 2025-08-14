using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Fake Item Box
/// </summary>
public class HazardTier2 : BaseItem
{

    [SerializeField] private Material[] fakeMaterials; // The fake item brick's mesh renderer
    public int index;

    // Start is called before the first frame update
    void Start()
    {
        
        timer = 10.0f;

        // sends the hazard slightly up and behind the player before landing on the ground
        transform.position = transform.position
                             - transform.forward * 5f   // behind the kart
                             + transform.up * 1.5f;       // slightly above ground
        kart.GetComponent<NEWDriver>().playerInfo.trapUsage["brickwall"]++;
        //apply the random material to the fake item box
        index = Random.Range(0, fakeMaterials.Length);
        if (fakeMaterials.Length == 0) return;
        // Apply it to this object's MeshRenderer
        MeshRenderer myRenderer = GetComponent<MeshRenderer>();
       
            myRenderer.material = fakeMaterials[index];
        
    }

    // Update is called once per frame
    private new void Update()
    {
        RotateBox();
        DecreaseTimer();
    }

    public void RotateBox()
    {
        transform.rotation *= new Quaternion(0.0f, 2.0f * Time.deltaTime, 0.0f, 1.0f);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Kart")) // checks if kart gameobject player or npc
        {
            Rigidbody kartRigidbody;
            if (collision.gameObject.TryGetComponent<Rigidbody>(out kartRigidbody)) // checks if they have rb while also assigning if they do
            {
                kartRigidbody.velocity *= 0.125f; //this slows a kart down to an eighth of its speed

                if (collision.gameObject.transform.parent.GetChild(0).GetComponent<NEWDriver>() != null)
                {
                    NEWDriver playerKart = collision.gameObject.transform.parent.GetChild(0).GetComponent<NEWDriver>();
                    playerKart.Stun(2.0f);
                }

                if (collision.gameObject.transform.parent.GetChild(0).GetComponent<NPCPhysics>() != null)
                {
                    NPCPhysics npcKart = collision.gameObject.transform.parent.GetChild(0).GetComponent<NPCPhysics>();
                    npcKart.Stun(2.0f);
                }

                if (!MultiplayerManager.Instance.IsMultiplayer)
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    DestroyItemRpc(this);
                }
            }
        }
    }
}
