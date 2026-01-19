using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUIManager : MonoBehaviour
{
    [SerializeField] private List<Image> fighterPortraits;
    [SerializeField] private List<TextMeshProUGUI> names;

    private void Start()
    {
        if (CharacterSelectManager.Instance != null)
        {
            CharacterSelectManager.Instance.OnCharacterSelected += UpdateSelected;
            CharacterSelectManager.Instance.OnCharacterDeselected += ClearPortrait;
        }
        else
        {
            Debug.LogError("CharacterSelectUIManager: CharacterSelectManager instance not found!");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe safely to avoid memory leaks or dangling events
        if (CharacterSelectManager.Instance != null)
        {
            CharacterSelectManager.Instance.OnCharacterSelected -= UpdateSelected;
            CharacterSelectManager.Instance.OnCharacterDeselected -= ClearPortrait;
        }
    }

    private void UpdateSelected(int playerIndex, CharacterDefinition character)
    {
        Debug.Log($"UIManager: Updating portrait for Player {playerIndex} to {character.displayName}");

        if (playerIndex < 0 || playerIndex >= fighterPortraits.Count)
        {
            Debug.LogWarning("UIManager: Invalid player index.");
            return;
        }

        fighterPortraits[playerIndex].sprite = character.portrait; // Assumes CharacterDefinition has a portrait
        fighterPortraits[playerIndex].enabled = true;
        names[playerIndex].text = character.displayName;
    }


    private void ClearPortrait(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= fighterPortraits.Count)
        {
            Debug.LogWarning("UIManager: Invalid player index.");
            return;
        }

        fighterPortraits[playerIndex].sprite = null;
        fighterPortraits[playerIndex].enabled = false;
        names[playerIndex].text = "";
    }


}
