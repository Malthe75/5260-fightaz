using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtState : PlayerState
{
    public HurtState(NewPlayerController player) : base(player) { }
    // Start is called before the first frame update

    float hurtTimer = 2f;
    public override void Enter()
    {
        // Set color to red, when hurt.
        player.sr.color = Color.red;
    }
    public override void Exit()
    {
        // white should be defautlt i think.
        player.sr.color = Color.white;
    }

    public override void Update()
    {
     
        hurtTimer -= Time.deltaTime;
        if(hurtTimer <= 0)
        {
            hurtTimer = 0.3f;
            player.stateMachine.ChangeState(new IdleState(player));
        }
    }
}
