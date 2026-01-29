using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{


    public static CharacterSelectManager Instance { get; private set; }
    private Dictionary<int, CharacterDefinition> playerSelections = new();
    public event System.Action<int, CharacterDefinition> OnCharacterSelected;
    public event System.Action<int, CharacterDefinition> OnCharacterHovered;

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
        PlayerManager.Instance.AddPlayer(new PlayerConfig
        {
            playerIndex = playerIndex,
            playerName = $"Player {playerIndex + 1}",
            characterDefinition = character
        });

        if(playerSelections.Count == 2)
        {
            GameManager.Instance.LoadScene("StageSelectScene");
        }
    }

    public void HoverCharacter(int playerIndex, CharacterDefinition character)
    {
        OnCharacterHovered?.Invoke(playerIndex, character);
    }

    public void DeselectCharacter(int playerIndex)
    {
        if (playerSelections.ContainsKey(playerIndex))
        {
            Debug.Log($"Player {playerIndex} deselected character:");
            playerSelections.Remove(playerIndex);
            PlayerManager.Instance.RemovePlayer(playerIndex);
        }
        OnCharacterDeselected?.Invoke(playerIndex);
    }
}

