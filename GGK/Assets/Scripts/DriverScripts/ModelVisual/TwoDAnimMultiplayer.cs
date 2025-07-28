using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TwoDAnimMultiplayer : NetworkBehaviour
{
    public TwoDimensionalAnimController controller;

    //UNUSED ATM
    public NEWDriver driver;

    public float lastSent = 0;

    //the minimum absolute value the client will accept, values lower than this will not be sent over.
    //this is so infinitely small values aren't sent over to the clients
    private float minSentValue = 0.001f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!controller) return;

        if (IsSpawned && IsOwner)
        {
            if (controller.turningValue != lastSent && Mathf.Abs(controller.turningValue) > minSentValue)
            {
                SendTurnDataRpc(controller.turningValue);
                lastSent = controller.turningValue;
            }
        }


    }

    [Rpc(SendTo.NotMe, RequireOwnership = false)]
    public void SendTurnDataRpc(float data)
    {
        controller.turningValue = data;
    }
}
