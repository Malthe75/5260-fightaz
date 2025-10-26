using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : PlayerState
{

    private Vector2 jumpDirection;
    private bool hasJumped = false;
    private Sprite sprite;

    public JumpState(NewPlayerController player) : base(player) { }

    public override void Enter()
    {
        switch (player.jumpInput)
        {
            case JumpInput.Right:
                jumpDirection = new Vector2(1f, 1f).normalized;
                sprite = player.jumpSprites[0];
                break;
            case JumpInput.Left:
                jumpDirection = new Vector2(-1f, 1f).normalized;
                sprite = player.jumpSprites[1];
                break;
            case JumpInput.Up:
                sprite = player.jumpSprites[2];
                break;
            default:
                jumpDirection = Vector2.up;
                break;
        }

        // Apply jump force
        player.rb.velocity = Vector2.zero; // reset velocity for consistent jump
        player.rb.AddForce(jumpDirection * 3, ForceMode2D.Impulse);

        hasJumped = true;

        // Set jump sprite or animation if you have one
        if (sprite != null)
            player.sr.sprite = sprite;
    }
    public override void Update()
    {

        float moveX = player.moveInput.x;
        player.rb.velocity = new Vector2(moveX * 3, player.rb.velocity.y);

        // Check landing
        if (hasJumped == true)
        {
            hasJumped = false;
            if (Mathf.Abs(player.moveInput.x) > 0.1f)
                player.stateMachine.ChangeState(new WalkState(player));
            else
                player.stateMachine.ChangeState(new IdleState(player));
        }
        
    }
}
