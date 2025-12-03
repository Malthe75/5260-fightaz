using UnityEngine;

public class WalkState : PlayerState
{
    private float idleBuffer = 0.15f;
    private float idleTimer = 0f;

    public WalkState(NewPlayerController player) : base(player) { }
    // Start is called before the first frame update
    public override void Enter()
    {
        player.Animation.SetAnimation(AnimationState.Walking);
    }
    public override void Exit()
    {
        player.Movement.SetIdle();

    }

    // Update is called once per frame
    public override void Update()
    {
        HandleNextState();
    }

    public override void FixedUpdate()
    {
        player.Movement.SetMove(player.moveInput.x, player.walkSpeed);
    }

    public override void HandleNextState()
    {
        if (player.moveInput.x == 0f)
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



}
