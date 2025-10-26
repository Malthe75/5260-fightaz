using System.Collections;
using UnityEngine;

public class AttackState : PlayerState
{
    private AttackInput attackInput;
    private int dashDirection;
    private Coroutine attackRoutine;

    public AttackState(NewPlayerController player, AttackInput input) : base(player)
    {
        attackInput = input;
    }

    public override void Enter()
    {
        attack(attackInput);
    }
    public override void Exit()
    {
        if(attackRoutine != null)
        {
            player.StopCoroutine(attackRoutine);
            attackRoutine = null;

            player.attackHitbox.Deactivate();
        }
    }

    private void attack(AttackInput attackInput)
    {

        foreach(var attack in player.attackData)
        {
            if(attackInput == attack.attackInput)
            {

                attackRoutine = player.StartCoroutine(showFrames(attack));
            }
        }

    }

    // Remember to delete coroutine if we exit the state early.
    private IEnumerator showFrames(AttackData attack)
    {
        foreach(var attackFrame in attack.frames)
        {
            Debug.Log("What?");
            // Sprite
            player.sr.sprite = attackFrame.frameSprite;

            // Activate hitbox if it exists
            if (attackFrame.hasHitbox)
            {
                player.attackHitbox.Activate(attackFrame);

                if (attackFrame.dashForce != 0)
                {
                    dash(attackFrame.dashForce);
                }

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

    
    