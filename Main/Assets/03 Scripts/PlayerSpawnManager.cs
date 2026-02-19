using Fusion;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public NetworkRunner runner;
    public NetworkObject networkedAvatarPrefab;

    // Set spawn points
    public Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(0, 0, 0), // Player 1
        new Vector3(2, 0, 0), // Player 2
        new Vector3(-2, 0, 0), // Player 3
    };

    private int spawnIndex = 0;

    public void OnPlayerJoined(PlayerRef player)
    {
        if (!runner.IsServer) return; // Solo el host spawnea

        // Select Vector3
        Vector3 spawnPos = spawnPositions[spawnIndex % spawnPositions.Length];
        spawnIndex++;

        // Spawn NetworkedAvatar
        runner.Spawn(networkedAvatarPrefab, spawnPos, Quaternion.identity, player);
    }
}

