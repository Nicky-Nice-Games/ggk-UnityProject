using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to convert the base player info class into serialized data to send over the API
/// Having this class will slow down the program when selializing the data but will overall 
/// retain runtime optimization due to the Player data in player into deing in a dictionary
/// This class is not used for gameplay logic
/// </summary>
[System.Serializable]
public class SerializablePlayerInfo : MonoBehaviour
{
    public int playerID, raceStartTime, racePosition, mapRaced, collisionsWithPlayers, collisionWithWalls, characterUsed, fellOffMap;
    public List<ItemUsageEntry> boostUsage = new List<ItemUsageEntry>();
    public List<ItemUsageEntry> offenceUsage = new List<ItemUsageEntry>();
    public List<ItemUsageEntry> trapUsage = new List<ItemUsageEntry>();

    public SerializablePlayerInfo ConvertToSerializable(PlayerInfo player)
    {
        playerID = player.playerID;
        raceStartTime = player.raceStartTime;
        racePosition = player.racePosition;
        mapRaced = player.mapRaced;
        collisionsWithPlayers = player.collisionsWithPlayers;
        collisionWithWalls = player.collisionWithWalls;
        characterUsed = player.characterUsed;

        // Serializing boost usage dict
        foreach (KeyValuePair<string, int> kvp in player.boostUsage)
        {
            boostUsage.Add(new ItemUsageEntry
            {
                itemName = kvp.Key,
                usageCount = kvp.Value
            });
        }

        // Serializing offence usage dict
        foreach (KeyValuePair<string, int> kvp in player.offenceUsage)
        {
            offenceUsage.Add(new ItemUsageEntry
            {
                itemName = kvp.Key,
                usageCount = kvp.Value
            });
        }

        // Serializing trap usage dict
        foreach (KeyValuePair<string, int> kvp in player.trapUsage)
        {
            trapUsage.Add(new ItemUsageEntry
            {
                itemName = kvp.Key,
                usageCount = kvp.Value
            });
        }
        return this;
    }
}
