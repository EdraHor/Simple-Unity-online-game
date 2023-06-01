using OnlineGame.Networking;
using TMPro;
using UnityEngine;

/// <summary>
/// ���������� ������������� ���������
/// </summary>
public class UI_Message : MonoBehaviour
{
    public int Id;
    public TextMeshProUGUI Sender;
    public TextMeshProUGUI Message;
    public TextMeshProUGUI Time;
    /// <summary>
    /// ������ ��������� ���������
    /// </summary>
    public MessageType Type;
}
