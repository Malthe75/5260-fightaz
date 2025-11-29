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
        player.isGrounded = false; // mark as airborne
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

        player.Movement.SetJump(xVelocity, player.jumpForce);
        player.Animation.SetAnimation(AnimationState.Jumping, false);

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
        if (player.Movement.hasLanded)
        {
            player.Movement.hasLanded = false;
            player.stateMachine.ChangeState(new IdleState(player));
            return;
        }
    }

    public override void Exit()
    {
        player.shouldJump = false;
        player.Movement.SetIdle();
    }

}
