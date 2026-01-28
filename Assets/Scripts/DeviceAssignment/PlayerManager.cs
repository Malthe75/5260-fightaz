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
    public bool IsDeviceUsed(InputDevice device)
    {
        return players.Any(p => p.inputDevice == device);
    }

    public void ChangeControlScheme(string controlScheme, int playerIndex)
    {
        var player = players.Find(p => p.playerIndex == playerIndex);
        if (player != null)
        {
            player.controlScheme = controlScheme;
        }
    }


    public void OnPlayerJoined(PlayerInput pi)
    {
        Debug.Log("PlayerManager: Player joined with device " + pi.currentControlScheme + " for player " + (pi.playerIndex + 1));
    }

}
