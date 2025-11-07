using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum JumpInput
{
    Nothing,
    Up,
    Right,
    Left
}
public class JumpState : PlayerState
{

    private Vector2 jumpDirection;
    private Sprite sprite;
    private float jumpForce = 15f;
    float gravity = -20f;
    float verticalVelocity;
    private JumpInput jumpInput = JumpInput.Nothing;
    private float startTimer = 0f;
    private float xVelocity = 0f;
    public JumpState(NewPlayerController player, JumpInput jumpInput) : base(player)
    {
        this.jumpInput = jumpInput;
    }

    public override void Enter()
    {
        verticalVelocity = jumpForce; // start jump
        switch (jumpInput)
        {
            case JumpInput.Right:
                jumpDirection = new Vector2(1f, 1f).normalized;
                player.sr.sprite = player.jumpSprites[0];
                xVelocity = 5f;
                break;
            case JumpInput.Left:
                jumpDirection = new Vector2(-1f, 1f).normalized;
                xVelocity = -5f;
                player.sr.sprite = player.jumpSprites[1];
                break;
            case JumpInput.Up:
                jumpDirection = Vector2.up;
                xVelocity = 0f;
                player.sr.sprite = player.jumpSprites[2];
                break;
            default:
                jumpDirection = Vector2.up;
                xVelocity = 0f;
                player.sr.sprite = player.jumpSprites[0];
                break;
        }
    }
    public override void Update()

    {
        HandleNextState();
    }

    public override void FixedUpdate()
    {
        // This bit is for landing detection and transition to IdleState
        startTimer += Time.fixedDeltaTime;
        if (player.isGrounded && startTimer > 0.3f)
        {
            verticalVelocity = 0;
            player.stateMachine.ChangeState(new IdleState(player));
        }
    }

    public override Vector2 GetDesiredMovement()
    {
        // Apply movement and gravity
        verticalVelocity += gravity * Time.fixedDeltaTime;
        return new Vector2(player.horizontalMultiplier * xVelocity, verticalVelocity) * Time.fixedDeltaTime;
    }

    public override void Exit()
    {
        player.shouldJump = false;
    }

    //public override void HandleNextState()
    //{
    //    if (player.isGrounded)
    //    {
    //        if (Mathf.Abs(player.moveInput.x) > 0.1f)
    //        {
    //            player.stateMachine.ChangeState(new WalkState(player));
    //        }
    //        else
    //        {
    //            player.stateMachine.ChangeState(new IdleState(player));

    //        }
    //    }

    //    //Jump Attack transition
    //    if (player.input == MoveInput.Hit)
    //    {
    //        player.stateMachine.ChangeState(new JumpAttackState(player, MoveInput.Hit_Jump));
    //        return;
    //    }else if(player.input == MoveInput.Kick)
    //    {
    //        player.stateMachine.ChangeState(new JumpAttackState(player, MoveInput.Kick_Jump));
    //        return;
    //    }

    //}

}
