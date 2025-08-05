using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
/* Log to keep track of where all the dynamic values sent to backend are being assigned:
 * playerID - From backend when PlayerInfo is created in APIManager
 * racePosition - KartCheckpoint::OnTriggerEnter
 * mapRaced - 
 * collisionsWithPlayers - KartCheckpoint::OnCollisionEnter
 * collisionWithWalls - BonkCollision::OnCollisionEnter
 * characterUsed - 
 * fellOffMap - DeathZone::OnTriggerEnter
 * raceStartTime - NEWDriver::OnNtetworkSpawn
 * raceTime - LeaderboardController::Finished
 */


/// <summary>
/// This Class is ised as the base container for the player info
/// It is used to retain basic functionality for C# Dictionaries
/// </summary>
[System.Serializable]
public class PlayerInfo : MonoBehaviour
{
    // Basic player and match stats
    [Header("Do not Change")]
    public int playerID, racePosition, mapRaced, collisionsWithPlayers, collisionWithWalls, characterUsed, fellOffMap;
    public DateTime raceStartTime;
    public float raceTime;
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
        raceStartTime = DateTime.Now;
        raceTime = 0.0f;
        racePosition = 0;
        mapRaced = 0;
        collisionsWithPlayers = 0;
        collisionWithWalls = 0;
        characterUsed = 0;
        fellOffMap = 0;
        playerName = "";
        playerPassword = "";
        playerEmail = "";
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
        raceStartTime = DateTime.Now;
        raceTime = 0.0f;
        racePosition = 0;
        mapRaced = 0;
        collisionsWithPlayers = 0;
        collisionWithWalls = 0;
        characterUsed = 0;
        fellOffMap = 0;
        playerName = "Default";
        playerPassword = "";
        playerEmail = "";
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
        raceTime = other.raceTime;
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