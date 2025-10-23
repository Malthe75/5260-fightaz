using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class WalkState : PlayerState
{
    private float animationTimer;

    private float idleBuffer = 0.15f;
    private float idleTimer = 0f;

    public WalkState(NewPlayerController player) : base(player) {
        Debug.Log("Why is thing happening?");
        Debug.Log(player.name);
        }
    // Start is called before the first frame update
    public override void Enter()
    {
        player.sr.sprite = player.walkSprites[1];
    }

    // Update is called once per frame
    public override void Update()
    {
        // Move the player
        player.rb.velocity = new Vector2(player.moveInput.x * player.walkSpeed, player.rb.velocity.y);

        // Handle walk animation
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
            if(idleTimer >= idleBuffer)
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
