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

    public bool IsDeviceUsed(InputDevice device)
    {
        return players.Any(p => p.inputDevice == device);
    }
}
