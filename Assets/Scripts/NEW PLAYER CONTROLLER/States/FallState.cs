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

        if (player.fallSounds != null)
        {
            AudioManagerTwo.Instance.PlaySFX(player.fallSounds[0]);
        }
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
            player.velocity.x = xVelocity;
            player.velocity.y += player.gravity * Time.fixedDeltaTime;
        }

        player.velocity = player.PushboxFeetCalculator(player.velocity);

        player.transform.Translate(player.velocity * Time.fixedDeltaTime);
    }

    public override void Exit()
    {
        player.isGrounded = true;
        player.velocity.y = 0f;
        player.velocity.x = 0f;

    }
}
