using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// This Class is ised as the base container for the player info
/// It is used to retain basic functionality for C# Dictionaries
/// </summary>
[System.Serializable]
public class PlayerInfo : ScriptableObject
{
    // Basic player and match stats
    public int playerID, raceStartTime, racePosition, mapRaced, collisionsWithPlayers, collisionWithWalls, characterUsed;
    public Dictionary<string, int> boostUsage = new Dictionary<string, int>();
    public Dictionary<string, int> offenceUsage = new Dictionary<string, int>();
    public Dictionary<string, int> trapUsage = new Dictionary<string, int>();
}
