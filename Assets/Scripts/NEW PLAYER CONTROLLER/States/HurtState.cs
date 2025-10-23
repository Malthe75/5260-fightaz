using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtState : PlayerState
{
    float knockbackForce; // How strong the knockback is
    float hurtTimer = 0.5f; // How long the player stays in hurt state (total time in hurt state)
    float knockbackTimer; // Timer for the knockback duration
    Vector2 knockbackDirection; // Direction of knockback (to the right or left)

    AttackFrameData attack;
    public HurtState(NewPlayerController player, AttackFrameData attack) : base(player)
    {
        this.attack = attack;
        knockbackTimer = hurtTimer;
    }
    // hurtTImer set to high, to test for now.

    public override void Enter()
    {
        knockbackForce = attack.knockback;
        Debug.Log("WHat?");
        Debug.Log(knockbackForce);
        if (player.tag == "Player1") knockbackDirection = Vector2.left;
        else knockbackDirection = Vector2.right;
        // Set color to red, when hurt.
        player.sr.sprite = player.idleSprites[1];
        player.sr.color = Color.red;

    }
    public override void Exit()
    {
        // white should be defautlt i think.
        player.sr.color = Color.white;
    }

    public override void Update()
    {

        if (knockbackTimer > 0)
        {
            // Move player based on knockback force
            player.transform.position = Vector2.MoveTowards(player.transform.position,
                (Vector2)player.transform.position + knockbackDirection, knockbackForce * Time.deltaTime);

            // Decrease the knockback timer
            knockbackTimer -= Time.deltaTime;
        }

        // After knockback ends, transition to the idle state
        if (knockbackTimer <= 0)
        {
            hurtTimer -= Time.deltaTime;  // Handle the remaining hurt state time
            if (hurtTimer <= 0)
            {
                player.stateMachine.ChangeState(new IdleState(player)); // Transition back to idle
            }
        }
    }
}