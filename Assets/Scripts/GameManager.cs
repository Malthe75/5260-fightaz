using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public PlayerConfig mainPlayerConfig = new PlayerConfig();

    public List<PlayerConfig> Players = new List<PlayerConfig>();
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
        if(config == null)
        {
            Debug.LogError("GameManager: Attempted to add a null PlayerConfig.");
            return;
        }

        if (!Players.Contains(config))
        {
            Players.Add(config);
            Debug.Log($"Added Player {config.playerName} with character {config.characterDefinition.displayName}");
        }
    }
     public void OnPlayerJoined(PlayerInput input)
    {
        Debug.Log("Player Joined: " + input.playerIndex);
    }

    public void ClearPlayers()
    {
        Players.Clear();
    }


    
    }
