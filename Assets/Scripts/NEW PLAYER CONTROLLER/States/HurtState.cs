using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class HurtState : PlayerState
{
    float knockbackForce; // How strong the knockback is
    float knockupForce; // How strong the knockup is
    float hurtTimer = 0.5f; // How long the player stays in hurt state (total time in hurt state)

    public HurtState(NewPlayerController player, float knockbackForce, float knockupForce) : base(player)
    {
        this.knockbackForce = knockbackForce;
        this.knockupForce = knockupForce;
    }

    public override void Enter()
    {
        // Set color to red, when hurt.

        player.Animation.SetSprite(player.idleSprites[1]);
        player.Animation.SetColor(Color.red);
        player.Movement.SetKnockup(knockbackForce, knockupForce, player.facing);

        if (player.hurtSounds != null)
        {
            AudioManagerTwo.Instance.PlaySFX(player.hurtSounds[0]);
        }

    }

    public override void Update()
    {
         if (hurtTimer > 0f)
        {
            hurtTimer -= Time.deltaTime;
        }
        else
        {
            if(player.Movement.IsGrounded()){
                player.stateMachine.ChangeState(new IdleState(player));
            }
        }
    }
    public override void Exit()
    {
        player.Animation.SetColor(Color.white);
    }

}