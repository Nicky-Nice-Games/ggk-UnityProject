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
    uint hitSpinoutID = 0;

    // Start is called before the first frame update
    void Start()
    {
        timer = 10.0f;

        // sends the hazard slightly up and behind the player before landing on the ground
        transform.position = transform.position
                             - transform.forward * 5f   // behind the kart
                             + transform.up * 1.5f;       // slightly above ground
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

                hitSpinoutID = AkUnitySoundEngine.PostEvent("Play_hit_spinout", gameObject);

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
