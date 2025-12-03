using UnityEngine;

public class FallState : PlayerState
{
    private float xVelocity;
    private float yVelocity;
    public FallState(NewPlayerController player, float xVelocity = 0f, float yVelocity = -0.02f) : base(player)
    {
        this.xVelocity = xVelocity;
        this.yVelocity = yVelocity;
    }

    public override void Enter()
    {
        player.Animation.SetSprite(player.fallSprite);
        Debug.Log(xVelocity + " " + yVelocity);
        player.Movement.SetFall(xVelocity, yVelocity);


        // if (player.fallSounds != null)
        // {
        //     AudioManagerTwo.Instance.PlaySFX(player.jumpSounds[0]);
        // }

    }
    public override void Update()
    {
        HandleNextState();

    }


    public override void HandleNextState()
    {
        if (player.Movement.IsGrounded() && Time.time - player.jumpPressedTime <= player.jumpBufferTime)
        {
            Debug.Log("Buffered jump executed");
            player.jumpPressedTime = -1f;
            player.stateMachine.ChangeState(new JumpState(player, player.Movement.GetJumpInput(player.moveInput.x)));
        }
        else if (player.Movement.IsGrounded())
        {
            //player.Movement.hasLanded = true;
            player.stateMachine.ChangeState(new IdleState(player));
        }
    }

    public override void Exit()
    {
        player.shouldJump = false;
    }

}
