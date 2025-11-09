using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class HurtState : PlayerState
{
    float knockbackForce; // How strong the knockback is
    //float hurtTimer = 0.1f; // How long the player stays in hurt state (total time in hurt state)
    float knockbackTimer = 0.8f; // Timer for the knockback duration
    float totalKnockbackDuration = 0.2f; // Total duration of the knockback effect
    Vector2 knockbackDirection; // Direction of knockback (to the right or left)

    AttackFrameData attack;
    public HurtState(NewPlayerController player, AttackFrameData attack) : base(player)
    {
        this.attack = attack;
        //knockbackTimer = hurtTimer;
    }
    // hurtTImer set to high, to test for now.

    public override void Enter()
    {
        knockbackForce = attack.knockback;
        if (player.tag == "Player1") knockbackDirection = Vector2.left;
        else knockbackDirection = Vector2.right;
        // Set color to red, when hurt.
        player.sr.sprite = player.idleSprites[1];
        player.sr.color = Color.red;

        if (player.hurtSounds != null)
        {
            AudioManagerTwo.Instance.PlaySFX(player.hurtSounds[0]);
        }

    }

    public override void Update()
    {
        if (knockbackTimer > 0f)
        {
            float t = knockbackTimer / totalKnockbackDuration; // normalized 0–1
            float currentForce = Mathf.Lerp(0f, knockbackForce, t);
            Vector2 knockbackMove = knockbackDirection * currentForce * Time.fixedDeltaTime;

            knockbackTimer -= Time.fixedDeltaTime;
        }
        else
        {

            player.stateMachine.ChangeState(new IdleState(player));
        }
    }
    public override void Exit()
    {
        // white should be defautlt i think.
        player.sr.color = Color.white;
    }

}