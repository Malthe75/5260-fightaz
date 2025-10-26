using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackFrameData
{
    public Sprite frameSprite;
    public Vector2 hitboxSize;
    public Vector2 hitboxOffset;
    public bool hasHitbox;
    public GameObject hitboxPrefab;
    public AudioClip attackSound;
    public float frameDuration = 0.2f; // Time before moving to next frame
    public float knockback = 3;
    public float dashForce = 0;
}


