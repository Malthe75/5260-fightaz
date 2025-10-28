using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Combo")]
public class ComboData : ScriptableObject
{
    public List<AttackData> attacks;
}
