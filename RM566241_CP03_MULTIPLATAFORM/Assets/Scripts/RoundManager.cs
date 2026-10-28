using Fusion;
using UnityEngine;

public class RoundManager : NetworkBehaviour
{
    public static RoundManager Instance { get; private set; }

    [Header("Regras")]
    [SerializeField] private int coinsToWin = 3;
    [SerializeField] private float roundSeconds = 45f;
    [SerializeField] private int totalCoins = 5;

    [Header("Moedas")]
    [SerializeField] private NetworkObject coinPrefab;
    [SerializeField] private Vector2 spawnArea = new Vector2(8f, 8f);
    [SerializeField] private float coinHeight = 1f;

    [Networked] public NetworkBool IsPlaying { get; set; }
    [Networked] public NetworkBool IsOver { get; set; }
    [Networked] public NetworkBool TimeExpired { get; set; }
    [Networked] public PlayerRef Winner { get; set; }
    [Networked] public PlayerRef FirstPlayer { get; set; }
    [Networked] public int CoinsFirst { get; set; }
    [Networked] public int CoinsSecond { get; set; }
    [Networked] public TickTimer RoundTimer { get; set; }

    public int CoinsToWin => coinsToWin;
    public bool CanPlay => IsPlaying && !IsOver;

    public bool HasTwoPlayers
    {
        get
        {
            int count = 0;
            foreach (PlayerRef p in Runner.ActivePlayers)
            {
                count++;
            }
            return count == 2;
        }
    }

    public override void Spawned()
    {
        Instance = this;

        if (HasStateAuthority)
        {
            FirstPlayer = Runner.LocalPlayer;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority || IsOver)
            return;

        if (!IsPlaying)
        {
            if (HasTwoPlayers)
                StartRound();
            return;
        }

        if (RoundTimer.Expired(Runner))
        {
            TimeExpired = true;
            IsOver = true;
        }
    }

    private void StartRound()
    {
        IsPlaying = true;
        RoundTimer = TickTimer.CreateFromSeconds(Runner, roundSeconds);

        for (int i = 0; i < totalCoins; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-spawnArea.x, spawnArea.x),
                coinHeight,
                Random.Range(-spawnArea.y, spawnArea.y)
            );
            Runner.Spawn(coinPrefab, pos, Quaternion.identity);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_CollectCoin(NetworkId coinId, RpcInfo info = default)
    {
        if (!CanPlay)
            return;

        if (!Runner.TryFindObject(coinId, out NetworkObject coin))
            return;

        Runner.Despawn(coin);

        PlayerRef collector = info.Source;
        int total;

        if (collector == FirstPlayer)
        {
            CoinsFirst++;
            total = CoinsFirst;
        }
        else
        {
            CoinsSecond++;
            total = CoinsSecond;
        }

        if (total >= coinsToWin)
        {
            Winner = collector;
            IsOver = true;
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (Instance == this)
            Instance = null;
    }
}