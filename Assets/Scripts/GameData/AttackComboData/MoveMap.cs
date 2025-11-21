using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public enum MoveInput
{
    Nothing,
    Hit,
    Shoot,
    Kick,
    Hit_Run_Forward,
    Shoot_Run_Forward,
    Kick_Run_Forward,
    Hit_Run_Backward,
    Shoot_Run_Backward,
    Kick_Run_Backward,
    Hit_Jump,
    Shoot_Jump,
    Kick_Jump,
    Hit_Crouch,
    Shoot_Crouch,
    Kick_Crouch,
}


[Serializable]
public class MoveEntry
{
    [HideInInspector] public MoveInput input; // fixed, not editable
    public AttackData attack;                 // assign this in inspector
}

[CreateAssetMenu(menuName = "Combat/MoveMap")]
public class MoveMap : ScriptableObject
{
    [SerializeField]
    private List<MoveEntry> moves = new List<MoveEntry>();

    private Dictionary<MoveInput, AttackData> moveDict;

    private void OnValidate()
    {
        // Automatically populate the list with every enum value (without duplicates)
        var existingInputs = new HashSet<MoveInput>();
        foreach (var m in moves)
            existingInputs.Add(m.input);

        foreach (MoveInput input in Enum.GetValues(typeof(MoveInput)))
        {
            if (!existingInputs.Contains(input))
                moves.Add(new MoveEntry { input = input });
        }

        // Sort by enum order for cleaner inspector
        moves.Sort((a, b) => a.input.CompareTo(b.input));
    }

    public void InitializeDict()
    {
        moveDict = new Dictionary<MoveInput, AttackData>();
        foreach (var move in moves)
            moveDict[move.input] = move.attack;
    }

    public AttackData GetAttack(MoveInput input)
    {
        if (moveDict == null)
            InitializeDict();

        moveDict.TryGetValue(input, out var attack);
        return attack;
    }
}

[CustomPropertyDrawer(typeof(MoveEntry))]
public class MoveEntryDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var inputProp = property.FindPropertyRelative("input");
        var attackProp = property.FindPropertyRelative("attack");

        // Show enum name as label, AttackData field next to it
        EditorGUI.BeginProperty(position, label, property);
        Rect labelRect = new Rect(position.x, position.y, 180, position.height);
        Rect fieldRect = new Rect(position.x + 185, position.y, position.width - 185, position.height);

        EditorGUI.LabelField(labelRect, inputProp.enumDisplayNames[inputProp.enumValueIndex]);
        EditorGUI.PropertyField(fieldRect, attackProp, GUIContent.none);
        EditorGUI.EndProperty();
    }
}