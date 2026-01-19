using UnityEngine;

public class CharacterItem : MenuItemBase, ICancelable
{
    [Header("Action")]
    [SerializeField] CharacterDefinition character;
    [SerializeField] private int playerIndex;


    public void Cancel()
    {
    }
    public override void Confirm( )
    {
        Debug.Log("Character " + character + " selected.");
        CharacterSelectManager.Instance.SelectCharacter(playerIndex, character);

    }
}
