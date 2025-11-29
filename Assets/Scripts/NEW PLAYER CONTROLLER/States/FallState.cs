using UnityEngine;

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

        if (player.fallSounds != null && player.fallSounds.Length > 0)
        {
            AudioManagerTwo.Instance.PlaySFX(player.fallSounds[0]);
        }
    }
    
    private void Gravity()
    {
        // float dt = Time.fixedDeltaTime;

        // // integrate velocity
        // player.velocity.x = xVelocity;
        // player.velocity.y += player.gravity * dt;

        // // allow existing push/feet logic to modify velocity (optional)
        // //player.velocity = player.PushboxFeetCalculator(player.velocity);

        // // movement delta (what CalculateAllowedMovement expects)
        // Vector2 movement = player.velocity * dt;

        // // returns a world-space nextPos (it does rb.position + movement internally)
        // Vector2 nextPos = player.CalculateAllowedMovement(movement);

        // const float eps = 0.001f;

        // // landed on the floor: stop vertical velocity, mark grounded and switch state
        // if (nextPos.y <= player.floorY + eps && player.velocity.y <= 0f)
        // {
        //     nextPos.y = player.floorY;
        //     player.velocity.y = 0f;
        //     player.rb.MovePosition(nextPos);
        //     player.stateMachine.ChangeState(new IdleState(player));
        //     return;
        // }

        // // normal movement
        // player.rb.MovePosition(nextPos);
    }


    public override void Exit()
    {
        // player.isGrounded = true;
        // player.velocity.y = 0f;
        // player.velocity.x = 0f;

    }
}
