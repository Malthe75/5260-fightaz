using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows;

public class AttackState : PlayerState
{
    private AttackInput attackInput;

    public AttackState(NewPlayerController player, AttackInput input) : base(player)
    {
        attackInput = input;
    }

    public override void Enter()
    {
        // Fix: Access the `frameSprite` property of the `AttackFrameData` object
        player.sr.sprite = player.attackMappings
            .FirstOrDefault(am => am.input == attackInput)
            ?.attack.frames[0].frameSprite;
    }
}

    
    