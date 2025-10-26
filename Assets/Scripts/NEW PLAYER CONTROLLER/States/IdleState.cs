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
        Debug.Log("Entering it");
        Debug.Log(player.name);
    }

    public override void Update()
    {
        if (player.input != AttackInput.Nothing)
        {
            player.stateMachine.ChangeState(new AttackState(player, player.input));
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

        if(player.jumpInput == JumpInput.Right)
        {
            Debug.Log("before sttae");
            player.stateMachine.ChangeState(new JumpState(player));
        }

    }

    //public override void OnMove(Vector2 input)
    //{
    //    if (Mathf.Abs(input.x) > 0.01f)
    //    {
    //        player.stateMachine.ChangeState(new WalkState(player));
    //        player.moveInput = input;
    //    }
    //}
}
