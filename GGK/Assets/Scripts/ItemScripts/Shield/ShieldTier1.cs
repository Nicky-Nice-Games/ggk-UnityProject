using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ShieldTier1 : BaseItem
{
    private VFXHandler vfxScript;

    public override void OnNetworkSpawn()
    {
        currentPos.OnValueChanged += OnPositionChange;
    }

    public override void OnNetworkDespawn()
    {
        currentPos.OnValueChanged -= OnPositionChange;
    }

    // Start is called before the first frame update
    void Start()
    {

        timer = 4.0f;

        if (kart.gameObject.GetComponent<NEWDriver>() != null) // for players
        {
            // find the visual effect script from the kart
            vfxScript = kart.gameObject.GetComponent<NEWDriver>().vfxHandler;

            // play shield effect from VFXHandler script 
            vfxScript.PlayShieldVFX(timer);
        }
        else if (kart.gameObject.GetComponent<NPCPhysics>() != null) // for npcs
        {
            // find the visual effect script from the npc kart
            vfxScript = kart.gameObject.GetComponent<VFXHandler>();

            // play shield effect from VFXHandler script 
            vfxScript.PlayShieldVFX(timer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Counts down to despawn
        DecreaseTimer();

        // Sets shield position to the karts position
        if (kart)
        {
            transform.position = new Vector3(kart.transform.position.x, kart.transform.position.y, kart.transform.position.z);

            if (IsSpawned)
            {
                if (IsOwner)
                {
                    currentPos.Value = kart.transform.position;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        // destroys projectiles and hazards
        if (collision.gameObject.CompareTag("Projectile") || collision.gameObject.CompareTag("Hazard"))
        {
            if (!MultiplayerManager.Instance.IsMultiplayer)
            {
                Destroy(collision.gameObject);
            }
            else
            {
                DestroyItemRpc(collision.gameObject.GetComponent<BaseItem>());
            }
        }
    }

    public void OnPositionChange(Vector3 previousPos, Vector3 nextPos)
    {
        currentPos.Value = nextPos;
        transform.position = currentPos.Value;
    }
}
