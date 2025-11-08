using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class FallState : PlayerState
{
    Sprite sprite = null;
    float xVelocity = 0f;
    public FallState(NewPlayerController player, Sprite sprite, float xVelocity) : base(player)
    {
        this.sprite = sprite;
        this.xVelocity = xVelocity;
    }
    public FallState(NewPlayerController player) : base(player)
    {
    }

    public override void FixedUpdate()
    {
        Gravity();
    }

    public override void Enter()
    {
       
        player.sr.sprite = player.fallSprite;
        player.isGrounded = false;
    }
    
    private void Gravity()
    {
        if (player.feet.position.y <= player.floorY)
        {
            float difference = player.floorY - player.feet.position.y;
            player.transform.position += new Vector3(0, difference, 0); // Snap player up so feet touch ground

            player.stateMachine.ChangeState(new IdleState(player));
        }
        else
        {
            Debug.Log(xVelocity);
            player.velocity.x = xVelocity;
            player.velocity.y += player.gravity * Time.fixedDeltaTime;
        }

        player.transform.Translate(player.velocity * Time.fixedDeltaTime);
    }

    public override void Exit()
    {
        player.isGrounded = true;
        player.velocity.y = 0f;
        player.velocity.x = 0f;

    }
}
