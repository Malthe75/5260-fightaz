using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows;

public class AttackState : PlayerState {

    private AttackInput attackInput;
    private AttackData attackData;

    // For combo management
    private bool isAttacking = false;
    private bool queuedNextAttack = false;
    private AttackInput currentComboInput;
    private int comboCounter = 0;
    private List<ComboData> currentCombos;

    public AttackState(NewPlayerController player, AttackInput input) : base(player) {
        attackInput = input;
    }
    public override void Enter()
    {
      
        currentCombos = player.comboLibrary.combos.ToList();
        Debug.Log("ENTER ATTACK STATE");
        PerformAttack(attackInput);

    }

    public void QueueNextAttack(AttackInput input)
    {
        PerformAttack(input);
    }

    public override void Exit() 
    {
    }

    public override void Update() 
    {
    }

    private void PerformAttack(AttackInput input)
    {
        Debug.Log("Attacking");
        // Check if the player is already attacking, if so, put the next attack in queue.
        if (isAttacking){
            currentComboInput = input;
            queuedNextAttack = true;
            return;
        }
        CheckForCombos(input);

        //player.sr.sprite = player.attackSprites[0];
    }
    
    private void CheckForCombos(AttackInput input)
    {
        Debug.Log("Checking for combos");
        // Removes all combos that don't match the current input
        currentCombos.RemoveAll(combo =>
        {
            // Return true (i.e., remove) if we've gone past the length of this combo
            if (combo.inputSequence.Count <= comboCounter)
            {
                Debug.Log("byo?");
                return true;
            }
            // Return true (i.e., remove) if the current input doesn't match
            return combo.inputSequence[comboCounter] != input;
        });
        Debug.Log("Why am i here?");
        Debug.Log(currentCombos.Count);
        // Check if input is possible for any of the combos
        if (currentCombos.Count == 0)
        {
            FinishCombo();
            if (currentCombos.Exists(c => c.inputSequence[0] == input))
            {
                CheckForCombos(input);
            }
            else
            {
                Debug.Log("No combos match input. Maybe play a default light attack?");
            }
            return;
        }
        // Incremenet comboCounter
        comboCounter++;

        // Start actual attack
        player.StartCoroutine(PlayAttackCoroutine(currentCombos[0].attacks[comboCounter - 1]));

        // Finish combo
        if (currentCombos.Count <= 1 && comboCounter >= currentCombos[0].inputSequence.Count)
        {
            FinishCombo();
        }
    }

    private void FinishCombo()
    {
        Debug.Log("finishing combO=");
        comboCounter = 0;
        currentCombos = player.comboLibrary.combos.ToList();
    }

     private IEnumerator PlayAttackCoroutine(AttackData attack)
    {
        Debug.Log("Playing attack coroutine");
        isAttacking = true;

        Debug.Log(attack);
        foreach (var frame in attack.frames)
        {
            player.sr.sprite = frame.frameSprite;

            //if (frame.hasHitbox)
            //{
            //    ActivateHitbox(frame.hitboxSize, frame.hitboxOffset);
            //}

            //if (frame.attackSound != null)
            //{
            //    AudioSource.PlayClipAtPoint(frame.attackSound, transform.position);
            //}

            // Simple test — feel free to make frame-specific movement later
            //float speed = 10f;
            //player.rb.MovePosition(player.rb.position + new Vector2(-speed * player.transform.localScale.x, 0) * Time.fixedDeltaTime);
            //Debug.Log("Attack force applied");


            Debug.Log("Foreach FRAME");
            yield return new WaitForSeconds(frame.frameDuration);
        }
        Debug.Log("Finished attack frames, starting combo buffer");
        float comboBufferTime = attack.frames[attack.frames.Count - 1].frameDuration;
        float timer = 0f;

        while(timer < comboBufferTime)
        {
            // another attack was queued, during the buffer time, so go to that.
            if (queuedNextAttack)
            {
                queuedNextAttack = false;
                isAttacking = false;
                //overrideMovement = false;
                PerformAttack(currentComboInput);
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        // Reset to "idle" sprite:
        FinishCombo();
        //sr.sprite = isCrouching ? crouchSprites[0] : walkSprites[0];
        isAttacking = false;
        //overrideMovement = false;

        // If no attack was queued, go back to idle state.
        player.stateMachine.ChangeState(new IdleState(player));
    }

}
