using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Kart"))
        {
            if (other.gameObject.gameObject.GetComponent<NEWDriver>() != null)
            {
                if (MultiplayerManager.Instance.IsMultiplayer)
                {
                    other.GetComponent<NEWDriver>().IncrementFellOffMapRpc();
                }
                else
                {
                    other.GetComponent<NEWDriver>().playerInfo.fellOffMap++;
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
