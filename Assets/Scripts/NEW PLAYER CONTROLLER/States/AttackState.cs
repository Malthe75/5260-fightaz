using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : PlayerState {

    private AttackInput attackInput;
    private AttackData attackData;

    public AttackState(NewPlayerController player, AttackInput input) : base(player) {
        attackInput = input;
    }
    public override void Enter()
    {

        // attackData = player.comboLibrary.GetAttackForInput(attackInput);

        PerformAttack(attackInput);
    }

    public override void Exit() 
    {
    }

    public override void Update() 
    {
    }

    private void PerformAttack(AttackInput input)
    {
        player.sr.sprite = player.attackSprites[0];
    }

}
