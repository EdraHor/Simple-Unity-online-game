using LiteNetLib;
using Network;
using OnlineGame.Networking;
using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviour
{
    public UnityEngine.Transform SpawnPoint;
    public string PrefabName;
    public bool IsSpawnOnStart = true;
    public int Port = 8405;
    private ClientCoreNetLib ConnectionToRoom = new ClientCoreNetLib();
    private PlayerData _localPlayerData;
    [SerializeField] private List<NetworkObject> _players = new List<NetworkObject>();
    private object locker = new object();

    private void Start()
    {
        ConnectionToRoom.Listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;
        ConnectionToRoom.ConnectToServer(NetworkManager.Instance._ipAddress, Port, "SomeConnectionKey");
        // Save scene name value before send to server
        var serverData = NetworkManager.Instance.LocalPlayerData;
        if (NetworkManager.Instance.LocalPlayerData.Transform == null)
        {
            var sceneData = new TransformData(SceneManager.GetActiveScene().name, PrefabName, null);
            NetworkManager.Instance.LocalPlayerData.Transform = sceneData;
        }
        else { NetworkManager.Instance.LocalPlayerData.Transform.SceneName = SceneManager.GetActiveScene().name; }

        _localPlayerData = NetworkManager.Instance.LocalPlayerData;
        if (IsSpawnOnStart)
            Spawn(_localPlayerData);

    }

    private void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        try
        {
            var data = reader.RawData.Skip(reader.UserDataOffset).Take(reader.UserDataSize).ToArray();

            NetworkPacket packet = PacketSerializator.DeserializePacket(data);

            Debug.Log($"[{_localPlayerData.Transform.SceneName}] Получено событие: {packet.PacketType}:");

            UnityThread.instance.AddJob(new Action(() => OnEventReceive(packet.PacketType, packet.Data, peer)));
        }
        catch (Exception e)
        {
            UnityThread.instance.AddJob(new Action(() => Debug.LogError("Network Eror: " + e.Message)));
        }
    }

    private void OnEventReceive(PacketType packetType, byte[] data, NetPeer peer)
    {
        switch (packetType)
        {
            case PacketType.Transfrom:
                var receivedNetObjects = PacketSerializator.DeserializePacketData<PlayerData[]>(data);
                Debug.Log("Получены координаты: " + receivedNetObjects.Count());

                foreach (var receivedObj in receivedNetObjects)
                {
                    if (receivedObj.PlayerID == _localPlayerData.PlayerID) continue;
                    var updatedObject = _players.FirstOrDefault(o => o.Id == receivedObj.PlayerID); //Если сейчас пришли данные существующего игрока
                    //if (updatedObject == null && receivedObj.PlayerID != _localPlayerData.PlayerID)
                    //    SpawnRemoteNetObject(receivedObj); //Если игрок еще не заспавлен спавним
                    if (updatedObject == null) //если объект еще не заспавлен
                        SpawnRemoteNetObject(receivedObj);
                    else
                        updatedObject.OnTransfrormReceive(receivedObj.Transform);
                }
                break;
            case PacketType.ChatMessage:
                var message = PacketSerializator.DeserializePacketData<MessageData>(data);
                NetworkManager.Instance.GlobalMessenger.OnMessageReceive(message);
                break;
            case PacketType.OnDisconnect:
                OnDisconnectEventReceive(data);
                break;
            default:
                Console.WriteLine($"[{_localPlayerData.Transform.SceneName}] Получено не существующее событие: ");
                break;
        }
    }

    private void OnDisconnectEventReceive(byte[] data)
    {
        //var netObjects = JsonConvert.DeserializeObject<TransformData>(data);
        //SpawnNetObject(netObjects);

        var netObjects = PacketSerializator.DeserializePacketData<PlayerData>(data); //Плохо: тут пароль
        var disconnectedPlayer = _players.FirstOrDefault(p => p.Id == netObjects.PlayerID);
        if (disconnectedPlayer != null)
        {
            Destroy(disconnectedPlayer.gameObject);
            _players.Remove(disconnectedPlayer);
            NetworkManager.Instance.GlobalMessenger.DrawSystemMessage($"{netObjects.AccessData.UserName} disconnected!");
        }
    }

    private void SpawnRemoteNetObject(PlayerData playerData)
    {
        var spawnObject = Resources.Load($"PlayerPrefabs/{PrefabName}") as GameObject; //TODO: создать папку и префаб

        var spawnedObj = Instantiate(spawnObject, Vector3.zero, Quaternion.identity);
        var remoteObjComponent = spawnedObj.GetComponent<NetworkObject>();
        if (remoteObjComponent == null)
        {
            Debug.LogError($"{remoteObjComponent} не содержит компонента NetworkObject");
            return;
        }

        remoteObjComponent.Initialize(false, playerData);
        _players.Add(remoteObjComponent);
    }

    //Вызывается при подключении к комнате
    public void Spawn(PlayerData playerData) //Создает объект игрока в данной игровой комнате
    {
        NetworkManager.SendEventFromServer(PacketType.Spawn, playerData, DeliveryMethod.ReliableOrdered);

        var spawnObject = Resources.Load($"PlayerPrefabs/{PrefabName}") as GameObject;

        var spawnedObj = Instantiate(spawnObject, SpawnPoint.position, SpawnPoint.rotation);
        var localeObjComponent = spawnedObj.GetComponent<NetworkObject>();
        localeObjComponent.Initialize(true, playerData);
    }

    private static byte[] SerializePacket<T>(PacketType packetType, T packetData) where T : class
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

    public void SendEventFromRoom<T>(PacketType packetType, T packetData, DeliveryMethod deliveryMethod) where T : class
    {
        var data = SerializePacket(packetType, packetData);
        ConnectionToRoom.SendImmediate(data, deliveryMethod);
    }

    public void Disconnect()
    {
        ConnectionToRoom.Stop();
    }
}
