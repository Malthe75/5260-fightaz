using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{
    private FightManagerTest fightManagerTest;
    private GameObject uiObject;
    private UIHandler uiHandler;
    private HitFeedback hitFeedback; 

    void Start()
    {
       fightManagerTest = this.GetComponent<FightManagerTest>();
       uiObject = GameObject.Find("UIHandler");
       uiHandler = uiObject.GetComponent<UIHandler>();
       hitFeedback = this.GetComponent<HitFeedback>();
    }



    // Clash mechanic, two players use the same move at same time - resulting in some mechanic, where maybe a button mash? or like press the right order of buttons
    void Clash()
    {

    }

    // Updae Takedamage so i dont have two different methods for each player
    //void applyHit(bool player1WasHit, int attackDamage)
    //{


    //    if (player1WasHit)
    //    {
    //        Debug.Log("Player 1 Got Hit");
    //        uiHandler.TakeDamage1(attackDamage); // Update UI for player health
    //        hitFeedback.TriggerHitEffect(); // Trigger hit feedback effects - Like camera shake, sound, etc. / Maybe even a hit flash on the player sprite

    //        fightManagerTest.p1sr.color = Color.red; // Temporary visual feedback for hit
    //    }
    //    else if(!player1WasHit){
    //        Debug.Log("Player 2 Got Hit");
    //        uiHandler.TakeDamage2(attackDamage); // Update UI for player health
    //        hitFeedback.TriggerHitEffect(); // Trigger hit feedback effects - Like camera shake, sound, etc. / Maybe even a hit flash on the player sprite

    //        fightManagerTest.p2sr.color = Color.red; // Temporary visual feedback for hit
    //    }

    //}


    

}
