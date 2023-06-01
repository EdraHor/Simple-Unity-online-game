using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using TMPro;
using System.Threading;
using System.Collections;
using System.Threading.Tasks;


public class ClientOld : MonoBehaviour
{

    //Заменить строки на блоки со скролом
    //Начать многопольвательский режим
    
//    [SerializeField] private int _port = 8404; // порт для приема входящих запросов
//    [SerializeField] private string _ipAddress = "195.2.92.6"; // порт для приема входящих запросов
//     private IPAddress _ip; //адрес сервера
//     private IPEndPoint _serverAdress;
//     private TcpClient client;
//     private NetworkStream stream;
//    [SerializeField] private string _name;
//    [SerializeField] private TextMeshProUGUI _labelText;
//    [SerializeField] private TextMeshProUGUI _inputText;
//    [SerializeField] private GameObject _namePanel;
//    [SerializeField] private TextMeshProUGUI _nameTextInput;
//    [SerializeField] private GameObject _messengerPanel;

//    private void Start()
//    {
//        _ip = IPAddress.Parse(_ipAddress);
//        _serverAdress = new IPEndPoint(_ip, _port);
//    }

//    public void EnterName()
//    {
//        if (_nameTextInput.text=="" || _nameTextInput.text==null)
//            return;

//        _name = _nameTextInput.text;
//        _namePanel.SetActive(false);
//        _messengerPanel.SetActive(true);
//        UnityMainThread.instance.AddJob(() =>
//        {
//            Messenger.Instance.DrawMessage(new Chat_MessageUI($"{_name} присоединился!"));
//            _nameTextInput.text ="";
//        });
//        ConnectToServer();

//    }   
    
//    private void ConnectToServer() {
//    {
//        client = new TcpClient();
//        try
//        {
//            client.Connect(_serverAdress);
//            stream = client.GetStream();

//            SendAsync(_name);
//            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessages));
//            receiveThread.Start();
//        }
//        catch (Exception e)
//        {
//            UnityMainThread.instance.AddJob(() =>
//            {
//                Messenger.Instance.DrawMessage(new Chat_MessageUI(e.Message));
//            });
//        }
//    }
//}

//    private void ReceiveMessages()
//    {
//        while(true)
//        {
//            try
//            {
//                byte[] data = new byte[128];
//                StringBuilder builder = new StringBuilder();
//                int count = 0;
//                do
//                {
//                    count = stream.Read(data, 0, data.Length); //stop thread
//                    builder.Append(Encoding.Unicode.GetString(data), 0, count);
//                }
//                while (stream.DataAvailable);
//                string message = builder.ToString();
//                UnityMainThread.instance.AddJob(() =>
//                {
//                    Messenger.Instance.DrawMessage(new Chat_MessageUI("User", message, DateTime.Now.ToShortTimeString()));
//                    Debug.Log(message);
//                });

//                //_labelText.text += message;
//            }
//            catch (Exception e)
//            {
//                UnityMainThread.instance.AddJob(() =>
//                {
//                    var error = "\n" + $"Disconesting from the server with error: \n {e.Message}";
//                    Messenger.Instance.DrawMessage(new Chat_MessageUI(error));
//                });
//                Disconect();
//            }
//        }
//    }

//    public async void SendAsync(string message)
//    {
//        try
//        {
//            byte[] data = Encoding.Unicode.GetBytes(message);
//            _inputText.SetText("");
//            stream.Write(data, 0, data.Length); //stop thread

//            UnityMainThread.instance.AddJob(() =>
//            {
//                Messenger.Instance.DrawMessage(new Chat_MessageUI(message, DateTime.Now.ToShortTimeString()));
//            });

//            await Task.Yield(); 
//        }
//        catch (Exception e)
//        {
//            UnityMainThread.instance.AddJob(() =>
//            {
//                Messenger.Instance.DrawMessage(new Chat_MessageUI(e.Message));
//            });
//        }
//    }
    
//    public async void SendAsync()
//    {
//        try
//        {
//            byte[] data = Encoding.Unicode.GetBytes(_inputText.text);
//            stream.Write(data, 0, data.Length); //stop thread
//            UnityMainThread.instance.AddJob(() =>
//            {
//                Messenger.Instance.DrawMessage(new Chat_MessageUI(_inputText.text, DateTime.Now.ToShortTimeString()));
//            });
//            _inputText.SetText("");

//            await Task.Yield(); 
//        }
//        catch (Exception e)
//        {
//            UnityMainThread.instance.AddJob(() =>
//            {
//                Messenger.Instance.DrawMessage(new Chat_MessageUI(e.Message));
//            });
//        }
//    }

//    private void Disconect()
//    {
//        if (stream != null)
//            stream.Close();
//        if (client != null)
//            client.Close();

//        UnityMainThread.instance.AddJob(() =>
//        {
//            Messenger.Instance.DrawMessage(new Chat_MessageUI("Вы покинули чат!"));
//        });
//    }
}
