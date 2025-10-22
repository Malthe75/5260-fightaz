using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitboxes : MonoBehaviour
{
    private BoxCollider2D col;
    private float timer;
    private bool active;
    private PlayerController player;


    // MIGHT NEED SOMETHING TO DO WITH OWNER OF HITBOX TO AVOID SELF HITS? AWHAT?


    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        player = GetComponentInParent<PlayerController>();
        col.enabled = false;
    }

    public void EnableFor(float duration)
    {
        active = true;
        timer = duration;
        col.enabled = true;
    }

    private void Update()
    {
        if (active)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                active = false;
                col.enabled = false;
            }
        }
    }

    //void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if(active && other.CompareTag("PlayerCollision"))
    //    {
    //        owner.OnHitLanded(other)
    //        PlayerController otherPlayer = collision.GetComponentInParent<PlayerController>();
    //        if(otherPlayer != player)
    //        {
    //            otherPlayer.TakeDamage(player.attackDamage);
    //        }
    //    }
    //}
}


