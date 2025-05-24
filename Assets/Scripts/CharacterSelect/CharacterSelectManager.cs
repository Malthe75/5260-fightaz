using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("Selection UI")]
    public List<Transform> characterIcons;
    public RectTransform p1Selector;
    public RectTransform p2Selector;

    [Header("Top fighter image")]
    public Image p1Image;
    public Image p2Image;
    public Sprite[] images;

    [Header("Character Prefabs")]
    public List<GameObject> characterPrefabs;



    private int p1Index = 0, p2Index = 0;
    private bool p1Locked = false, p2Locked = false;

    private CharacterSelectInputHandler p1Input, p2Input;

    public void OnPlayerJoined(PlayerInput input)
    {
        DontDestroyOnLoad(input.gameObject);

        var handler = input.GetComponent<CharacterSelectInputHandler>();
        Debug.Log("Player joined!");

        Debug.Log(input.playerIndex);
        if (input.playerIndex == 0)
        {
            p1Input = handler;
            p1Input.OnMove += HandleP1Move;
            p1Input.OnConfirm += HandleP1Confirm;
        }
        else if (input.playerIndex == 1)
        {
            p2Input = handler;
            p2Input.OnMove += HandleP2Move;
            p2Input.OnConfirm += HandleP2Confirm;
        }
    }

    private bool p1MoveReleased = true;

    private void HandleP1Move(Vector2 input)
    {
        if (!p1Locked)
        {
            if (Mathf.Abs(input.x) < 0.2f)
            {
                p1MoveReleased = true;
            }

            if (Mathf.Abs(input.x) > 0.5f && p1MoveReleased)
            {
                ChangeIndex(ref p1Index, input.x > 0 ? 1 : -1);
                p1Image.sprite = images[p1Index];

                p1MoveReleased = false;
            }
        }
    }

    private void HandleP1Confirm()
    {
        if (!p1Locked)
        {
            GameObject selectedCharacterPrefab = characterPrefabs[p1Index];
            //CharacterData.p1CharacterIndex = p1Index;
            var config = new PlayerConfig
            {
                playerName = "Player 1",
                characterPrefab = selectedCharacterPrefab,
                inputDevice = Keyboard.current,
                controlScheme = "Keyboard"
            };
            PlayerManager.Instance.AddPlayer(config);
            p1Locked = true;
            Debug.Log("P1 locked in: " + characterIcons[p1Index].name);
        }
    }

    private bool p2MoveReleased = true;

    private void HandleP2Move(Vector2 input)
    {
        if (!p2Locked)
        {
            if (Mathf.Abs(input.x) < 0.2f)
            {
                p2MoveReleased = true;
            }

            if (Mathf.Abs(input.x) > 0.5f && p2MoveReleased)
            {
                ChangeIndex(ref p2Index, input.x > 0 ? 1 : -1);
                p2Image.sprite = images[p2Index];

                p2MoveReleased = false;
            }
        }
    }
    private void HandleP2Confirm()
    {
        if (!p2Locked)
        {
            //CharacterData.p2CharacterIndex = p2Index;
            GameObject selectedCharacterPrefab = characterPrefabs[p2Index];
            var config = new PlayerConfig
            {
                playerName = "Player 2",
                characterPrefab = selectedCharacterPrefab,
                inputDevice = Gamepad.current,
                controlScheme = "Gamepad"
            };
            PlayerManager.Instance.AddPlayer(config);
            p2Locked = true;
            Debug.Log("P2 locked in: " + characterIcons[p2Index].name);
        }
    }

    private void Update()
    {
        p1Selector.position = characterIcons[p1Index].position;
        p2Selector.position = characterIcons[p2Index].position;

        if (p1Locked && p2Locked)
        {
            SceneManager.LoadScene("StageSelectScene");
        }
    }

    private void ChangeIndex(ref int index, int direction)
    {
        int count = characterIcons.Count;
        index = (index + direction + count) % count;
    }
}

