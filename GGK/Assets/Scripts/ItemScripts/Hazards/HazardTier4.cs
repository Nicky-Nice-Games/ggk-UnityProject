using UnityEngine;

/// <summary>
/// Blizzard
/// </summary>
public class HazardTier4 : BaseItem
{
    uint hitSpinoutID = 0;
    GameObject hazardVFX;

    private void Start()
    {
        Vector3 behindPos = transform.position - transform.forward * 11;
        behindPos.y += 0.8f;
        transform.position = behindPos;

        // Checking for multiplayer
        if (IsSpawned)
        {
            kart.gameObject.GetComponent<NEWDriver>().IncrementHazardUsageTier4Rpc();
        }
        else
        {
            kart.gameObject.GetComponent<NEWDriver>().playerInfo.trapUsage["confuseritchie"]++;
        }

        // get a reference to only the vfx
        hazardVFX = GameObject.Find("VFX");
    }

    private new void Update()
    {
        RotateBox();
        DecreaseTimer();
    }

    private void RotateBox()
    {
        // only rotate the vfx portion of the hazard and not the cloud
        hazardVFX.transform.rotation *= new Quaternion(0.0f, 1.5f * Time.deltaTime, 0.0f, 1.0f);
    }

    private void OnTriggerEnter(Collider collision)
    {
        // stop the trap from falling when they reach the ground/road
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Road"))
        {
            rb.constraints = RigidbodyConstraints.FreezePositionY;
        }

        // slows the kart a little when it goes into the hazard
        if (collision.gameObject.CompareTag("Kart")) // checks if kart gameobject player or npc
        {
            Rigidbody kartRigidbody;
            if (collision.gameObject.TryGetComponent<Rigidbody>(out kartRigidbody)) // checks if they have rb while also assigning if they do
            {
                kartRigidbody.velocity *= 0.75f; //this slows a kart down to an eighth of its speed
            }
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        // slows the kart a little when it goes into the hazard
        if (collision.gameObject.CompareTag("Kart")) // checks if kart gameobject player or npc
        {
            Rigidbody kartRigidbody;
            if (collision.gameObject.TryGetComponent<Rigidbody>(out kartRigidbody)) // checks if they have rb while also assigning if they do
            {
                kartRigidbody.velocity *= 0.9f; //this slows a kart down to an eighth of its speed

                hitSpinoutID = AkUnitySoundEngine.PostEvent("Play_hit_spinout", gameObject);
            }
        }
    }

    // called when the kart leaves the hazard
    private void OnTriggerExit(Collider collision)
    {
        // reverses the karts controls after leaving the hazard
        if (collision.gameObject.CompareTag("Kart")) // checks if kart gameobject player or npc
        {
            NEWDriver playerKart = collision.gameObject.gameObject.GetComponentInChildren<NEWDriver>();
            if (playerKart)
            {
                if (!playerKart.isConfused)
                {
                    playerKart.dazedStars.Play();
                    playerKart.confusedTimer = 10;
                    playerKart.isConfused = true;
                    playerKart.movementDirection *= -1;
                }
            }
        }
    }
}
