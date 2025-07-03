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
    public int clientID;

    // Properties
    public string PlayerName { get; private set; }

    public PlayerKart PlayerCharacter; // should probably be an enum referencing each character
    public Color PlayerColor;

    // constructor
    public PlayerData(/*ulong clientId,*/ string name, int playerNumber = -1)
    {
        //ClientId = clientId;
        clientID = playerNumber;
        playerName = name;
        PlayerName = playerName;
        PlayerCharacter = PlayerKart.Freddie;
        PlayerColor = Color.white;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        //serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref clientID);
        serializer.SerializeValue(ref PlayerCharacter);
        serializer.SerializeValue(ref PlayerColor);
    }

    public bool Equals(PlayerData other)
    {
        return  /*ClientId == other.ClientId &&*/
                playerName.Equals(other.playerName) &&
                clientID == other.clientID &&
                PlayerCharacter == other.PlayerCharacter &&
                PlayerColor == other.PlayerColor;
    }
}