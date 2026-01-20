using UnityEngine;

public class CharacterItem : MenuItemBase, ICancelable, IHoverable
{
    [Header("Action")]
    [SerializeField] CharacterDefinition character;


    public void Cancel()
    {
    }
    public override void Confirm(int playerIndex)
    {
        CharacterSelectManager.Instance.SelectCharacter(playerIndex, character);

    }

    public void Hover(int playerIndex)
    {
        CharacterSelectManager.Instance.HoverCharacter(playerIndex, character);
    }
}
