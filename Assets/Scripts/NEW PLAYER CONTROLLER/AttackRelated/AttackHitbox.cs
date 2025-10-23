using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public BoxCollider2D hitboxCollider;
    private bool isActive = false;
    private int damage;
    AttackFrameData attack;

    private void Awake()
    {
        if (hitboxCollider)
        {
            hitboxCollider.enabled = false;
        }
    }

    public void Activate(AttackFrameData attack)
    {
        this.attack = attack;
        hitboxCollider.enabled = true;
        // AttackData should have damage?
        damage = 1;
        hitboxCollider.size = attack.hitboxSize;
        hitboxCollider.offset = attack.hitboxOffset;

        isActive = true;
    }

    public void Deactivate()
    {
        hitboxCollider.enabled = false;
        isActive = false;
    }

    private void OnTriggerEnter2D(Collider2D otherPlayer)
    {
        if (!isActive) return;

        // Check if we hit another player's hurtbox
        Hurtbox hurtbox = otherPlayer.GetComponent<Hurtbox>();
        if(hurtbox != null)
        {
            hurtbox.TakeDamage(damage, attack);
            // Optional: prevent multiple hits per attack
            Deactivate();
        }
    }

}


