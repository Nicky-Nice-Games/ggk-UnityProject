using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
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

    // OnNetworkSpawn is called when the GameObject is synced to all clients
    public override void OnNetworkSpawn()
    {
        if (!GetComponent<NEWDriver>()) return;
        if (!IsServer) return;
        string characterName = MultiplayerManager.Instance.players[OwnerClientId].CharacterName;
        Color characterColor = MultiplayerManager.Instance.players[OwnerClientId].CharacterColor;
        SetKartAppearanceRpc(characterName, characterColor);

    }

    public void UpdateAppearance()
    {
        //// If player (will need to be changed for multiplayer)
        if (GetComponent<NEWDriver>() && !IsSpawned)
        {
            // Loading characterData
            characterData = FindAnyObjectByType<CharacterData>();

            // Set correct character model active
            if (characterData != null)
            {
                icon = characterData.characterSprite;
                color = characterData.characterColor;
                name = characterData.characterName.ToLower();

                UpdateModel();
            }
            else
            {
                // If no characterData found, set the last model in the list as active
                models[models.Count - 1].SetActive(true);
            }
        }
        // I think NPC icons and colors are currently manually set in scene,so
        // Else NPCDriver code when NPCDrivers get a new prefab?
    }

    public void UpdateModel()
    {
        // Set correct character model active
        if (models != null)
        {
            for (int i = 0; i < models.Count; i++)
            {
                //Setting active correct model
                if (name == models[i].name || CharacterBuilder.ModelToName(name) == models[i].name)
                {
                    Animator nAnim = models[i].GetComponent<Animator>();
                    if (nAnim && GetComponent<NEWDriver>())
                    {
                        //NetworkObject nO1 = models[i].GetComponent<NetworkObject>();
                        //NetworkObject nO2 = transform.root.GetComponent<NetworkObject>();
                        //if (IsSpawned && IsServer)
                        //{
                            //nAnim.OnNetworkSpawn();
                        //}
                        
                    }
                    models[i].SetActive(true);
                    break;
                }
                //deleting the models that are not supposed to be active

                //bugs out in multiplayer :(
                //Destroy(models[i])
            }
        }
        else
        {
            // If no characterData found, set the last model in the list as active
            models[models.Count - 1].SetActive(true);
        }
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    public void SetKartAppearanceRpc(/*CharacterData characterData, */ string characterName, Color characterColor) // should use a struct or a simpler keyword that fully represents the look of the kart
    {
        print(characterName);
        color = characterColor;
        kartName = characterName.ToLower();
        icon = CharacterBuilder.NameToSprite(kartName);
        name = kartName;
        Debug.Log($"Setting {OwnerClientId}'s kart color to {color}, sprite to {icon.name} and name/model to {kartName}");
        UpdateModel();
        MiniMapHud.instance.UpdateIconAppearance(gameObject, this);
    }
}
