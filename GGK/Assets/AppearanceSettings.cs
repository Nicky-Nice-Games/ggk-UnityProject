using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppearanceSettings : MonoBehaviour
{
    public Sprite icon;
    public Color color;
    public string name;

    public List<GameObject> models; // List of GameObjects representing different character models
    CharacterData characterData; // Reference to CharacterData script

    // Start is called before the first frame update
    void Start()
    {
        // If player (will need to be changed for multiplayer)
        if (GetComponent<NEWDriver>())
        {
            // Loading characterData
            characterData = FindAnyObjectByType<CharacterData>();
            icon = characterData.characterSprite;
            color = characterData.characterColor;
            name = characterData.characterName.ToLower();

            // Set correct character model active
            if (characterData != null)
            {
                for (int i = 0; i < models.Count; i++)
                {
                    //Setting active correct model
                    if (name == models[i].name)
                    {
                        models[i].SetActive(true);
                        break;
                    }

                    //deleting the models that are not supposed to be active
                    Destroy(models[i]);
                }
            }
        }
        // I think NPC icons and colors are currently manually set in scene,so
        // Else NPCDriver code when NPCDrivers get a new prefab?
    }

    // Update is called once per frame
    void Update()
    {
        //UPDATE ME LATER

        
    }
}
