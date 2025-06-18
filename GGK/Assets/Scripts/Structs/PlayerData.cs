using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// PlayerObjectData by Phillip Brown
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

    public string PlayerCharacter;
    public Color PlayerColor;

    // constructor
    public PlayerData(/*ulong clientId,*/ string name, int playerNumber = -1)
    {
        //ClientId = clientId;
        PlayerNumber = playerNumber;
        playerName = name;
        PlayerName = playerName;
        PlayerCharacter = "Not Set";
        PlayerColor = Color.white;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        //serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref PlayerNumber);
        serializer.SerializeValue(ref PlayerCharacter);
        serializer.SerializeValue(ref PlayerColor);
    }

    public bool Equals(PlayerData other)
    {
        return  /*ClientId == other.ClientId &&*/
                playerName.Equals(other.playerName) &&
                PlayerNumber == other.PlayerNumber &&
                PlayerCharacter.Equals(other.PlayerCharacter) &&
                PlayerColor == other.PlayerColor;
    }
}