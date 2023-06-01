using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace OnlineGame.Networking
{
    public delegate void ClientHandlePacketData(byte[] data, int bytesRead);

    /// <summary>
    /// Реализует простой TCP-клиент, который подключается к указанному серверу и
    /// вызывает C# события при получении данных с сервера
    /// </summary>
    public class ClientCore
    {
        public event ClientHandlePacketData OnDataReceived;

        private TcpClient tcpClient;
        private NetworkStream clientStream;
        private NetworkBuffer buffer;
        private int writeBufferSize = 1024;
        private int readBufferSize = 1024;
        private int port;
        private bool started = false;

        /// <summary>
        /// Constructs a new client
        /// </summary>
        public ClientCore()
        {
            buffer = new NetworkBuffer();
            buffer.WriteBuffer = new byte[writeBufferSize];
            buffer.ReadBuffer = new byte[readBufferSize];
            buffer.CurrentWriteByteCount = 0;
        }

        /// <summary>
        /// Инициирует TCP-соединение с TCP-сервером с заданным адресом и портом
        /// </summary>
        /// <param name="ipAddress">The IP address (IPV4) of the server</param>
        /// <param name="port">The port the server is listening on</param>
        public void ConnectToServer(string ipAddress, int port)
        {
            this.port = port;

            tcpClient = new TcpClient(ipAddress, port);
            clientStream = tcpClient.GetStream();
            Console.WriteLine("Connected to server, listening for packets");

            Thread t = new Thread(new ThreadStart(ListenForPackets));
            started = true;
            t.Start();
        }

        /// <summary>
        /// Этот метод работает в собственном потоке и отвечает за
        /// получение данных с сервера и вызов события, когда данные
        /// получено
        /// </summary>
        private void ListenForPackets()
        {
            int bytesRead;

            while (started)
            {
                bytesRead = 0;

                try
                {
                    //Блокирует поток до получения сообщения от сервера
                    bytesRead = clientStream.Read(buffer.ReadBuffer, 0, readBufferSize);
                }
                catch
                {
                    //A socket error has occurred
                    Console.WriteLine("A socket error has occurred with the client socket " + tcpClient.ToString());
                    break;
                }

                if (bytesRead == 0)
                {
                    //The server has disconnected
                    break;
                }

                //Отправляем данные другим классам для обработки
                OnDataReceived?.Invoke(buffer.ReadBuffer, bytesRead);

                Thread.Sleep(15);
            }

            started = false;
            Disconnect();
        }

        /// <summary>
        /// Добавляет данные в отправляемый пакет, но не отправляет его по сети
        /// </summary>
        /// <param name="data">The data to be sent</param>
        public void AddToPacket(byte[] data)
        {
            if (buffer.CurrentWriteByteCount + data.Length > buffer.WriteBuffer.Length)
            {
                FlushData();
            }

            Array.ConstrainedCopy(data, 0, buffer.WriteBuffer, buffer.CurrentWriteByteCount, data.Length);
            buffer.CurrentWriteByteCount += data.Length;
        }

        /// <summary>
        /// Flushes all outgoing data to the server
        /// </summary>
        public void FlushData()
        {
            clientStream.Write(buffer.WriteBuffer, 0, buffer.CurrentWriteByteCount);
            clientStream.Flush();
            buffer.CurrentWriteByteCount = 0;
        }

        /// <summary>
        /// Sends the byte array data immediately to the server
        /// </summary>
        /// <param name="data"></param>
        public void SendImmediate(byte[] data)
        {
            AddToPacket(data);
            FlushData();
        }

        /// <summary>
        /// Сообщает, подключены ли мы к серверу
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            return started && tcpClient.Connected;
        }

        /// <summary>
        /// Disconnect from the server
        /// </summary>
        public void Disconnect()
        {
            if (tcpClient == null)
            {
                return;
            }

            Console.WriteLine("Disconnected from server");

            tcpClient.Close();

            started = false;
        }
    }
}