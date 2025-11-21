using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class HurtState : PlayerState
{
    float knockbackForce; // How strong the knockback is
    float hurtTimer = 0.4f; // How long the player stays in hurt state (total time in hurt state)

    public HurtState(NewPlayerController player, float knockbackForce) : base(player)
    {
        this.knockbackForce = knockbackForce;
    }

    public override void Enter()
    {
        // Set color to red, when hurt.
        player.sr.sprite = player.idleSprites[1];
        player.sr.color = Color.red;

        if (player.hurtSounds != null)
        {
            AudioManagerTwo.Instance.PlaySFX(player.hurtSounds[0]);
        }

    }

    public override void FixedUpdate()
    {
        if (hurtTimer > 0f)
        {
            hurtTimer -= Time.fixedDeltaTime;
        }
        else
        {
            player.stateMachine.ChangeState(new IdleState(player));
        }
    }
    public override void Exit()
    {
        player.sr.color = Color.white;
    }

}