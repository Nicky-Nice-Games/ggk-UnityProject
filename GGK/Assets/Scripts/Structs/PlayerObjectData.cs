using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// PlayerObjectData by Phillip Brown
/// </summary>
public struct PlayerObjectData : INetworkSerializable, IEquatable<PlayerObjectData>
{
    // private fields
    private string playerName;

    // public fields
    public ulong ClientId;
    public int PlayerNumber;

    // Properties
    public string PlayerName { get; private set; }

    // constructor
    public PlayerObjectData(ulong clientId, string name, int playerNumber)
    {
        ClientId = clientId;
        PlayerNumber = playerNumber;
        playerName = name;
        PlayerName = playerName;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref PlayerNumber);
    }

    public bool Equals(PlayerObjectData other)
    {
        return  ClientId == other.ClientId &&
                playerName.Equals(other.playerName) &&
                PlayerNumber == other.PlayerNumber;
    }
}