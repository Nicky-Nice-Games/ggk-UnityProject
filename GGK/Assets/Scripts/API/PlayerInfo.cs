using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// This Class is ised as the base container for the player info
/// It is used to retain basic functionality for C# Dictionaries
/// </summary>
[System.Serializable]
public class PlayerInfo : MonoBehaviour
{
    // Basic player and match stats
    [Header("Do not Change")]
    public int playerID, raceStartTime, racePosition, mapRaced, collisionsWithPlayers, collisionWithWalls, characterUsed, fellOffMap;
    public Dictionary<string, int> boostUsage;
    public Dictionary<string, int> offenceUsage;
    public Dictionary<string, int> trapUsage;

    // NOT SENT TO BACKEND / Used unity side
    public bool isGuest;
    public string playerName;
    public PlayerKart PlayerCharacter; // should probably be an enum referencing each character
    public Color PlayerColor;
    public int clientID;

    //Default constructor
    public PlayerInfo() 
    {
        playerID = 0;
        raceStartTime = 0;
        racePosition = 0;
        mapRaced = 0;
        collisionsWithPlayers = 0;
        collisionWithWalls = 0;
        characterUsed = 0;
        fellOffMap = 0;

        boostUsage = new Dictionary<string, int>();
        offenceUsage = new Dictionary<string, int>();
        trapUsage = new Dictionary<string, int>();

        isGuest = false;
    }

    // Param constructor
    public PlayerInfo(int pid = 0, int startTime = 0, int racePos = 0, int map = 0, int collisionWPlayers = 0, 
        int collisionWWals = 0, int character = 0, int offMap = 0, string name = null, int clientID = -1)
    {
        this.clientID = clientID;
        playerID = pid;
        raceStartTime = startTime;
        racePosition = racePos;
        mapRaced = map;
        collisionsWithPlayers = collisionWPlayers;
        collisionWithWalls = collisionWWals;
        characterUsed = character;
        fellOffMap = offMap;
        playerName = name;
        PlayerCharacter = PlayerKart.Freddie;
        PlayerColor = Color.white;

        boostUsage = new Dictionary<string, int>();
        offenceUsage = new Dictionary<string, int>();
        trapUsage = new Dictionary<string, int>();

        isGuest = false;

    }

    // Copy constructor
    public PlayerInfo(PlayerInfo other)
    {
        playerID = other.playerID;
        raceStartTime = other.raceStartTime;
        racePosition = other.racePosition;
        mapRaced = other.mapRaced;
        collisionsWithPlayers = other.collisionsWithPlayers;
        collisionWithWalls = other.collisionWithWalls;
        characterUsed = other.characterUsed;
        fellOffMap = other.fellOffMap;

        boostUsage = new Dictionary<string, int>(other.boostUsage);
        offenceUsage = new Dictionary<string, int>(other.offenceUsage);
        trapUsage = new Dictionary<string, int>(other.trapUsage);
    }
}
