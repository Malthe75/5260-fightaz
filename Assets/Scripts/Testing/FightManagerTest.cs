using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FightManagerTest : MonoBehaviour
{
    public Transform player1Spawn;
    public Transform player2Spawn;
    public GameObject playerPrefab; // Reference to the player prefab
    public List<GameObject> stages;
    public PlayerInput p1;
    public PlayerInput p2;
    public SpriteRenderer p1sr;
    public SpriteRenderer p2sr;
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
        p1 = PlayerInput.Instantiate(playerPrefab, controlScheme: "Keyboard", pairWithDevice: Keyboard.current);
        p1.transform.position = player1Spawn.position;
        Vector3 newScale = p1.transform.localScale;
        newScale.x = -0.5f;
        newScale.y = 0.5f;
        p1.transform.localScale = newScale;
        p1.gameObject.name = "Player1";
        p1.tag = "Player1";
        //p1.gameObject.layer = LayerMask.NameToLayer("Player1");
        p1sr = p1.GetComponentInChildren<SpriteRenderer>();
        p1.transform.Find("Hurtbox").gameObject.tag = "Player1";


        p2 = PlayerInput.Instantiate(playerPrefab, controlScheme: "Gamepad", pairWithDevice: Gamepad.current);
        p2.transform.position = player2Spawn.position;
        Vector3 newScale2 = p2.transform.localScale;
        newScale2.x = 0.5f;
        newScale2.y = 0.5f;
        p2.transform.localScale = newScale2;
        p2.gameObject.name = "Player2";
        p2.tag = "Player2";
        //p2.gameObject.layer = LayerMask.NameToLayer("Player2");
        p2sr = p2.GetComponentInChildren<SpriteRenderer>();

        // Set player tag on hurtbox
        p2.transform.Find("Hurtbox").gameObject.tag = "Player2";
    }
}

