using Fusion;
using UnityEngine;
public class DoubleSpawner : SimulationBehaviour, IPlayerJoined
{
    [SerializeField]
    private NetworkObject playerPrefab;
    [SerializeField]
    private NetworkObject roundPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player != Runner.LocalPlayer)
            return;
        bool isMaster =
        Runner.IsSharedModeMasterClient;
        if (isMaster)
        {
            Runner.Spawn(
            roundPrefab,
            Vector3.zero,
            Quaternion.identity
            );
        }

        Vector3 spawnPosition = isMaster
        ? new Vector3(-4, 1, 0)
        : new Vector3(4, 1, 0);
        Quaternion spawnRotation = isMaster
        ? Quaternion.Euler(0, 90, 0)
        : Quaternion.Euler(0, -90, 0);
        NetworkObject playerObject = Runner.Spawn(
        playerPrefab,
        spawnPosition,
        spawnRotation
        );
        Runner.SetPlayerObject(player, playerObject);
    }
}