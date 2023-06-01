using OnlineGame.Networking;
using TMPro;
using UnityEngine;

/// <summary>
/// Визуальное представление сообщения
/// </summary>
public class UI_Message : MonoBehaviour
{
    public int Id;
    public TextMeshProUGUI Sender;
    public TextMeshProUGUI Message;
    public TextMeshProUGUI Time;
    /// <summary>
    /// Способ отрисовки сообщения
    /// </summary>
    public MessageType Type;
}
