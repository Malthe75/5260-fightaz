using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Attack data")]
public class AttackData : ScriptableObject
{
    public string attackName;
    public List<AttackFrameData> frames;
}

