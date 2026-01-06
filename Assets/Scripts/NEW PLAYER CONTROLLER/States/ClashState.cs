using UnityEngine;

public class ClashState : PlayerState
{

    public ClashState(NewPlayerController player) : base(player)
    {
    }

    public override void Enter()
    {
        player.Animation.SetSprite(player.idleSprites[0]);
        Debug.Log("Im clashing, are u clashing?");
    }

}
