using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Windows;

public class AttackState : PlayerState
{
    private AttackInput attackInput;

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
        foreach(var frame in attack.frames)
        {
            player.sr.sprite = frame.frameSprite;
            yield return new WaitForSeconds(frame.frameDuration);
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
}

    
    