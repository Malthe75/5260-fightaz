using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{


    public static CharacterSelectManager Instance { get; private set; }
    private Dictionary<int, CharacterDefinition> playerSelections = new();
    public event System.Action<int, CharacterDefinition> OnCharacterSelected;
    public event System.Action<int> OnCharacterDeselected;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SelectCharacter(int playerIndex, CharacterDefinition character)
    {
        if(character == null)
        {
            Debug.LogError("CharacterSelectManager: Attempted to select a null character.");
            return;
        }

        playerSelections[playerIndex] = character;
        Debug.Log($"Player {playerIndex} selected character: {character.displayName}");
        OnCharacterSelected?.Invoke(playerIndex, character);
    }

    public void DeselectCharacter(int playerIndex)
    {
        if (playerSelections.ContainsKey(playerIndex))
        {
            Debug.Log($"Player {playerIndex} deselected character:");
            playerSelections.Remove(playerIndex);
        }
        OnCharacterDeselected?.Invoke(playerIndex);
    }

    public CharacterDefinition GetSelection(int playerIndex)
    {
        playerSelections.TryGetValue(playerIndex, out var character);
        return character;
    }

    public Dictionary<int, CharacterDefinition> GetAllSelections()
    {
        return new Dictionary<int, CharacterDefinition>(playerSelections);
    }


    // [Header("Top fighter details")]
    // public Image p1Image;
    // public Image p2Image;
    // public Sprite[] images;
    // public TextMeshProUGUI p1NameText;
    // public TextMeshProUGUI p2NameText;
    // public List<AudioClip> characterSelectSounds;
    // private AudioSource audioSource;
    // private AudioSource audioSource2;
    // private string[] characterNames = { "GANNI", "BIG MAC", "GREENLANDER", "MOTOR", "ABDALLAH", "RANDOM" };


    // [Header("Character Prefabs")]
    // public List<GameObject> characterPrefabs;



    // private int p1Index = 0, p2Index = 0;
    // private bool p1Locked = false, p2Locked = false;

    // private CharacterSelectInputHandler p1Input, p2Input;


    // void Start()
    // {
    //     // Add an AudioSource component if not already attached
    //     audioSource = gameObject.AddComponent<AudioSource>();
    //     audioSource2 = gameObject.AddComponent<AudioSource>();
    // }

    // public void OnPlayerJoined(PlayerInput input)
    // {
    //     DontDestroyOnLoad(input.gameObject);

    //     var handler = input.GetComponent<CharacterSelectInputHandler>();
    //     Debug.Log("Player joined!");

    //     Debug.Log(input.playerIndex);
    //     if (input.playerIndex == 0)
    //     {
    //         p1Input = handler;
    //         p1Input.OnMove += HandleP1Move;
    //         p1Input.OnConfirm += HandleP1Confirm;
    //     }
    //     else if (input.playerIndex == 1)
    //     {
    //         p2Input = handler;
    //         p2Input.OnMove += HandleP2Move;
    //         p2Input.OnConfirm += HandleP2Confirm;
    //     }
    // }

    // private bool p1MoveReleased = true;

    // private void HandleP1Move(Vector2 input)
    // {
    //     if (!p1Locked)
    //     {
    //         if (Mathf.Abs(input.x) < 0.2f)
    //         {
    //             p1MoveReleased = true;
    //         }

    //         if (Mathf.Abs(input.x) > 0.5f && p1MoveReleased)
    //         {
    //             ChangeIndex(ref p1Index, input.x > 0 ? 1 : -1);
    //             p1Image.sprite = images[p1Index];
    //             p1NameText.text = characterNames[p1Index];
    //             audioSource.clip = characterSelectSounds[p1Index];
    //             audioSource.Play();


    //             p1MoveReleased = false;
    //         }
    //     }
    // }

    // private void HandleP1Confirm()
    // {
    //     if (!p1Locked)
    //     {
    //         GameObject selectedCharacterPrefab = characterPrefabs[p1Index];

    //         InputDevice inputDevice = null;
    //         string controlScheme = "";

    //         // Prefer unused gamepad if available
    //         foreach (var gamepad in Gamepad.all)
    //         {
    //             if (!PlayerManager.Instance.IsDeviceUsed(gamepad))
    //             {
    //                 inputDevice = gamepad;
    //                 controlScheme = "Gamepad";
    //                 break;
    //             }
    //         }

    //         // Fallback to keyboard if no unused gamepad
    //         if (inputDevice == null && Keyboard.current != null && !PlayerManager.Instance.IsDeviceUsed(Keyboard.current))
    //         {
    //             inputDevice = Keyboard.current;
    //             controlScheme = "Keyboard";
    //         }

    //         if (inputDevice == null)
    //         {
    //             Debug.LogError("No available input device for Player 1.");
    //             return;
    //         }

    //         var config = new PlayerConfig
    //         {
    //             playerName = "Player 1",
    //             characterPrefab = selectedCharacterPrefab,
    //             inputDevice = inputDevice,
    //             controlScheme = controlScheme
    //         };

    //         PlayerManager.Instance.AddPlayer(config);
    //         p1Locked = true;
    //         Debug.Log("P1 locked in: " + characterIcons[p1Index].name + " using " + controlScheme);
    //     }
    // }


    // private bool p2MoveReleased = true;

    // private void HandleP2Move(Vector2 input)
    // {
    //     if (!p2Locked)
    //     {
    //         if (Mathf.Abs(input.x) < 0.2f)
    //         {
    //             p2MoveReleased = true;
    //         }

    //         if (Mathf.Abs(input.x) > 0.5f && p2MoveReleased)
    //         {
    //             ChangeIndex(ref p2Index, input.x > 0 ? 1 : -1);
    //             p2Image.sprite = images[p2Index];
    //             p2NameText.text = characterNames[p2Index];
    //             audioSource2.clip = characterSelectSounds[p2Index];
    //             audioSource2.Play();
    //             p2MoveReleased = false;
    //         }
    //     }
    // }
    // private void HandleP2Confirm()
    // {
    //     if (!p2Locked)
    //     {
    //         GameObject selectedCharacterPrefab = characterPrefabs[p2Index];

    //         InputDevice inputDevice = null;
    //         string controlScheme = "";

    //         // Prefer unused gamepad if available
    //         foreach (var gamepad in Gamepad.all)
    //         {
    //             if (!PlayerManager.Instance.IsDeviceUsed(gamepad))
    //             {
    //                 inputDevice = gamepad;
    //                 controlScheme = "Gamepad";
    //                 break;
    //             }
    //         }

    //         // Fallback to keyboard if no unused gamepad
    //         if (inputDevice == null && Keyboard.current != null && !PlayerManager.Instance.IsDeviceUsed(Keyboard.current))
    //         {
    //             inputDevice = Keyboard.current;
    //             controlScheme = "Keyboard";
    //         }

    //         if (inputDevice == null)
    //         {
    //             Debug.LogError("No available input device for Player 2.");
    //             return;
    //         }

    //         var config = new PlayerConfig
    //         {
    //             playerName = "Player 2",
    //             characterPrefab = selectedCharacterPrefab,
    //             inputDevice = inputDevice,
    //             controlScheme = controlScheme
    //         };

    //         PlayerManager.Instance.AddPlayer(config);
    //         p2Locked = true;
    //         Debug.Log("P2 locked in: " + characterIcons[p2Index].name + " using " + controlScheme);
    //     }
    // }


    // private void Update()
    // {
    //     p1Selector.position = characterIcons[p1Index].position;
    //     p2Selector.position = characterIcons[p2Index].position;

    //     if (p1Locked && p2Locked)
    //     {
    //         SceneManager.LoadScene("StageSelectScene");
    //     }
    // }

    // private void ChangeIndex(ref int index, int direction)
    // {
    //     int count = characterIcons.Count;
    //     index = (index + direction + count) % count;
    // }
}

