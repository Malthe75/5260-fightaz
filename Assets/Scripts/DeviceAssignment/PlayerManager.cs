using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public List<PlayerConfig> players = new List<PlayerConfig>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddPlayer(PlayerConfig config)
    {
        players.Add(config);
    }

    public void ClearPlayers()
    {
        players.Clear();
    }

    public int GetPlayerCount()
    {
        return players.Count;
    }

    public CharacterDefinition GetCharacterForPlayer(int playerIndex)
    {
        var player = players.FirstOrDefault(p => p.playerIndex == playerIndex);
        return player != null ? player.characterDefinition : null;
    }

    public void RemovePlayer(int playerIndex)
    {
        var playerToRemove = players.FirstOrDefault(p => p.playerIndex == playerIndex);
        if (playerToRemove != null)
        {
            players.Remove(playerToRemove);
            Debug.Log($"PlayerManager: Removed Player {playerToRemove.playerName}");
        }
        else
        {
            Debug.LogWarning($"PlayerManager: No player found with index {playerIndex} to remove.");
        }
    }


    public void OnPlayerJoined(PlayerInput pi)
    {
        Debug.Log("PlayerManager: Player joined with device " + pi.currentControlScheme + " for player " + (pi.playerIndex + 1));
    }

}
