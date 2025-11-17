using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
