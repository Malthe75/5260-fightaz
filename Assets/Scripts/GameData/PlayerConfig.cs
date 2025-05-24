using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerConfig
{
    public string playerName;
    public GameObject characterPrefab;
    public InputDevice inputDevice;
    public string controlScheme;
}
