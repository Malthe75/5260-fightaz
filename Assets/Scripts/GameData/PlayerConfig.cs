using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerConfig
{
    public string playerName;
    public CharacterDefinition characterDefinition;
    public InputDevice inputDevice;
    public string controlScheme;
}
