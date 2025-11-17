using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FightManager : MonoBehaviour
{
    public Transform player1Spawn;
    public Transform player2Spawn;

    public List<GameObject> stages;
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
        Debug.Log("Lets print out the players!");
        Debug.Log(players[0].characterPrefab.name);
        Debug.Log(players[0].playerName);
        Debug.Log(players[0].inputDevice);

        Debug.Log(players[1].characterPrefab.name);
        Debug.Log(players[1].playerName);
        Debug.Log(players[1].inputDevice);
        var p1 = PlayerInput.Instantiate(players[0].characterPrefab, controlScheme: players[0].controlScheme, pairWithDevice: players[0].inputDevice);
        p1.transform.position = player1Spawn.position;
        Vector3 newScale = p1.transform.localScale;
        newScale.x = -1f;
        p1.transform.localScale = newScale;

        p1.gameObject.name = "Player1";
        p1.tag = "Player1";
        SetupPlayerHitboxes(p1, "P1");


        var p2 = PlayerInput.Instantiate(players[1].characterPrefab, controlScheme: players[1].controlScheme, pairWithDevice: players[1].inputDevice);
        p2.transform.position = player2Spawn.position;
        Vector3 newScale2 = p2.transform.localScale;
        newScale2.x = 1f;
        p2.transform.localScale = newScale2;

        p2.gameObject.name = "Player2";
        p2.tag = "Player2";
        SetupPlayerHitboxes(p2, "P2");

        GameObject stage;
        stage = Instantiate(stages[StageData.selectedStageIndex]);

    }
    void SetupPlayerHitboxes(PlayerInput playerRoot, string playerPrefix)
    {
        // Assign attack hitbox tag
        Transform attackHitbox = playerRoot.transform.Find("Hitbox");
        if (attackHitbox != null)
        {
            attackHitbox.gameObject.tag = playerPrefix;
        }

        // Assign body hitboxes tags (head, torso, legs)
        Transform hitboxes = playerRoot.transform.Find("Hitboxes");
        if (hitboxes != null)
        {
            foreach (string part in new[] { "Upper", "Lower" })
            {
                Transform partTransform = hitboxes.Find(part);
                if (partTransform != null)
                {
                    partTransform.gameObject.tag = playerPrefix + part.Substring(0, 1).ToUpper() + part.Substring(1);
                    // e.g. "Player1Head"
                }
            }
        }
    }
}

