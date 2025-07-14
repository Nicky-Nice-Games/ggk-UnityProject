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
    public string playerPassword;
    public string playerEmail;
    public PlayerKart PlayerCharacter; // should probably be an enum referencing each character
    public Color PlayerColor;
    public ulong clientID;

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
        playerName = "Default";
        playerPassword = "";
        PlayerCharacter = PlayerKart.Freddie;
        PlayerColor = Color.white;

        boostUsage = new Dictionary<string, int>();
        offenceUsage = new Dictionary<string, int>();
        trapUsage = new Dictionary<string, int>();

        isGuest = false;
    }

    // Param constructor
    public PlayerInfo(ulong clientID)
    {
        this.clientID = clientID;
        playerID = 0;
        raceStartTime = 0;
        racePosition = 0;
        mapRaced = 0;
        collisionsWithPlayers = 0;
        collisionWithWalls = 0;
        characterUsed = 0;
        fellOffMap = 0;
        playerName = "Default";
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
