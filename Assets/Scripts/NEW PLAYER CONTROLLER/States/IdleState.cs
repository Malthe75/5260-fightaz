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

    public override void OnMove(Vector2 input)
    {
        if (Mathf.Abs(input.x) > 0.01f)
        {
            player.stateMachine.ChangeState(new WalkState(player));
            player.moveInput = input;
        }
    }
}
