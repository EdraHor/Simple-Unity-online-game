using LiteNetLib;
using Network;
using OnlineGame.Networking;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Messenger : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputText;
    [SerializeField] private UnityEngine.Transform Content;//��������� � ������� ���������� ���������
    [SerializeField] private UI_ResourcesSO Resources; //������ ������� �������� ���������
    public List<MessageData> Messages;

    private void Start()
    {
        DrawSystemMessage("����� ���������� � ���!");
    }

    public void OnMessageReceive(MessageData message) => DrawMessage(message);

    public void SendMessageFromServer()
    {
        if (_inputText.text == "")
            return;

        var messageData = new MessageData(NetworkManager.Instance.LocalPlayerData.AccessData.UserName,
            _inputText.text, MessageType.Global);
        NetworkManager.SendEventFromServer(PacketType.ChatMessage, messageData, DeliveryMethod.ReliableOrdered);
        _inputText.text = "";
        DrawLocalMessage(messageData);
    }

    private void DrawLocalMessage(MessageData message)
    {
        var newMessage = Instantiate(Resources.Chat_LocalMessage, Content);
        newMessage.Message.text = message.Message;
        newMessage.Time.text = message.SendTime.ToShortTimeString();
        InitMessage(newMessage, message);
    }

    private void DrawMessage(MessageData Message) //TODO: �����������
    {
        UI_Message newMessage;

        switch (Message.Type) //������� ������� ��������� ��� ���������
        {
            case MessageType.Global:
                newMessage = Instantiate(Resources.Chat_GlobalMessage, Content);
                newMessage.Sender.text = Message.SenderName;
                newMessage.Message.text = Message.Message;
                newMessage.Time.text = Message.SendTime.ToShortTimeString();
                InitMessage(newMessage, Message);
                break;

            case MessageType.Local:
                newMessage = Instantiate(Resources.Chat_LocalMessage, Content);
                newMessage.Message.text = Message.Message;
                newMessage.Time.text = Message.SendTime.ToShortTimeString();
                InitMessage(newMessage, Message);
                break;

            case MessageType.System:
                newMessage = Instantiate(Resources.Chat_SystemMessage, Content);
                newMessage.Message.text = Message.Message;
                InitMessage(newMessage, Message);
                break;
        }
    }

    private void InitMessage(UI_Message newUIMessage, MessageData newChatMessage) //���������� � ������� ���������, ���������� ���� � ��������� � ������
    {
        newUIMessage.transform.SetSiblingIndex(0); // ��������� ����� ��������� � ������ (���) �������� ���������
        Messages.Add(newChatMessage);
        newUIMessage.Id = Messages.Count + 1;
    }

    public void DrawSystemMessage(string message)
    {
        DrawMessage(new MessageData("system", message, MessageType.System));
    }

    //public void RemoveMessage(int MessageID)
    //{        
    //    var removedMessage = Messages.First(k => k.Id == MessageID); // ���� ��������� �� �����
    //    Messages.Remove(removedMessage);
    //}
}
