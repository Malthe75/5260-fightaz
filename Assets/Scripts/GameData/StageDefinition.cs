using UnityEngine;

[CreateAssetMenu(fileName = "StageDefinition", menuName = "Game Data/Stage Definition", order = 1)]
public class StageDefinition : ScriptableObject
{
    public Sprite stageImage;
    public string stageName;
}