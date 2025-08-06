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
public class SerializablePlayerInfo : ScriptableObject
{
    public string pid;
    public int racePosition, mapRaced, collisionsWithPlayers, collisionWithWalls, characterUsed, fellOffMap;
    public string raceStartTime;
    public float raceTime;
    public Dictionary<string, int> boostStat = new Dictionary<string, int>();
    public Dictionary<string, int> offenseStat = new Dictionary<string, int>();
    public Dictionary<string, int> trapUsage = new Dictionary<string, int>();
    public Dictionary<string, int> defenseUsage = new Dictionary<string, int>();

    public SerializablePlayerInfo ConvertToSerializable(PlayerInfo player)
    {
        Debug.Log("Called ConvertToSerializable in SerializablePlayerInfo");
        pid = player.pid;
        raceStartTime = player.raceStartTime.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");
        raceTime = player.raceTime;
        racePosition = player.racePosition;
        mapRaced = player.mapRaced;
        collisionsWithPlayers = player.collisionsWithPlayers;
        collisionWithWalls = player.collisionWithWalls;
        characterUsed = player.characterUsed;
        fellOffMap = player.fellOffMap;

        boostStat = new Dictionary<string, int>(player.boostUsage);
        offenseStat = new Dictionary<string, int>(player.offenceUsage);
        trapUsage = new Dictionary<string, int>(player.trapUsage);
        defenseUsage = new Dictionary<string, int>(player.defenseUsage);
        return this;
    }
}
