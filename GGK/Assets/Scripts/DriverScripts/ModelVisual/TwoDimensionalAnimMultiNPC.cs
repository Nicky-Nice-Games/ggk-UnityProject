using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TwoDimensionalAnimMultiNPC : NetworkBehaviour
{
     public TwoDimensionalAnimControllerNPC controller;

    public float lastSent = 0;

    private float lastRecieved;

    //the minimum absolute value the client will accept, values lower than this will not be sent over.
    //this is so infinitely small values aren't sent over to the clients
    private float minSentValue = 0.001f;

    //how often the server can send to the other clients (1/10 second, just so stuff isnt getting sent every frame)
    private float sendBuffer = 0.1f;

    private float currentBuffer;
    // Start is called before the first frame update
    void Start()
    {
        if (IsSpawned && IsOwner) return;
        currentBuffer = sendBuffer;
    }

    // Update is called once per frame
    void Update()
    {
        if (!controller) return;
        //if in multiplayer...
        if (IsSpawned)
        {
            //if the owner, send turn data to other clients
            if (IsOwner)
            {
                currentBuffer -= Time.deltaTime;
                if (currentBuffer <= 0)
                {
                    if (controller.turningValue != lastSent && Mathf.Abs(controller.turningValue) > minSentValue)
                    {
                        SendTurnDataRpc(controller.turningValue);
                        lastSent = controller.turningValue;
                    }
                    currentBuffer = sendBuffer;
                }
            }
            //if not the owner, smooth lerp recieved turn data
            else
            {
                controller.turningValue = Mathf.Lerp(controller.turningValue, lastRecieved, Time.deltaTime * controller.lerpSpeed);
            }
           
        }


    }

    [Rpc(SendTo.NotMe, RequireOwnership = false)]
    public void SendTurnDataRpc(float data)
    {
        lastRecieved = data;
    }
}
