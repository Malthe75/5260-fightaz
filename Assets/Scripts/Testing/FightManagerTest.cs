using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FightManagerTest : MonoBehaviour
{

    public static FightManagerTest Instance {get; private set; }
    // Needed for spawning prefab.
    public Transform player1Spawn;
    public Transform player2Spawn;
    public GameObject playerPrefab1;
    public GameObject playerPrefab2;
    public List<GameObject> stages;
    public PlayerInput p1;
    public PlayerInput p2;
    private NewPlayerController pc1;
    private NewPlayerController pc2;

    private AttackHitbox p1Hitbox;
    private AttackHitbox p2Hitbox;

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
        SpawnPlayers();
       
    }
    private void Update()
    {
        ClashMechanic();
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
        pc1 = p1.GetComponent<NewPlayerController>();
        pc2 = p2.GetComponent<NewPlayerController>();
        pc1.SetEnemy(pc2);
        pc2.SetEnemy(pc1);
        p1Hitbox = p1.GetComponentInChildren<AttackHitbox>();
        p2Hitbox = p2.GetComponentInChildren<AttackHitbox>();
    }

    private void ClashMechanic()
    {
        if(p1Hitbox.shouldClash && p2Hitbox.shouldClash)
        {
            SwitchAllActionMaps("Clash");
            p1Hitbox.shouldClash = false;
            p2Hitbox.shouldClash = false;
            p1Hitbox.cancelRoutine = true;
            p2Hitbox.cancelRoutine = true;
            pc1.SetClashState();
            pc2.SetClashState();
        }
    }

    public void SwitchAllActionMaps(string actionMap)
    {
        p1.SwitchCurrentActionMap(actionMap);
        p2.SwitchCurrentActionMap(actionMap);
    }

}

