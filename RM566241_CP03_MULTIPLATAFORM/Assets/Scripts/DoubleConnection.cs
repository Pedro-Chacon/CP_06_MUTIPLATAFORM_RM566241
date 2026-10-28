using TMPro;
using UnityEngine;
using Fusion;

public class DoubleConnection : MonoBehaviour
{
    [SerializeField] private TMP_Text connectionStatus;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private NetworkRunner runner;
    [SerializeField] private TMP_InputField roomInput;


    private bool connecting;
    public async void ConnectToRoom()
    {
        if (connecting || runner.IsRunning)
        {
            return;
        }

        string roomName = roomInput.text.Trim();

        if (string.IsNullOrEmpty(roomName))
        {
            connectionStatus.text =
            "Digite o nome de uma sala!";
            return;
        }

        connecting = true;
        connectionStatus.text = "Conectando...";
        runner.ProvideInput = false;
        var sceneManager = GetComponent<NetworkSceneManagerDefault>();
        var result = await runner.StartGame(
        new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = roomName,
            PlayerCount = 2,
            SceneManager = sceneManager
            

        }
        );

        if (result.Ok)
        {
            menuPanel.SetActive(false);
        }

        else
        {
            connectionStatus.text =
            "Erro: " + result.ShutdownReason;
            Debug.LogError(result.ShutdownReason);
        }

        connecting = false;
    }
}
