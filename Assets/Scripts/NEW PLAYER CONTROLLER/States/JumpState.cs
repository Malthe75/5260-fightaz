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
        player.Movement.hasLanded = false;
        player.isGrounded = false; // mark as airborne
        switch (jumpInput)
        {
            case JumpInput.Right:
                xVelocity = 5f;
                if (player.facing == -1) animateBackwards = true;
                break;
            case JumpInput.Left:
                xVelocity = -5f;
                if (player.facing == 1) animateBackwards = true;
                break;
            default:
                xVelocity = 0f;
                break;
        }

        float jumpTime = player.Movement.CalculateJumpTime(player.jumpForce);

        if (animateBackwards) player.Animation.SetJumpAnimation(AnimationState.BackwardsJumping, jumpTime);
        else player.Animation.SetJumpAnimation(AnimationState.Jumping, jumpTime);
        player.Movement.SetJump(xVelocity, player.jumpForce);

        if (player.jumpSounds != null)
        {
            AudioManagerTwo.Instance.PlaySFX(player.jumpSounds[0]);
        }

    }
    public override void Update()
    {
        HandleNextState();

    }


    public override void HandleNextState()
    {
        if (player.shouldAttack)
        {
            SetJumpAttack();
            return;
        }
        if (player.Movement.hasLanded && Time.time - player.jumpPressedTime <= player.jumpBufferTime)
        {
            Debug.Log("Buffered jump executed");
            player.jumpPressedTime = -1f;
            player.stateMachine.ChangeState(new JumpState(player, player.Movement.GetJumpInput(player.moveInput.x)));
        }
        else if (player.Movement.hasLanded)
        {
            player.Movement.hasLanded = true;
            player.stateMachine.ChangeState(new IdleState(player));
        }
    }

    private void SetJumpAttack()
    {
        player.SetAttack(MoveInput.Hit_Jump);
        int index = player.Animation.GetCurrentIndex();
        player.stateMachine.ChangeState(new AttackState(player));
    }

    public override void Exit()
    {
        player.shouldJump = false;
        //player.Movement.SetIdle();
    }

}
