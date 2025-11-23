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
    float dt = Time.fixedDeltaTime;

    // integrate vertical velocity with gravity (gravity should be negative)
    player.verticalVelocity += player.gravity * dt;

    // Optionally update sprite based on vertical velocity (you already have this helper)
    UpdateJumpSprite_FullArc();

    // Build movement delta (what CalculateAllowedMovement expects)
    Vector2 movement = new Vector2(xVelocity, player.verticalVelocity) * dt;

    // CalculateAllowedMovement returns a world-space next position (it uses rb.position + movement internally)
    Vector2 nextPos = player.CalculateAllowedMovement(movement);

    // Landing detection — small epsilon to avoid float issues
    const float eps = 0.001f;
    if (nextPos.y <= player.floorY + eps && player.verticalVelocity <= 0f)
    {
        // Snap to exact floor, stop vertical movement, mark grounded and switch to Idle
        nextPos.y = player.floorY;
        player.verticalVelocity = 0f;
        player.velocity = Vector2.zero;
        player.rb.MovePosition(nextPos);
        player.stateMachine.ChangeState(new IdleState(player));
        return;
    }

    // Normal movement while in air
    player.rb.MovePosition(nextPos);
}

    private int jumpSpriteIndex = -1;

private void UpdateJumpSprite_FullArc(float maxFall = -1f)
{
    var sprites = player.jumpSprites;
    if (sprites == null || sprites.Length == 0) return;

    float maxV = player.jumpForce;
    // allow caller to pass a desired min (negative). Default use -maxV if not provided.
    float minV = (maxFall < 0f) ? -maxV : maxFall;

    if (Mathf.Approximately(maxV, minV)) return;

    // t = 0 at maxV, 1 at minV (so index increases as velocity goes down)
    float t = Mathf.InverseLerp(maxV, minV, player.verticalVelocity);

    int idx = Mathf.Clamp(Mathf.FloorToInt(t * sprites.Length), 0, sprites.Length - 1);

    if (idx != jumpSpriteIndex)
    {
        jumpSpriteIndex = idx;
        player.sr.sprite = sprites[jumpSpriteIndex];
    }
}

    public override void Exit()
    {
        player.shouldJump = false;
    }

}
