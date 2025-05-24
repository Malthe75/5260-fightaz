using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FightManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform player1Spawn;
    public Transform player2Spawn;

    private void Start()
    {
        // Clear any previous players that may have been carried from another scene
        foreach (var input in FindObjectsOfType<PlayerInput>())
        {
            Destroy(input.gameObject);
        }

        // Spawn both players
        //var p1 = PlayerInput.Instantiate(playerPrefab, controlScheme: "Keyboard", pairWithDevice: Keyboard.current);
        //p1.transform.position = player1Spawn.position;

        //var p2 = PlayerInput.Instantiate(playerPrefab, controlScheme: "Gamepad", pairWithDevice: Gamepad.current);
        //p2.transform.position = player2Spawn.position;

        var players = PlayerManager.Instance.players;
        var p1 = PlayerInput.Instantiate(players[0].characterPrefab, controlScheme: players[0].controlScheme, pairWithDevice: players[0].inputDevice);
        p1.transform.position = player1Spawn.position;

        var p2 = PlayerInput.Instantiate(players[1].characterPrefab, controlScheme: players[1].controlScheme, pairWithDevice: players[1].inputDevice);
        p2.transform.position = player2Spawn.position;


    }
}

