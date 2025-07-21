using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTier3 : BaseItem
{
    // Start is called before the first frame update
    void Start()
    {
        timer = 8.0f;
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
        }
    }
}
