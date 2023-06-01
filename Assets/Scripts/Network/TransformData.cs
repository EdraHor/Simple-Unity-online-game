using ProtoBuf;
using System;

namespace Network
{

    /// <summary>
    /// Данные пользователя для пересылки клиенту
    /// </summary>
    [Serializable, ProtoContract]
    public class PlayerData
    {
        [ProtoMember(1)] public string PlayerID { get; } //Уникальный идентификатор пользователя
        [ProtoMember(2)] public AccessData AccessData { get; } //Логин и пароль пользователя для доступа
        [ProtoMember(3)] public TransformData Transform { get; set; } //Текущее местонахождение клиента

        public PlayerData(string playerId, AccessData accessData, TransformData data)
        {
            this.PlayerID = playerId;
            this.AccessData = accessData;
            this.Transform = data;
        }

        public PlayerData() { }
    }

    /// <summary>
    /// Создержит информацию для авторизации клиента и доступа к классу Player
    /// </summary>
    [Serializable, ProtoContract]
    public class AccessData
    {
        [ProtoMember(1)] public string UserName { get; set; }
        [ProtoMember(2)] public string Login { get; private set; }
        [ProtoMember(3)] public string Password { get; private set; }

        public AccessData(string userName, string login, string password)
        {
            this.UserName = userName;
            this.Login = login;
            this.Password = password;
        }

        public AccessData() { }
    }

    //Делим данные между сценами, в каждой могут происходить свои независимые передвижения

    [Serializable, ProtoContract]
    public class TransformData
    {
        [ProtoMember(1)] public string SceneName { get; set; }
        [ProtoMember(2)] public string PrefabName { get; set; }
        [ProtoMember(3)] public Transform Transform { get; set; }

        public TransformData(string sceneName, string prefabName, Transform transform)
        {
            this.SceneName = sceneName;
            this.Transform = transform;
            this.PrefabName = prefabName;
        }

        public TransformData() { }
    }

    [Serializable, ProtoContract]
    public class Transform
    {
        [ProtoMember(1)] public Position Position { get; }
        [ProtoMember(2)] public Rotation Rotation { get; }

        public Transform(Position position, Rotation rotation)
        {
            this.Position = position;
            this.Rotation = rotation;
        }

        public Transform() { }
    }

    [Serializable, ProtoContract]
    public struct Position
    {
        [ProtoMember(1)] public float X { get; set; }
        [ProtoMember(2)] public float Y { get; set; }
        [ProtoMember(3)] public float Z { get; set; }

        public Position(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
    [Serializable, ProtoContract]
    public struct Rotation
    {
        [ProtoMember(1)] public float X { get; set; }
        [ProtoMember(2)] public float Y { get; set; }
        [ProtoMember(3)] public float Z { get; set; }

        public Rotation(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
