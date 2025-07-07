using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class DevShortcutsManager : MonoBehaviour
{
    [Tooltip("Should the player be allowed to toggle dev shortcuts on and off(with usage of the Backspace button)?")]
    public bool allowDevShortcuts;
    [Tooltip("If true, the script will accept shortcuts")]
    public bool devEnabled;

    [SerializeField, Tooltip("Will set itself to the first GameManager component in the scene if null on startup")]
    private GameManager manager;
    [SerializeField, Tooltip("Will set itself to the CharacterData component attatched to the GameManager object if null on startup")]
    private CharacterData charData;
    private Sprite devIcon;
    private Color devColor = Color.magenta;

    private NEWDriver trackingPlayer;
    private GameObject trackingObject;

    private DynamicRecovery trackingRecovery;
    [SerializeField] private ItemBox itemBoxReference;
    [SerializeField] private UpgradeBox upgradeBoxReference;

    [SerializeField] private GameManager managerPrefabReference;

    [SerializeField] private TMP_Text shortcutText;
    // Start is called before the first frame update
    void Start()
    {
        devIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/Player Kart Scene Images/ggk-temp-icon-gim.png");
        if (manager == null)
        {
            manager = FindAnyObjectByType<GameManager>();
        }
        if (manager != null && charData == null)
        {
            charData = manager.GetComponent<CharacterData>();
        }
        devEnabled = allowDevShortcuts;
    }

    // Update is called once per frame
    void Update()
    {
        //press backspace to turn dev mode on
        if (Input.GetKeyDown(KeyCode.Backspace) && allowDevShortcuts)
        {
            devEnabled = !devEnabled;

            if (devEnabled)
            {
                StartCoroutine(AnimateText("Developer Shortcuts: enabled"));
            }
            else
            {
                StartCoroutine(AnimateText("Developer Shortcuts: disabled"));
            }
            
        }
        if (devEnabled)
        {
            if (Input.GetKey(KeyCode.Backslash))
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    StartCoroutine(AnimateText("Input: B-Slash + S - Skip to map select"));
                    //Backslash + S: skip to map select, make character iconic PINK GIM 🗣🗣️‍🔥️️
                    if (charData)
                    {
                        charData.characterColor = devColor;
                        charData.characterSprite = devIcon;
                    }
                    manager.PlayerSelected();
                }
                if (Input.GetKeyDown(KeyCode.G))
                {
                    //Backslash + G: instantiate a GameManager object here, if there isn't one already
                    if (managerPrefabReference && manager == null)
                    {
                        StartCoroutine(AnimateText("Input: B-Slash + G - Instantiate new GameManager"));
                        manager = Instantiate(managerPrefabReference);
                        if (charData == null)
                        {
                            charData = manager.GetComponent<CharacterData>();
                        }

                        if (manager.GetComponent<DevShortcutsManager>())
                        {
                            Destroy(this.gameObject);
                        }
                    }

                }
                //INGAME
                if (Input.GetKeyDown(KeyCode.R))
                {
                    StartCoroutine(AnimateText("Input: B-Slash + R - Force Respawn"));
                    //Backslash + R: force respawn
                    if (CheckForRecovery())
                    {
                        trackingRecovery.StartRecovery();
                    }
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    StartCoroutine(AnimateText("Input: B-Slash + 1 - Give Boost"));
                    //Backslash + 1: give user a Puck
                    DevGiveItem(0);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    //Backslash + 2: give user a Shield
                    DevGiveItem(1);
                    StartCoroutine(AnimateText("Input: B-Slash + 2 - Give Shield"));
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    //Backslash + 2: give user a Boost
                    DevGiveItem(2);
                    StartCoroutine(AnimateText("Input: B-Slash + 3 - Give Puck"));
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    //Backslash + 2: give user a Hazard
                    DevGiveItem(3);
                    StartCoroutine(AnimateText("Input: B-Slash + 4 - Give Trap"));
                }
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    //Backslash + 0: upgrade held item
                    DevUpgradeItem();
                    StartCoroutine(AnimateText("Input: B-Slash + 0 - Upgrade Item"));
                }
            }
        }
    }

    //checks if the player is in a game by seeing if a trackingPlayer variable is null
    private bool CheckInGame()
    {
        if (trackingPlayer == null)
        {
            trackingPlayer = FindObjectOfType<NEWDriver>();
            if (trackingPlayer != null)
            {
                trackingObject = trackingPlayer.gameObject;
                return true;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckForRecovery()
    {
        if (CheckInGame())
        {
            Debug.LogWarning(trackingPlayer.transform.parent);
            if (trackingRecovery == null)
            {
                trackingRecovery = trackingPlayer.transform.parent.gameObject.GetComponentInChildren<DynamicRecovery>();
                //true if above code went through and trackingRecovery is now instantiated, false otherwise
                if (trackingRecovery != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            //true if trackingRecovery is already instantiated
            return true;
        }
        //false if not in-game
        else
        {
            return false;
        }
        
    }

    private void DevGiveItem(int index)
    {
        if (CheckInGame())
        {
            ItemHolder holder = trackingObject.GetComponent<ItemHolder>();
            itemBoxReference.GiveItem(trackingObject, index);
            holder.uses = holder.HeldItem.UseCount;
            holder.ApplyItemTween(holder.HeldItem.itemIcon);
        }
    }

    private void DevUpgradeItem()
    {
        if (CheckInGame())
        {
            ItemHolder holder = trackingObject.GetComponent<ItemHolder>();
            upgradeBoxReference.UpgradeItem(trackingObject);
            holder.HeldItem.OnLevelUp(holder.HeldItem.ItemTier);
            holder.uses = holder.HeldItem.UseCount;
            holder.ApplyItemTween(holder.HeldItem.itemIcon);
        }
    }


    //IMPLIMENT LATER W/DEVTOOLS CANVAS
    //SHOULDNT BREAK ANYTHING
    private IEnumerator AnimateText(string txt)
    {
        if (shortcutText != null)
        {
            shortcutText.text = txt;
            shortcutText.gameObject.SetActive(true);
            shortcutText.color = new Color(shortcutText.color.r, shortcutText.color.g, shortcutText.color.b, 1);
            float dt;
            while (shortcutText.color.a > 0)
            {
                dt = Time.deltaTime;
                shortcutText.color = new Color(shortcutText.color.r, shortcutText.color.g, shortcutText.color.b, Mathf.Max(0, shortcutText.color.a - dt));
                yield return new WaitForSeconds(dt);
            }

            shortcutText.gameObject.SetActive(false);
            shortcutText.color = new Color(shortcutText.color.r, shortcutText.color.g, shortcutText.color.b, 1);
        }

    }
}
