using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AttackState : PlayerState
{
    private Coroutine attackRoutine;
    private float xVelocity;
    private float yVelocity;

    public AttackState(NewPlayerController player, float xVelocity = 0, float yVelocity = 0) : base(player)
    {
        this.xVelocity = xVelocity;
        this.yVelocity = yVelocity;
    }

    public override void Enter()
    {
        player.StartCoroutine(ShowFrames(player.attack));
    }

    // Remember to delete coroutine if we exit the state early.
    private IEnumerator ShowFrames(AttackData attack)
    {
        for (int i = 0; i < attack.frames.Count; i++)
        {
            AttackFrameData attackFrame = attack.frames[i];
            // Sprite
            player.Animation.SetSprite(attackFrame.frameSprite);


            // Dash
            if (attackFrame.yDash != 0)
            {
                player.Movement.SetYDash(attackFrame.yDash, 0.2f);
            }
            else
            {
                player.Movement.SetMove(player.facing, attackFrame.xDash);
            }
            // else
            // {
            //     player.Movement.SetMove(player.facing, attackFrame.xDash);
            // }
            // Activate hitbox if it exists
            if (attackFrame.hasHitbox)
            {
                player.attackHitbox.Activate(attackFrame);
            }
            // Sound
            if (attackFrame.attackSound != null)
            {
                AudioManagerTwo.Instance.PlaySFX(attackFrame.attackSound);
            }
            // Timer
            yield return new WaitForSeconds(attackFrame.frameDuration);

            // Deactivate hitbox if it exists
            if (attackFrame.hasHitbox)
            {
                player.attackHitbox.Deactivate();
            }

        }
        HandleNextState();
    }

    public override void HandleNextState()
    {
        if (!player.Movement.IsGrounded())
        {
            player.stateMachine.ChangeState(new FallState(player));
            return;
        }
        if (player.moveInput != Vector2.zero)
        {
            player.stateMachine.ChangeState(new WalkState(player));
            return;
        }
        else
        {
            player.stateMachine.ChangeState(new IdleState(player));
            return;
        }
    }

    public override void Exit()
    {
        if (attackRoutine != null)
        {
            player.StopCoroutine(attackRoutine);
            attackRoutine = null;
            player.attackHitbox.Deactivate();
        }
        player.shouldAttack = false;
    }


}


