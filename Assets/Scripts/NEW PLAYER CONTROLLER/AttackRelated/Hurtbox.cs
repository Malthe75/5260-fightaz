using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    private UIHandler uiHandler;
    private GameObject uiObject;
    private NewPlayerController player;
    private void Awake()
    {
        player = GetComponentInParent<NewPlayerController>();
        // Find the UI objects for updating UI.
        uiObject = GameObject.Find("UIHandler");
        uiHandler = uiObject.GetComponent<UIHandler>();
    }
    public void TakeDamage(int damage, AttackFrameData attack)
    {
        player.TakeHit(damage, attack);
        Debug.Log(transform.tag);
        uiHandler.TakeDamage(damage, transform.tag);
    }
}
