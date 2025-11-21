using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    private JumpInput jumpInput = JumpInput.Nothing;
    private float xVelocity = 0f;
    public JumpState(NewPlayerController player, JumpInput jumpInput) : base(player)
    {
        this.jumpInput = jumpInput;
    }

    public override void Enter()
    {
        player.verticalVelocity = player.jumpForce; // start jump
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

        if (player.jumpSounds != null)
        {
            AudioManagerTwo.Instance.PlaySFX(player.jumpSounds[0]);
        }
    }
    public override void Update()

    {
        HandleNextState();
    }

    public override void FixedUpdate()
    {
        Jump();
    }

    private void Jump()
    {
        // Apply jump
        player.verticalVelocity += player.gravity * Time.fixedDeltaTime;

        // Move the player
        Vector2 movement = new Vector2(xVelocity, player.verticalVelocity) * Time.fixedDeltaTime;
        Vector2 nextPos = player.CalculateAllowedMovement(movement);
        player.rb.MovePosition(nextPos);
        //player.transform.Translate(movement);

        // Check if peak is reached (vertical velocity <= 0)  switch to fall state
        if (player.verticalVelocity <= 0)
        {
            player.stateMachine.ChangeState(new FallState(player, player.sr.sprite, xVelocity));
        }

    }

    public override void Exit()
    {
        player.shouldJump = false;
    }






    //public override void HandleNextState()
    //{
    //    if (player.isGrounded && startTimer > 0.3f)
    //    {
    //        verticalVelocity = 0;
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
    //        player.stateMachine.ChangeState(new JumpAttackState(player, MoveInput.Hit_Jump, xVelocity, verticalVelocity));
    //        return;
    //    }
    //    else if (player.input == MoveInput.Kick)
    //    {
    //        player.stateMachine.ChangeState(new JumpAttackState(player, MoveInput.Kick_Jump, xVelocity, verticalVelocity));
    //        return;
    //    }

    //}

}
