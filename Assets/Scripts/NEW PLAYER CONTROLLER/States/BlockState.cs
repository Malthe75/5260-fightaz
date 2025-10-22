using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockState : PlayerState
{

    public BlockState(NewPlayerController player) : base(player) { }

    public override void Enter()
    {
        player.sr.sprite = player.blockSprites[0];
    }
}
