using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(NewPlayerController player) : base(player) { }
    public override void Enter()
    {
        Debug.Log("Entered Idle State");
        player.sr.sprite = player.idleSprites[0];
    }

    public override void Exit()
    {  
        Debug.Log("Exited Idle State");
        // Cleanup if necessary
    }
    public override void Update()
    {
        //if(Mathf.Abs(player.moveInput.x) > 0.01)
        //{
        //    Debug.Log("Transitioning to Walk State from Idle State");
        //    player.stateMachine.ChangeState(new WalkState(player));
        //}
    }

    public override void OnMove(Vector2 input)
    {
        Debug.Log("On move idle");
        if (Mathf.Abs(input.x) > 0.01f)
        {
            player.moveInput = input;
            player.stateMachine.ChangeState(new WalkState(player));
        }
    }
}
