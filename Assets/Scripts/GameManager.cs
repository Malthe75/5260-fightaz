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
    public void LoadScene(string sceneName)
    {
        Debug.Log("GameManager: Loading scene " + sceneName);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);

    
    }
}
