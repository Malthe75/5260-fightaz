using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/ComboLibrary")]
public class ComboLibrary : ScriptableObject
{
    public List<ComboData> combos;
}
