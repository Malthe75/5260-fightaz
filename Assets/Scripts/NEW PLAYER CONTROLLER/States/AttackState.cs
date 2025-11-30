using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AttackState : PlayerState
{
    private Coroutine attackRoutine;
    private float dashForce = 0f;
    private int currentIndex;
    private bool isJumpAttack;

    public AttackState(NewPlayerController player, int currentIndex = 0, bool isJumpAttack = false) : base(player)
    {
        this.currentIndex = currentIndex;
        this.isJumpAttack = isJumpAttack;
    }

    public override void Enter()
    {
        attack();
    }
    public override void Exit()
    {
        if(attackRoutine != null)
        {
            player.StopCoroutine(attackRoutine);
            attackRoutine = null;
            player.attackHitbox.Deactivate();
        }
        player.shouldAttack = false;
    }

    private void attack()
    {
        player.StartCoroutine(showFrames(player.attack));
    }

    // Remember to delete coroutine if we exit the state early.
    private IEnumerator showFrames(AttackData attack)
    {
        for(int i = currentIndex; i < attack.frames.Count; i++)
        {
            AttackFrameData attackFrame = attack.frames[i];
            // Sprite
            player.Animation.SetSprite(attackFrame.frameSprite);

            // Dash if there is dash, and it isnt a jump attack.
            if(!isJumpAttack) player.Movement.SetMove(player.facing, attackFrame.dashForce);
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
            // dashForce = 0f;
            // player.Movement.SetMove(player.facing, dashForce);

        }
        HandleNextState();
    }

    public override void HandleNextState()
    {
        if(player.moveInput != Vector2.zero)
        {
            player.stateMachine.ChangeState(new WalkState(player));
        }
        else
        {
            player.stateMachine.ChangeState(new IdleState(player));
        }
    }


}

    
    