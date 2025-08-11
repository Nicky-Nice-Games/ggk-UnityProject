using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeathZone : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Kart"))
        {
            if (other.transform.parent.GetChild(0).TryGetComponent<NEWDriver>(out NEWDriver kart))
            {
                if (MultiplayerManager.Instance.IsMultiplayer)
                {
                    if (kart.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                    {
                        kart.IncrementFellOffMapRpc();
                    }
                }
                else
                {
                    kart.playerInfo.fellOffMap++;
                }
            }

            DynamicRecovery recovery = other.GetComponent<DynamicRecovery>();
            if (recovery != null)
            {
                //Debug.Log("Got here!!!!!!!!!!!!!!!!");
                recovery.StartRecovery();

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
