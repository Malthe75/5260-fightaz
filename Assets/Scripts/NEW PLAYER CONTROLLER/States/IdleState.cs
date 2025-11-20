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
        if (player.input != MoveInput.Nothing)
        {
            player.stateMachine.ChangeState(new AttackState(player, player.input, 0));
            return;
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
