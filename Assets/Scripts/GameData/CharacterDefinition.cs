using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDefinition", menuName = "Game Data/Character Definition", order = 1)]
public class CharacterDefinition : ScriptableObject
{
    public string id;
    public GameObject characterPrefab;
    public Sprite portrait;
    public string displayName;
}