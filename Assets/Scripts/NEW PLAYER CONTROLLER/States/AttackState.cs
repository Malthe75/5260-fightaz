using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AttackState : PlayerState
{
    private Coroutine attackRoutine;
    private float dashForce = 0f;

    public AttackState(NewPlayerController player) : base(player)
    {
    }

    public override void Enter()
    {
        attack();
    }
    public override void Exit()
    {
        if(attackRoutine != null)
        {
            player.StopCoroutine(attackRoutine);
            attackRoutine = null;
            player.attackHitbox.Deactivate();
        }
        player.shouldAttack = false;
    }

    private void attack()
    {
        player.StartCoroutine(showFrames(player.attack));
    }

    public override void FixedUpdate()
    {
        if (dashForce != 0)
        {
            Dash();
        }
    }

    // Remember to delete coroutine if we exit the state early.
    private IEnumerator showFrames(AttackData attack)
    {
        foreach(var attackFrame in attack.frames)
        {
            // Sprite
            player.sr.sprite = attackFrame.frameSprite;

            // Dash if needed
            if (attackFrame.dashForce != 0)
            {
                dashForce = attackFrame.dashForce;
            }

            // Activate hitbox if it exists
            if (attackFrame.hasHitbox)
            {
                player.attackHitbox.Activate(attackFrame);

            }

            // Sound
            if (attackFrame.attackSound != null)
            {
                AudioManagerTwo.Instance.PlaySFX(attackFrame.attackSound);
            }
            // Timer
            yield return new WaitForSeconds(attackFrame.frameDuration);

            // Deactivate hitbox if it exists
            if (attackFrame.hasHitbox)
            {
                Debug.Log("Why am i deactivating?");
                player.attackHitbox.Deactivate();
            }
            dashForce = 0f;
        }
        HandleNextState();
    }

    public override void HandleNextState()
    {
        if(player.moveInput != Vector2.zero)
        {
            player.stateMachine.ChangeState(new WalkState(player));
        }
        else
        {
            player.stateMachine.ChangeState(new IdleState(player));
        }
    }

    private void Dash()
    {
        Vector2 desiredMove = new Vector2(player.facing * dashForce, 0f) * Time.fixedDeltaTime;
        Vector2 actualMove = player.PushboxCalculator(desiredMove);
        player.rb.MovePosition(player.rb.position + actualMove);
    }

}

    
    