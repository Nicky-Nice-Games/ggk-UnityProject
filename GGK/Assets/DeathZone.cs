using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Kart"))
        {
            if (other.transform.parent.GetChild(0).GetComponent<NEWDriver>() != null)
            {
                if (MultiplayerManager.Instance.IsMultiplayer)
                {
                    other.transform.parent.GetChild(0).GetComponent<NEWDriver>().IncrementFellOffMapRpc();
                }
                else
                {
                    other.transform.parent.GetChild(0).GetComponent<NEWDriver>().playerInfo.fellOffMap++;
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
