using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAttackState : PlayerState
{
    private MoveInput attackInput;
    private int dashDirection;
    private float vecticalVelocity = 0f;
    private float xVelocity = 0f;
    private float verticalVelocity = 0f;

    public JumpAttackState(NewPlayerController player, MoveInput attackInput, float xVelocity, float verticalVelocity) : base(player)
    {
        this.attackInput = attackInput;
        this.xVelocity = xVelocity;
        this.verticalVelocity = verticalVelocity;
    }

    public override void Enter()
    {
    }
    public override void Exit() { base.Exit(); }

    public override void Update() 
    { 
        HandleNextState();
    }

    public override Vector2 GetDesiredMovement()
    {
        // Apply movement and gravity
        verticalVelocity += player.gravity * Time.fixedDeltaTime;
        return new Vector2(player.horizontalMultiplier * xVelocity, verticalVelocity) * Time.fixedDeltaTime;
    }

    public override void HandleNextState()
    {
        if (player.isGrounded)
        {
            if (Mathf.Abs(player.moveInput.x) > 0.1f)
            {
                player.stateMachine.ChangeState(new WalkState(player));
            }
            else
            {
                player.stateMachine.ChangeState(new IdleState(player));

            }
        }
    }

    //private void attack(MoveInput attackInput)
    //{

    //    player.StartCoroutine(showFrames(player.moveMap.GetAttack(attackInput)));

    //}

    //private IEnumerator showFrames(AttackData attack)
    //{
    //    foreach (var attackFrame in attack.frames)
    //    {
    //        // Sprite
    //        player.sr.sprite = attackFrame.frameSprite;

    //        // Activate hitbox if it exists
    //        if (attackFrame.hasHitbox)
    //        {
    //            player.attackHitbox.Activate(attackFrame);

    //            if (attackFrame.dashForce != 0)
    //            {
    //                dash(attackFrame.dashForce);
    //            }

    //        }
    //        if (attackFrame.attackSound != null)
    //        {
    //            AudioManagerTwo.Instance.PlaySFX(attackFrame.attackSound);
    //        }
    //        // Timer
    //        yield return new WaitForSeconds(attackFrame.frameDuration);

    //        // Deactivate hitbox if it exists
    //        if (attackFrame.hasHitbox)
    //        {
    //            player.attackHitbox.Deactivate();
    //        }
    //    }
    //    HandleNextState();
    //}

    //private void dash(float dashForce)
    //{
    //    if (player.tag == "Player1") dashDirection = 1;
    //    else dashDirection = -1;
    //    player.rb.AddForce(new Vector2(dashDirection * dashForce, 0f), ForceMode2D.Impulse);
    //}


}
