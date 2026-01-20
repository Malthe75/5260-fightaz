using System.Diagnostics;
using UnityEngine;

public class HurtState : PlayerState
{
    float hurtTimer = 0.5f; // How long the player stays in hurt state (total time in hurt state)
    int juggleCounter = 0;
    AttackFrameData attack;
    public HurtState(NewPlayerController player, AttackFrameData attack) : base(player)
    {
        this.attack = attack;
    }

    public override void Enter()
    {
        // Set color to red, when hurt.
        player.Animation.SetColor(Color.red);
        
        if(attack.yKnockup > 0){
            Knockup();
        } else {
            Knockback();
        }

        if (player.hurtSounds != null)
        {
            AudioManager.Instance.PlaySFX(player.hurtSounds[0]);
        }

    }

    private void Knockup()
    {
        player.isKnockup = true;
        float totalAnimationTime = player.Movement.CalculateAirTime(attack.yKnockup, 1f);
        player.Animation.SetKnockupAnimation(totalAnimationTime);
        player.Movement.SetKnockup(attack.xKnockback, attack.yKnockup, player.facing);
    }

    private void Knockback()
    {
        player.Animation.SetSprite(player.idleSprites[0]);
        player.Movement.SetMove(-player.facing, attack.xKnockback);
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
        player.juggleCount = 0;
        player.isKnockup = false;
    }

}