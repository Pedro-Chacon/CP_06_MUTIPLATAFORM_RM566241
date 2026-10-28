using Fusion;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
public class DoubleHUD : MonoBehaviour
{
    [SerializeField] private NetworkRunner runner;
    [Header("HUD")]
    [SerializeField] private TMP_Text matchStatus;
    [Header("Resultado")]
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private TMP_Text resultText;

    [SerializeField] public int coinCount;
    [SerializeField] public TMP_Text coinText;

    private void Start()
    {
        resultsPanel.SetActive(false);

    }
    private void Update()
    {
        if (runner == null || !runner.IsRunning)
            return;
        RoundManager match = RoundManager.Instance;
        if (match == null)
        {
            matchStatus.text =
            "Aguardando estado da partida...";
            return;
        }
        UpdateCoin();
        if (match.IsOver)
        {
            ShowResult(match);
        }
        else
        {
            resultsPanel.SetActive(false);
            matchStatus.text = match.CanPlay
            ? "Duelo em andamento"
            : "Aguardando segundo jogador...";
        }
    }
    private void UpdateCoin()
    {
        if (runner.TryGetPlayerObject(
        runner.LocalPlayer,
        out NetworkObject playerObject))
        {
            RoundManager coin = playerObject.GetComponent<RoundManager>();

                coinText.text =
                "Coin: " + coinCount + " / 3";
            
        }
    }
    private void ShowResult(RoundManager match)
    {
        resultsPanel.SetActive(true);
        matchStatus.text = "Partida encerrada";
        bool localPlayerWon =
        match.Winner == runner.LocalPlayer;
        string winnerName =
        match.Winner == match.BluePlayer ? "AZUL" : "VERMELHO";
        string message = localPlayerWon ? "VOCÊ VENCEU!" : "VOCÊ PERDEU!";
        resultText.text = message + "\nVencedor: Jogador " + winnerName;
    }
}
