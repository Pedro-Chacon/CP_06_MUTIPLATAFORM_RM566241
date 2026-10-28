using Fusion;
using TMPro;
using UnityEngine;

public class DoubleHUD : MonoBehaviour
{
    [SerializeField] private NetworkRunner runner;

    [Header("HUD")]
    [SerializeField] private TMP_Text matchStatus;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text timerText;

    [Header("Painéis de resultado")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject timeOverPanel;

    private void Start()
    {
        HideResults();
    }

    private void Update()
    {
        if (runner == null || !runner.IsRunning)
            return;

        RoundManager round = RoundManager.Instance;

        if (round == null)
        {
            matchStatus.text = "Aguardando estado da partida...";
            return;
        }

        UpdateCoins(round);

        if (round.IsOver)
        {
            ShowResult(round);
            return;
        }

        HideResults();
        UpdateTimer(round);

        matchStatus.text = round.IsPlaying
            ? "Pegue " + round.CoinsToWin + " moedas!"
            : "Aguardando segundo jogador...";
    }

    private void UpdateCoins(RoundManager round)
    {
        int mine = runner.LocalPlayer == round.FirstPlayer
            ? round.CoinsFirst
            : round.CoinsSecond;

        coinText.text = "Moedas: " + mine + " / " + round.CoinsToWin;
    }

    private void UpdateTimer(RoundManager round)
    {
        if (!round.IsPlaying)
        {
            timerText.text = "--";
            return;
        }

        float? remaining = round.RoundTimer.RemainingTime(runner);
        timerText.text = Mathf.CeilToInt(Mathf.Max(0f, remaining ?? 0f)).ToString();
    }

    private void ShowResult(RoundManager round)
    {
        matchStatus.text = "Partida encerrada";

        bool iWon = round.Winner == runner.LocalPlayer;

        winPanel.SetActive(!round.TimeExpired && iWon);
        losePanel.SetActive(!round.TimeExpired && !iWon);
        timeOverPanel.SetActive(round.TimeExpired);
    }

    private void HideResults()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        timeOverPanel.SetActive(false);
    }
}