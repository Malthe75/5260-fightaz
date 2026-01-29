using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FightManager : MonoBehaviour
{

    [SerializeField] ClashManager clashManager;
    public static FightManager Instance {get; private set; }
    // Needed for spawning prefab.
    public Transform player1Spawn;
    public Transform player2Spawn;
    private PlayerInput p1;
    private PlayerInput p2;
    private NewPlayerController pc1;
    private NewPlayerController pc2;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        foreach (var input in FindObjectsOfType<PlayerInput>())
        {
            Destroy(input.gameObject);
        }
        // Player 1
        pc1 = SpawnPlayer(0, "Player1");
        // Player 2
        pc2 = SpawnPlayer(1, "Player2");

        SetEnemy(pc1, pc2);
        SetEnemy(pc2, pc1);
        SetInputs();
        
        clashManager.Initialize(p1.GetComponentInChildren<PlayerClash>(), p2.GetComponentInChildren<PlayerClash>());
    }
    
    private NewPlayerController SpawnPlayer(int playerIndex, string playerName)
    {   

        // Clear any previous players that may have been carried from another scene
        foreach (var input in FindObjectsOfType<PlayerInput>())
        {
            Destroy(input.gameObject);
        }

        GameObject player = Instantiate(PlayerManager.Instance.GetCharacterForPlayer(playerIndex).characterPrefab);
        player.transform.position = player1Spawn.position;

        if (playerIndex == 0)
        {
            Vector3 newScale = player.transform.localScale;
            newScale.x = -0.5f;
            newScale.y = 0.5f;
            player.transform.localScale = newScale;
        }else if(playerIndex == 1)
        {
            Vector3 newScale = player.transform.localScale;
            newScale.x = 0.5f;
            newScale.y = 0.5f;
            player.transform.localScale = newScale;
        }
        player.gameObject.name = playerName;
        player.tag = playerName;

        return player.GetComponent<NewPlayerController>();

    }

    private void SetEnemy(NewPlayerController pc, NewPlayerController enemy)
    {
        pc.SetEnemy(enemy);
    }

    private void SetInputs()
    {
        p1 = pc1.GetComponent<PlayerInput>();
        p2 = pc2.GetComponent<PlayerInput>();
    }

    public void SwitchAllActionMaps(string actionMap)
    {
        p1.SwitchCurrentActionMap(actionMap);
        p2.SwitchCurrentActionMap(actionMap);
    }

    public void ChangeAllStates(string stateName)
    {
        pc1.SetState(stateName);
        pc2.SetState(stateName);
    }

}
