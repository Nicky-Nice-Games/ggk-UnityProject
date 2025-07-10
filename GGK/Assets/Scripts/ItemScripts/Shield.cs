using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shield : BaseItem
{
    // save the default color of the shield
    private Renderer renderer;
    private Material material;
    private Color defaultColor;
    // the color the shield flashes the last few seconds of it's duration
    private Color timerColor;

    // item icons for each tier
    [SerializeField]
    public Texture tierOneItemIcon;
    [SerializeField]
    public Texture tierTwoItemIcon;
    [SerializeField]
    public Texture tierThreeItemIcon;
    [SerializeField]
    public Texture tierFourItemIcon;

    // the interval between color switches on the shield
    private float blinkInterval = 1.0f;

    // the time when the shield should indicate it's ending
    private float indicatorTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        // save the default color of the shield
        renderer = GetComponent<Renderer>();
        material = renderer.material;
        defaultColor = material.color;

        timerColor = Color.red;
        timerColor.a = defaultColor.a;
    }

    // Update is called once per frame
    void Update()
    {
        // Counts down to despawn
        DecreaseTimer();

        if (timer <= indicatorTime)
        {
            // flash a different color to indicate the shield is ending
            StartCoroutine(ColorBlink());
        }

        // Sets shield position to the karts position
        if (kart)
        {
            transform.position = new Vector3(kart.transform.position.x, kart.transform.position.y, kart.transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        // Destroys any game object or hazard the shield hits
        if (collision.gameObject.CompareTag("Projectile") || collision.gameObject.CompareTag("Hazard"))
        {
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Kart") && itemTier == 4)
        {
            // ensures that the shield is hitting a player or NPC kart
            if (collision.gameObject.GetComponent<NEWDriver>() != null)
                //&& collision.gameObject.GetComponent<NEWDriver>() != Kart)
            {
                NEWDriver playerKart = collision.gameObject.GetComponent<NEWDriver>();
                playerKart.Stun(2.0f);
            }

            if(collision.gameObject.GetComponent<NPCDriver>() != null)
                //&& collision.gameObject.GetComponent<NPCDriver>() != Kart)
            {
                NPCDriver npcKart = collision.gameObject.GetComponent<NPCDriver>();
                npcKart.StartRecovery(2.0f);
            }
        }
    }

    // Should be moved to BaseItem to be overridable
    // Added separately for now so there aren't big conflicts with the Puck when that's pushed
    public void LevelUp()
    {
        // update the icon based on the item tier
        switch (itemTier)
        {
            case 2:
                itemIcon = tierTwoItemIcon;
                break;
            case 3:
                itemIcon = tierThreeItemIcon;
                break;
            case 4:
                itemIcon = tierFourItemIcon;
                break;
            default:
                itemIcon = tierOneItemIcon;
                break;
        }
    }

    IEnumerator ColorBlink()
    {
        float elapsed = 0f;
        bool toggle = false;

        while (elapsed < indicatorTime)
        {

            material.color = toggle ? defaultColor : timerColor;
            toggle = !toggle;

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        material.color = defaultColor;
    }
}
