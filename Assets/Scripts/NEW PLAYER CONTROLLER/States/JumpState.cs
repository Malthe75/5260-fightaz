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
    private JumpInput jumpInput = JumpInput.Nothing;
    private float xVelocity = 0f;
    private bool animateBackwards;
    public JumpState(NewPlayerController player, JumpInput jumpInput) : base(player)
    {
        animateBackwards = false;
        this.jumpInput = jumpInput;
    }

    public override void Enter()
    {
        player.isGrounded = false; // mark as airborne
        switch (jumpInput)
        {
            case JumpInput.Right:
                xVelocity = 5f;
                if(player.facing == -1) animateBackwards = true;
                break;
            case JumpInput.Left:
                xVelocity = -5f;
                if(player.facing == 1) animateBackwards = true;
                break;
            default:
                xVelocity = 0f;
                break;
        }
        
        float jumpTime = player.Movement.CalculateJumpTime(player.jumpForce);

        if(animateBackwards) player.Animation.SetJumpAnimation(AnimationState.BackwardsJumping, jumpTime);
        else player.Animation.SetJumpAnimation(AnimationState.Jumping, jumpTime);
        player.Movement.SetJump(xVelocity, player.jumpForce);

        if (player.jumpSounds != null)
        {
            AudioManagerTwo.Instance.PlaySFX(player.jumpSounds[0]);
        }

    }
    public override void Update()
    {
        HandleBufferedJump();

    }


    private void HandleBufferedJump()
    {
        if (player.Movement.hasLanded && Time.time - player.jumpPressedTime <= player.jumpBufferTime)
        {
            Debug.Log("Buffered jump executed");
            player.Movement.hasLanded = false;
            player.jumpPressedTime = -1f; // consume the buffered input
            // Transition to JumpState again based on input direction
            if (player.moveInput.x > 0f) player.stateMachine.ChangeState(new JumpState(player, JumpInput.Right));
            else if (player.moveInput.x < 0f) player.stateMachine.ChangeState(new JumpState(player, JumpInput.Left));
            else player.stateMachine.ChangeState(new JumpState(player, JumpInput.Up));
        }else if (player.Movement.hasLanded)
        {
            player.Movement.hasLanded = false;
            player.stateMachine.ChangeState(new IdleState(player));
        }
    }

    public override void Exit()
    {
        player.shouldJump = false;
        player.Movement.SetIdle();
    }

}
