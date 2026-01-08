using System.Threading;
using UnityEngine;

public class ClashState : PlayerState
{
    public ClashState(NewPlayerController player) : base(player)
    {
    }


    public override void Update()
    {
        if(!player.Clash.isInClash)
        {
            player.stateMachine.ChangeState(new IdleState(player));
        }
    }
    public override void Enter()
    {
        Debug.Log("Entered Clash State");
    }


    


}
