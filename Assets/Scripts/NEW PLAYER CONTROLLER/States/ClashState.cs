using System.Threading;
using UnityEngine;

public class ClashState : PlayerState
{
    private float timer = 0.5f;
    public ClashState(NewPlayerController player) : base(player)
    {
    }

    public override void Update()
    {

        
        HandleNextState();
    }
    public override void Enter()
    {
        Debug.Log("Im clashing, are u clashing?");
    }

}
