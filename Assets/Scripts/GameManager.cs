using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public PlayerConfig mainPlayerConfig = new PlayerConfig();
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
    
     public void OnPlayerJoined(PlayerInput input)
    {
        Debug.Log("Player Joined: " + input.playerIndex);
        // var config = new PlayerConfig
        // {
        //     playerName = "Player " + (input.playerIndex + 1),
        //     characterPrefab = null, // Assign default character prefab if needed
        //     inputDevice = input.devices[0],
        //     controlScheme = input.currentControlScheme
        // };
    }
    
    }
