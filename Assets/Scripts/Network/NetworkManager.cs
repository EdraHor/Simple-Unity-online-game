using LiteNetLib;
using OnlineGame.Networking;
using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public delegate void ReceiveMessage(MessageData messageData);

    public class NetworkManager: MonoBehaviour
    {
        private static NetworkManager _instance;
        public static NetworkManager Instance { get { return _instance; } }

        public int _port = 8404; // порт для приема входящих запросов
        public string _ipAddress = "195.2.92.6"; // адрес сервера
        [SerializeField] private string _connectionKey = "SomeConnectionKey"; // ключ для подключения
        [SerializeField] private GameObject _mainMenuPanel;
        private Autorization _autorization;

        public Messenger GlobalMessenger { get { return FindObjectOfType<Messenger>(); } }
        public static ClientCoreNetLib ClientConnection { get; } = new ClientCoreNetLib();
        public static AnswerHandler ServerAnswerHandler { get; } = new AnswerHandler();
        public bool IsLogIn = false;
        public Room CurrentRoom { get { return FindObjectOfType<Room>(); } }


        public static NetworkObject LocalPlayerObject; //Объект в мире который передвигается нами
        public PlayerData LocalPlayerData; //Данные о позицие, логине, нике и т.д.

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            var room = FindObjectOfType<Room>();
            if (room)
                room.Disconnect();

            ClientConnection.Stop();
            SceneManager.LoadSceneAsync(0);
        }

        public void StartGame()
        {
            if (ClientConnection.IsConnected && IsLogIn)
                SceneManager.LoadSceneAsync(1);
            else
                Debug.LogError("Вы не авторизованы или нет связи с сервером");
        }

        public void ConnnectToServer()
        {
            if (!ClientConnection.IsConnected)
            {
                ClientConnection.ConnectToServer(_ipAddress, _port, _connectionKey);
                ClientConnection.Listener.NetworkReceiveEvent += OnDataReceivedFromServer;
                _autorization = FindObjectOfType<Autorization>();
                //TODO: ожидание авторизации
                _autorization.gameObject.SetActive(true);

                ClientConnection.Listener.NetworkErrorEvent += (endPoint, socketError) =>
                {
                    Debug.LogWarning(socketError);
                };
                ClientConnection.Listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
                {
                    Debug.LogWarning("[CLIENT] disconnected because " + disconnectInfo.Reason);
                };
            }
        }

        IEnumerator CheckConnection()
        {
            while(true)
            {
                if (!ClientConnection.IsConnected)
                    StartCoroutine(DelayToReconnect());

                yield return new WaitForSeconds(5);
            }
        }

        IEnumerator DelayToReconnect()
        {
            int reconnectCount = 0;
            while (reconnectCount < 10 && !ClientConnection.IsConnected)
            {
                reconnectCount++;;
                SendEventFromServer(PacketType.Login, LocalPlayerData.AccessData, DeliveryMethod.ReliableOrdered);
                Debug.LogWarning("Попытка переподключения к серверу #" + reconnectCount);
                yield return new WaitForSecondsRealtime(5);
            }
            if (!ClientConnection.IsConnected) Debug.LogWarning("Не удается подключиться к серверу TIMEOUT");
        }

        private void OnDataReceivedFromServer(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            try
            {
                var data = reader.RawData.Skip(reader.UserDataOffset).Take(reader.UserDataSize).ToArray();

                using var stream = new MemoryStream(data);
                NetworkPacket packet = Serializer.Deserialize<NetworkPacket>(stream);

                Debug.Log($"Получено событие: {packet.PacketType}");
                //Переносим данные из потока сервера в главный поток Unity
                UnityThread.instance.AddJob(new Action(() => OnEventReceive(packet.PacketType, packet.Data)));
                
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        private void OnEventReceive(PacketType packetType, byte[] data)
        {
            switch (packetType)
            {
                case PacketType.ChatMessage:
                    OnChatMessageEventReceived(data);
                    break;
                case PacketType.Answer:
                    OnAnswerEventReceive(data);
                    break;
                case PacketType.OnDisconnect:
                    OnDisconnectEventReceive(data);
                    break;
                case PacketType.GiveData:
                    OnGetDataEventReceive(data);
                    break;

                default:
                    Debug.LogWarning($"ERROR: получено неизвестное событие {packetType}");
                    break;
            }
        }

        private T DeserializePacketData<T>(byte[] data)
        {
            try
            {
                using var stream = new MemoryStream(data);
                T packetData = Serializer.Deserialize<T>(stream);
                return packetData;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception();
            }
        }

        //Получаем данные игрока, его имя и информацию о его старом местоположении
        private void OnGetDataEventReceive(byte[] data)
        {
            PlayerData playerData = DeserializePacketData<PlayerData>(data);// JsonUtility.FromJson<PlayerData>(data);
            LocalPlayerData = playerData;
            _autorization.OnLocalDataReceived();
        }

        #region Event Handlers
        private void OnDisconnectEventReceive(byte[] data)
        {
            Debug.Log("Кто-то отключился");
        }

        private void OnAnswerEventReceive(byte[] data)
        {
            var answer = DeserializePacketData<Answer>(data);
            ServerAnswerHandler.OnAnswerReceive(answer);
        }

        private void OnChatMessageEventReceived(byte[] data)
        {
            var message = DeserializePacketData<MessageData>(data);
            GlobalMessenger.OnMessageReceive(message);
        }
        #endregion


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

        public static void SendEventFromServer<T>(PacketType packetType, T packetData, DeliveryMethod deliveryMethod) where T : class
        {
            var data = SerializePacket(packetType, packetData);
            ClientConnection.SendImmediate(data, deliveryMethod);
        }
    }
}
