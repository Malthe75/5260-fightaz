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
                sprite = player.jumpSprites[0];
                xVelocity = 5f;
                break;
            case JumpInput.Left:
                jumpDirection = new Vector2(-1f, 1f).normalized;
                xVelocity = -5f;
                sprite = player.jumpSprites[1];
                break;
            case JumpInput.Up:
                jumpDirection = Vector2.up;
                xVelocity = 0f;
                sprite = player.jumpSprites[2];
                break;
            default:
                jumpDirection = Vector2.up;
                xVelocity = 0f;
                sprite = player.jumpSprites[0];
                break;
        }

        //Jump();
    }
    public override void Update()

    {
        HandleNextState();
    }

    public override void FixedUpdate()
    {

        Vector2 move = new Vector2(xVelocity, verticalVelocity) * Time.fixedDeltaTime;
        // Move player
        player.rb.MovePosition(player.rb.position + move);

        // Apply gravity
        verticalVelocity += gravity * Time.fixedDeltaTime;

        startTimer += Time.fixedDeltaTime;
        // Check if landed
        if (player.isGrounded && startTimer > 0.3f)
        {
            verticalVelocity = 0;
            player.stateMachine.ChangeState(new IdleState(player));
        }
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

    //private void Jump()
    //{
    //    player.rb.velocity = new Vector2(player.rb.velocity.x, 0f);

    //    // Apply vertical jump force
    //    player.rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

    //    if (jumpDirection.x != 0)
    //    {
    //        player.rb.AddForce(new Vector2(jumpDirection.x * jumpForce, 0f), ForceMode2D.Impulse);
    //    }


    //    player.sr.sprite = sprite;
    //    player.isGrounded = false;
    //}

}
