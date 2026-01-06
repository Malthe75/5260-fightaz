using System.Threading;
using UnityEngine;

public class ClashState : PlayerState
{
    public ClashState(NewPlayerController player) : base(player)
    {
    }


    public override void Update()
    {
        if(player.Clash.hasEnded)
        {
            player.stateMachine.ChangeState(new IdleState(player));
        }
    }
    public override void Enter()
    {
        player.Clash.PlayClashEffect();
    }


    


}
