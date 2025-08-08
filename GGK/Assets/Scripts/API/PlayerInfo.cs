using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
/* Log to keep track of where all the dynamic values sent to backend are being assigned:
 * playerID - From backend when PlayerInfo is created in APIManager
 * racePosition - KartCheckpoint::OnTriggerEnter
 * mapRaced - GameManager::FillMapRaced (called in NEWDriver::Start)
 * collisionsWithPlayers - KartCheckpoint::OnCollisionEnter
 * collisionWithWalls - BonkCollision::OnCollisionEnter
 * characterUsed - PlayerKartHandeler::SendAppearanceToPlayerInfo
 * fellOffMap - DeathZone::OnTriggerEnter
 * raceStartTime - NEWDriver::OnNtetworkSpawn
 * raceTime - LeaderboardController::Finished
 * 
 * All the items are added in their respected classes
 */


/// <summary>
/// This Class is ised as the base container for the player info
/// It is used to retain basic functionality for C# Dictionaries
/// </summary>
[System.Serializable]
public class PlayerInfo : ScriptableObject
{
    // Basic player and match stats
    [Header("Do not Change")]
    public string pid;
    public DateTime raceStartTime;
    public float raceTime;
    public int racePos, mapRaced, characterUsed, collisionsWithPlayers, collisionWithWalls, fellOffMap;
    public Dictionary<string, int> boostUsage;
    public Dictionary<string, int> offenceUsage;
    public Dictionary<string, int> trapUsage;
    public Dictionary<string, int> defenseUsage;

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
        pid = "";
        raceStartTime = DateTime.Now;
        raceTime = 0.0f;
        racePos = 0;
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

        boostUsage = new Dictionary<string, int>
        {
            {"speedBoost1", 0 },
            {"speedBoost2", 0 },
            {"speedBoost3", 0 },
            {"speedBoost4", 0 }
        };
        offenceUsage = new Dictionary<string, int>
        {
            {"puck1", 0 },
            {"puck2", 0 },
            {"puck3", 0 },
            {"puck4", 0 }
        };
        trapUsage = new Dictionary<string, int>
        {
            {"oilSpill", 0 },
            {"brickwall", 0 },
            {"confuseritchie", 0 },
            {"fakepowerupbrick", 0 }
        };
        defenseUsage = new Dictionary<string, int>
        {
            {"defense1", 0 },
            {"defense2", 0 },
            {"defense3", 0 },
            {"defense4", 0 }
        };

        isGuest = false;
    }

    // Param constructor
    public PlayerInfo(ulong clientID)
    {
        this.clientID = clientID;
        pid = "";
        raceStartTime = DateTime.Now;
        raceTime = 0.0f;
        racePos = 0;
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

        boostUsage = new Dictionary<string, int>
        {
            {"speedBoost1", 0 },
            {"speedBoost2", 0 },
            {"speedBoost3", 0 },
            {"speedBoost4", 0 }
        };
        offenceUsage = new Dictionary<string, int>
        {
            {"puck1", 0 },
            {"puck2", 0 },
            {"puck3", 0 },
            {"puck4", 0 }
        };
        trapUsage = new Dictionary<string, int>
        {
            {"oilSpill", 0 },
            {"brickwall", 0 },
            {"confuseritchie", 0 },
            {"fakepowerupbrick", 0 }
        };
        defenseUsage = new Dictionary<string, int>
        {
            {"shield1", 0 },
            {"shield2", 0 },
            {"shield3", 0 },
            {"shield4", 0 }
        };

        isGuest = false;

    }

    // Copy constructor
    public PlayerInfo(PlayerInfo other)
    {
        pid = other.pid;
        raceStartTime = other.raceStartTime;
        raceTime = other.raceTime;
        racePos = other.racePos;
        mapRaced = other.mapRaced;
        collisionsWithPlayers = other.collisionsWithPlayers;
        collisionWithWalls = other.collisionWithWalls;
        characterUsed = other.characterUsed;
        fellOffMap = other.fellOffMap;

        boostUsage = new Dictionary<string, int>(other.boostUsage);
        offenceUsage = new Dictionary<string, int>(other.offenceUsage);
        trapUsage = new Dictionary<string, int>(other.trapUsage);
        defenseUsage = new Dictionary<string, int>(other.defenseUsage);
    }
}