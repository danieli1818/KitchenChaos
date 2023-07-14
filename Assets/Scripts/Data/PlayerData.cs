using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public int colorIndex;
    public FixedString64Bytes playerId;
    public FixedString64Bytes playerName;

    public bool Equals(PlayerData other) {
        return clientId == other.clientId && colorIndex == other.colorIndex && playerId == other.playerId && playerName == other.playerName;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorIndex);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref playerName);
    }
}
