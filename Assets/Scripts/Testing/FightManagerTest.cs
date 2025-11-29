using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FightManagerTest : MonoBehaviour
{
    // Needed for spawning prefab.
    public Transform player1Spawn;
    public Transform player2Spawn;
    public GameObject playerPrefab1;
    public GameObject playerPrefab2;
    public List<GameObject> stages;
    public PlayerInput p1;
    public PlayerInput p2;
    private void Start()
    {
        SpawnPlayers();
       

    }
    private void SpawnPlayers()
    {

        // Clear any previous players that may have been carried from another scene
        foreach (var input in FindObjectsOfType<PlayerInput>())
        {
            Destroy(input.gameObject);
        }

        // Spawn both players
        p1 = PlayerInput.Instantiate(playerPrefab1, controlScheme: "Keyboard", pairWithDevice: Keyboard.current);
        p1.transform.position = player1Spawn.position;
        Vector3 newScale = p1.transform.localScale;
        newScale.x = -0.5f;
        newScale.y = 0.5f;
        p1.transform.localScale = newScale;
        p1.gameObject.name = "Player1";
        p1.tag = "Player1";


        p2 = PlayerInput.Instantiate(playerPrefab2, controlScheme: "Gamepad", pairWithDevice: Gamepad.current);
        p2.transform.position = player2Spawn.position;
        Vector3 newScale2 = p2.transform.localScale;
        newScale2.x = 0.5f;
        newScale2.y = 0.5f;
        p2.transform.localScale = newScale2;
        p2.gameObject.name = "Player2";
        p2.tag = "Player2";


        // Set enemies
        NewPlayerController pc1 = p1.GetComponent<NewPlayerController>();
        NewPlayerController pc2 = p2.GetComponent<NewPlayerController>();
        pc1.SetEnemy(pc2);
        pc2.SetEnemy(pc1);
    }
}

