using UnityEngine;

/// <summary>
/// Fake Item Box
/// </summary>
public class HazardTier2 : BaseItem
{
    [SerializeField] private MeshRenderer meshRenderer; // The fake item brick's mesh renderer

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        timer = 10.0f;

        // sends the hazard slightly up and behind the player before landing on the ground
        transform.position = transform.position
                             - transform.forward * 5f   // behind the kart
                             + transform.up * 1.5f;       // slightly above ground

        // Checking for multiplayer
        if (IsSpawned)
        {
            kart.gameObject.GetComponent<NEWDriver>().IncrementHazardUsageTier2Rpc();
        }
        else
        {
            kart.gameObject.GetComponent<NEWDriver>().playerInfo.trapUsage["fakepowerupbrick"]++;
        }
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
