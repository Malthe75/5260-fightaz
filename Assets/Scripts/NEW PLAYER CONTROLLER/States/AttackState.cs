using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Windows;

public class AttackState : PlayerState
{
    private AttackInput attackInput;
    private int dashDirection;

    public AttackState(NewPlayerController player, AttackInput input) : base(player)
    {
        attackInput = input;
    }

    public override void Enter()
    {
        attack(attackInput);
    }

    private void attack(AttackInput attackInput)
    {

        foreach(var attack in player.attackData)
        {
            if(attackInput == attack.attackInput)
            {

                player.StartCoroutine(showFrames(attack));
            }
        }

    }

    private IEnumerator showFrames(AttackData attack)
    {
        foreach(var attackFrame in attack.frames)
        {
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

    private void HandleNextState()
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

    
    