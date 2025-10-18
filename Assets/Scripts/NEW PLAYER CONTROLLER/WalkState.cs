using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : PlayerState
{
    private float animationTimer;

    public WalkState(NewPlayerController player) : base(player) { }
    // Start is called before the first frame update
    public override void Enter()
    {
        // Set first walk sprite?
        Debug.Log("Walk state Start");
        player.sr.sprite = player.walkSprites[0];
    }

    // Update is called once per frame
    public override void Update()
    {
        // Move the player
        player.rb.velocity = new Vector2(player.moveInput.x * player.walkSpeed, player.rb.velocity.y);

        // Transition back to IdleState if no movement
        if (Mathf.Abs(player.moveInput.x) < 0.01f)
        {
            player.stateMachine.ChangeState(new IdleState(player));
        }

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
    }

    public override void Exit()
    {
        Debug.Log("Exited Walk State");
        // Stop horizontal velocity (optional)
        player.rb.velocity = new Vector2(0, player.rb.velocity.y);
    }
}
