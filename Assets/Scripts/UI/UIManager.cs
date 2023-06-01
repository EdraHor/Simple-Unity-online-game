using Network;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI PingText;
    public float PingUpdateTime = 0.1f;
    [SerializeField] private GameObject _mainMenuPanel;
    void Start()
    {
        StartCoroutine(CheckConnection());
    }

    public void OnlineGame()
    {
        NetworkManager.Instance.ConnnectToServer();
        _mainMenuPanel.SetActive(false);
    }

    public void StartGame()
    {
        NetworkManager.Instance.StartGame();
    }

    IEnumerator CheckConnection()
    {
        while (true)
        {
            PingText.text = $"Connection: {NetworkManager.ClientConnection.IsConnected}  " +
                $"Ping: {NetworkManager.ClientConnection.Ping}";
            yield return new WaitForSeconds(PingUpdateTime);
        }
    }
}
