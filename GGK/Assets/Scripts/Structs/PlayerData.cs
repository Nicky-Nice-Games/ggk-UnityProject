using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// PlayerObject by Phillip Brown
/// </summary>
public struct PlayerData : INetworkSerializable, IEquatable<PlayerData>
{
    // private fields
    private string playerName;

    // public fields
    //public ulong ClientId;
    public int PlayerNumber;

    // Properties
    public string PlayerName { get; private set; }

    //public PlayerKart PlayerCharacter; // should probably be an enum referencing each character

    public String CharacterName;
    public Color CharacterColor;

    // constructor
    public PlayerData(/*ulong clientId,*/ string name, int playerNumber = -1)
    {
        //ClientId = clientId;
        PlayerNumber = playerNumber;
        playerName = name;
        PlayerName = playerName;
        //PlayerCharacter = PlayerKart.Freddie;
        CharacterName = "";
        CharacterColor = Color.white;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        //serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref PlayerNumber);
        //serializer.SerializeValue(ref PlayerCharacter);
        serializer.SerializeValue(ref CharacterColor);
    }

    public bool Equals(PlayerData other)
    {
        return  /*ClientId == other.ClientId &&*/
                playerName.Equals(other.playerName) &&
                PlayerNumber == other.PlayerNumber &&
                //PlayerCharacter == other.PlayerCharacter &&
                CharacterName.Equals(other.CharacterName) &&
                CharacterColor == other.CharacterColor;
    }
}