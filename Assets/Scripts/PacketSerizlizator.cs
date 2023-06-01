using OnlineGame.Networking;
using ProtoBuf;
using System;
using System.IO;
using UnityEngine;

public static class PacketSerializator
{
    /// <summary>
    /// Двойная сериализация: сериализация данных, пакетирование и сериализация пакета
    /// </summary>
    public static byte[] SerializeToPacket<T>(PacketType packetType, T packetData) where T : class
    {
        //Сериализуем данные
        using var stream = new MemoryStream();
        Serializer.Serialize(stream, packetData);


        //Сериализуем пакет для данных
        var data = new NetworkPacket(packetType, stream.ToArray());
        stream.SetLength(0);
        Serializer.Serialize(stream, data);

        return stream.ToArray();
    }
    /// <summary>
    /// Десериализует пакет, но не его поле Data
    /// </summary>
    public static NetworkPacket DeserializePacket(byte[] data)
    {
        using var stream = new MemoryStream(data);
        NetworkPacket packet = Serializer.Deserialize<NetworkPacket>(stream);

        return packet;
    }
    /// <summary>
    /// Десериализует байты в классы
    /// </summary>
    public static T DeserializePacketData<T>(byte[] data)
    {
        try
        {
            using var stream = new MemoryStream(data);
            T packetData = Serializer.Deserialize<T>(stream);
            return packetData;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw new Exception();
        }
    }
}