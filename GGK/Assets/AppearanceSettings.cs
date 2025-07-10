using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AppearanceSettings : NetworkBehaviour
{
    public Sprite icon;
    public Color color;
    public string kartName;

    public List<GameObject> models; // List of GameObjects representing different character models
    CharacterData characterData; // Reference to CharacterData script

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //UPDATE ME LATER


    }

    public void UpdateAppearance()
    {
        // If player (will need to be changed for multiplayer)
        if (GetComponent<NEWDriver>() && !MultiplayerManager.Instance.IsMultiplayer)
        {
            // Loading characterData
            characterData = FindAnyObjectByType<CharacterData>();

            // Set correct character model active
            if (characterData != null)
            {
                icon = characterData.characterSprite;
                color = characterData.characterColor;
                name = characterData.characterName.ToLower();

                for (int i = 0; i < models.Count; i++)
                {
                    //Setting active correct model
                    if (kartName == models[i].name)
                    {
                        models[i].SetActive(true);
                        continue;
                    }

                    //deleting the models that are not supposed to be active
                    Destroy(models[i]);
                }
            }
        }
        // I think NPC icons and colors are currently manually set in scene,so
        // Else NPCDriver code when NPCDrivers get a new prefab?
    }

    // OnNetworkSpawn is called when the GameObject is synced to all clients
    public override void OnNetworkSpawn()
    {
        if (!GetComponent<NEWDriver>()) return;
        if (!IsServer) return;
        string characterName = MultiplayerManager.Instance.players[OwnerClientId].CharacterName;
        Color characterColor = MultiplayerManager.Instance.players[OwnerClientId].CharacterColor;
        SetKartAppearanceRpc(characterName, characterColor);
        
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    public void SetKartAppearanceRpc(/*CharacterData characterData, */ string characterName, Color characterColor) // should use a struct or a simpler keyword that fully represents the look of the kart
    {
        // Loading characterData
        characterData = FindAnyObjectByType<CharacterData>();
        icon = characterData.characterSprite;
        color = characterColor;
        kartName = characterName.ToLower();
        Debug.Log($"Setting {OwnerClientId}'s kart color to {color} and name/model to {kartName}");

        // Set correct character model active
        if (characterData != null)
        {
            for (int i = 0; i < models.Count; i++)
            {
                //Setting active correct model
                if (kartName == models[i].name)
                {
                    models[i].SetActive(true);
                    break;
                }

                //deleting the models that are not supposed to be active
                Destroy(models[i]);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        //UPDATE ME LATER

        
    }
}
