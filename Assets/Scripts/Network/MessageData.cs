using ProtoBuf;
using System;

namespace OnlineGame.Networking
{
    public enum MessageType
    {
        Global, Local, System
    }

    /// <summary>
    /// Информация о сообщении
    /// </summary>
    [Serializable, ProtoContract]
    public class MessageData
    {
        [ProtoMember(1)] public string SenderLogin { get; }
        [ProtoMember(2)] public string SenderName { get; set; }
        [ProtoMember(3)] public string Message { get; }
        [ProtoMember(4)] public DateTime SendTime { get; }
        [ProtoMember(5)] public MessageType Type { get; set; }

        public MessageData(string senderLogin, string message, MessageType messageType)
        {
            SenderLogin = senderLogin;
            Message = message;
            SendTime = DateTime.Now;
            Type = messageType;
        }
        public MessageData() {}
    }

    public enum PacketType
    {
        Transfrom, ChatMessage, Registration, Login, RequestData, GiveData, OnDisconnect, Answer, Spawn
    }

    [ProtoContract]
    public class NetworkPacket
    {
        public NetworkPacket(PacketType senderLogin, byte[] data)
        {
            PacketType = senderLogin;
            Data = data;
        }
        public NetworkPacket() {}

        [ProtoMember(1)] public PacketType PacketType { get; }
        [ProtoMember(2)] public byte[] Data { get; set; }
    }
}
