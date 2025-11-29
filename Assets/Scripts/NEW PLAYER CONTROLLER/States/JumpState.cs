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
        Debug.Log("Entered Jump State");
        player.isGrounded = false; // mark as airborne
        switch (jumpInput)
        {
            case JumpInput.Right:
                jumpDirection = new Vector2(1f, 1f).normalized;
                xVelocity = 5f;
                break;
            case JumpInput.Left:
                jumpDirection = new Vector2(-1f, 1f).normalized;
                xVelocity = -5f;
                break;
            default:
                jumpDirection = Vector2.up;
                xVelocity = 0f;
                break;
        }

        float jumpTime = player.Movement.CalculateJumpTime(player.jumpForce);
        player.Animation.SetAnimation(AnimationState.Jumping, jumpTime, false);
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

    public override void FixedUpdate()
    {
        if (player.Movement.hasLanded)
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
