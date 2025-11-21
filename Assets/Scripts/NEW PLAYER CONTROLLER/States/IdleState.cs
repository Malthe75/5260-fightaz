using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(NewPlayerController player) : base(player) { }
    public override void Enter()
    {
        player.sr.sprite = player.idleSprites[0];
    }

    public override void Update()
    {
        if (player.shouldAttack)
        {
            Debug.Log("Should attack");
            player.stateMachine.ChangeState(new AttackState(player));
        }

        if (Mathf.Abs(player.moveInput.x) > 0.01f)
        {
            player.stateMachine.ChangeState(new WalkState(player));
        }

        if (player.isBlocking)
        {
            player.stateMachine.ChangeState(new BlockState(player));
        }

        if(player.shouldJump)
        {
            player.stateMachine.ChangeState(new JumpState(player, JumpInput.Up));
            return;
        }

    }
}
