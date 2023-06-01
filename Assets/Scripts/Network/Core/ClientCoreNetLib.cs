using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Network
{
    public class ClientCoreNetLib
    {

        public EventBasedNetListener Listener = new EventBasedNetListener();
        private readonly NetManager _netClient;

        public bool IsConnected { get {
                if (_netClient.ConnectedPeersCount > 0)
                    return _netClient.FirstPeer.ConnectionState == ConnectionState.Connected;
                else
                    return false;
            } }

        public float Ping { get { 
                if (IsConnected) 
                    return _netClient.FirstPeer.Ping;
                else return 0;
            } }

        public ClientCoreNetLib()
        {
            _netClient = new NetManager(Listener);
        }

        public void ConnectToServer(string ipAdress, int port, string conectionKey)
        {            
            _netClient.UnconnectedMessagesEnabled = true;
            _netClient.UpdateTime = 15;
            _netClient.Start();
            _netClient.Connect(ipAdress, port, conectionKey);

            Thread thread = new Thread(new ThreadStart(ListenForClients));
            thread.Start();
        }

        /// <summary>
        /// Работает в своем собственном потоке. Отвечает за прием новых клиентов и отправку их в свою собственную ветку.
        /// </summary>
        private void ListenForClients()
        {
            while (_netClient.IsRunning)
            {
                _netClient.PollEvents();
                Thread.Sleep(15);
            }
        }

        /// <summary>
        /// Stops the server from accepting new clients
        /// </summary>
        public void Stop()
        {
            if (_netClient.IsRunning)
            {
                _netClient.Stop(true);
                Debug.Log("Disconnect from the server . . . ");
            }
        }

        /// <summary>
        /// Немедленно отправляет данные массива байтов указанному клиенту
        /// </summary>
        /// <param name="data">Данные для отправки</param>
        /// <param name="peer">Клиент для отправки данных</param>
        public void SendImmediate(byte[] data, DeliveryMethod deliveryMethod)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(data);
            if (IsConnected)
                _netClient.FirstPeer.Send(writer, deliveryMethod);
            else
                Debug.LogWarning("Connection lost, reconnect..."); //Запускать очередь отправки, после востановления соединения
        }
    }
}
