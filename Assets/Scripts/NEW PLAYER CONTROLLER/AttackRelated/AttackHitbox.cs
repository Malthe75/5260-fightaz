using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public BoxCollider2D hitboxCollider;
    private bool isActive = false;
    private int damage;
    AttackFrameData attack;
    private float clashTimer = 0.01f;
    public bool shouldClash = false;
    public Coroutine clashCoroutine;
    public bool cancelRoutine = false;

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
        if (hurtbox != null)
        {
            clashCoroutine = StartCoroutine(ChooseEvent(otherPlayer, hurtbox));
            Deactivate();

        }
    }

    public IEnumerator ChooseEvent(Collider2D otherPlayer, Hurtbox hurtbox)
    { 
        cancelRoutine = false;
        // This timer is for clash detection
        shouldClash = true;
        yield return new WaitForSeconds(clashTimer);
        shouldClash = false;
        if (cancelRoutine) yield break;
        
        NewPlayerController player = otherPlayer.GetComponentInParent<NewPlayerController>();
        if( player.isBlocking)
        {
            Debug.Log("Blocked Attack!");
        }
        else 
        {
            hurtbox.TakeDamage(damage, attack);
            HitFeedback.Instance.TriggerHitEffect();
            // Optional: prevent multiple hits per attack
        }
    }

}


