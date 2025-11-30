using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class JumpAttackState : PlayerState
{
    private Coroutine jumpAttackRoutine;
    private int currentIndex;
    private bool playBackwards;

    public JumpAttackState(NewPlayerController player, int currentIndex, bool playBackwards = false) : base(player)
    {
        this.currentIndex = currentIndex;
        this.playBackwards = playBackwards;
    }

    public override void Enter()
    {
        player.StartCoroutine(ShowFrames(player.attack));
    }
   
    private IEnumerator ShowFrames(AttackData attack)
    {
        Debug.Log(currentIndex);
        if (!playBackwards)
        {
            for (int i = currentIndex; i < attack.frames.Count; i++)
                yield return DoFrame(attack.frames[i]);
        }
        else
        {
            for (int i = currentIndex; i >= 0; i--)
                yield return DoFrame(attack.frames[i]);
        }

        HandleNextState();
    }

    private IEnumerator DoFrame(AttackFrameData frame)
    {
        // Sprite
        player.Animation.SetSprite(frame.frameSprite);

        // Hitbox
        if (frame.hasHitbox)
            player.attackHitbox.Activate(frame);

        // Sound
        if (frame.attackSound != null)
            AudioManagerTwo.Instance.PlaySFX(frame.attackSound);

        // Wait
        yield return new WaitForSeconds(frame.frameDuration);

        // Cleanup
        if (frame.hasHitbox)
            player.attackHitbox.Deactivate();
    }
    public override void HandleNextState()
    {

        if (player.Movement.hasLanded && Time.time - player.jumpPressedTime <= player.jumpBufferTime)
        {
            Debug.Log("Buffered jump executed");
            player.jumpPressedTime = -1f; 
            player.stateMachine.ChangeState(new JumpState(player, player.Movement.GetJumpInput(player.moveInput.x)));
        }else if (player.Movement.hasLanded)
        {
            player.Movement.hasLanded = true;
            player.stateMachine.ChangeState(new IdleState(player));
        }else if(player.Movement.IsGrounded())
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
    }

     public override void Exit()
    {
        if(jumpAttackRoutine != null)
        {
            player.StopCoroutine(jumpAttackRoutine);
            jumpAttackRoutine = null;
            player.attackHitbox.Deactivate();
        }
        player.shouldAttack = false;
    }


}

    
    