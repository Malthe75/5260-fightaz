using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements.Experimental;
using UnityEngine.Windows;

public class WalkState : PlayerState
{
    private float animationTimer;
    private float idleBuffer = 0.15f;
    private float idleTimer = 0f;

    public WalkState(NewPlayerController player) : base(player) { }
    // Start is called before the first frame update
    public override void Enter()
    {
        player.sr.sprite = player.walkSprites[1];
    }
    public override void Exit()
    {
        player.Movement.SetIdle();
    }

    // Update is called once per frame
    public override void Update()
    {
        HandleNextState();
        PlayAnimation();
    }

    public override void FixedUpdate()
    {
        player.Movement.SetMove(player.moveInput.x, player.walkSpeed);
    }

    public override void HandleNextState()
    {
        if (player.shouldAttack)
        {
            player.stateMachine.ChangeState(new AttackState(player));
        }
        // Jump transition
        if (player.shouldJump)
        {
            if (player.moveInput.x > 0f) player.stateMachine.ChangeState(new JumpState(player, JumpInput.Right));
            else if (player.moveInput.x < 0f) player.stateMachine.ChangeState(new JumpState(player, JumpInput.Left));
        }
    }


    private void PlayAnimation()
    {
        animationTimer += Time.deltaTime;
        if (animationTimer >= player.animationSpeed)
        {
            // Advance to the next sprite
            int currentIndex = System.Array.IndexOf(player.walkSprites, player.sr.sprite);
            int nextIndex = (currentIndex + 1) % player.walkSprites.Length;
            player.sr.sprite = player.walkSprites[nextIndex];
            // Reset timer
            animationTimer = 0f;
        }
        if (Mathf.Abs(player.moveInput.x) < 0.01)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleBuffer)
            {

                player.stateMachine.ChangeState(new IdleState(player));
            }
        }
        else
        {
            idleTimer = 0f;
        }
    }


}
