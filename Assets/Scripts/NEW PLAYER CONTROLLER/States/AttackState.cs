using System.Collections;
using UnityEngine;

public class AttackState : PlayerState
{
    private int dashDirection;
    private Coroutine attackRoutine;

    public AttackState(NewPlayerController player) : base(player)
    {
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

        //else if(x > 0)
        //{
        //    attackRoutine = player.StartCoroutine(showFrames(player.moveMap.GetAttack(MoveInput.Hit_RunForward)));
        //}
        //else if (x < 0)
        //{
        //    attackRoutine = player.StartCoroutine(showFrames(player.moveMap.GetAttack(MoveInput.Hit_RunBackward)));
        //}

    }

    // Remember to delete coroutine if we exit the state early.
    private IEnumerator showFrames(AttackData attack)
    {
        foreach(var attackFrame in attack.frames)
        {
            Debug.Log("What?");
            // Sprite
            player.sr.sprite = attackFrame.frameSprite;

            if (attackFrame.dashForce != 0)
                {
                    dash(attackFrame.dashForce);
                }
            // Activate hitbox if it exists
            if (attackFrame.hasHitbox)
            {
                player.attackHitbox.Activate(attackFrame);

                

            }

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
        if(player.moveInput != Vector2.zero)
        {
            player.stateMachine.ChangeState(new WalkState(player));
        }
        else
        {
            player.stateMachine.ChangeState(new IdleState(player));
        }
    }

    private void dash(float dashForce)
    {
        if (player.tag == "Player1") dashDirection = 1;
        else dashDirection = -1;
        player.rb.AddForce(new Vector2(dashDirection * dashForce, 0f), ForceMode2D.Impulse);
    }

}

    
    