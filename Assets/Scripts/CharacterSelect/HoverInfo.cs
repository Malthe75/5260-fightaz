using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class HoverInfo : MonoBehaviour
{
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private int playerIndex;


    private void Start()
    {
        CharacterSelectManager.Instance.OnCharacterHovered += UpdateCharacterInfo;
    }

    private void OnDestroy()
    {
        if (CharacterSelectManager.Instance == null) return;

        CharacterSelectManager.Instance.OnCharacterHovered -= UpdateCharacterInfo;

    }

    private void UpdateCharacterInfo(int playerIndex, CharacterDefinition character)
    {
        if (playerIndex != this.playerIndex) return;
        if (character == null)
        {
            characterImage.sprite = null;
            characterImage.enabled = false;
            characterName.text = "";
            return;
        }

        characterImage.sprite = character.portrait; // Assumes CharacterDefinition has a portrait
        characterImage.enabled = true;
        characterName.text = character.displayName;
    }


}
