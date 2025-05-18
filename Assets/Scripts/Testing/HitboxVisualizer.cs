using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class HitboxVisualizer : MonoBehaviour
{
    public AttackData attackToPreview;
    [Range(0, 50)] public int frameIndex;
    public SpriteRenderer spriteRenderer;

    private void OnValidate()
    {
        if(attackToPreview != null && spriteRenderer != null)
        {
            if(frameIndex >= 0 && frameIndex < attackToPreview.frames.Count)
            {
                spriteRenderer.sprite = attackToPreview.frames[frameIndex].frameSprite;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(attackToPreview == null || frameIndex < 0 || frameIndex >= attackToPreview.frames.Count)
        {
            return;
        }
        var frame = attackToPreview.frames[frameIndex];
        if (!frame.hasHitbox)
        {
            return;
        }
        Gizmos.color = Color.red;

        Vector3 worldpos = transform.position + (Vector3)frame.hitboxOffset;
        Gizmos.DrawWireCube(worldpos, frame.hitboxSize);
    }
}
