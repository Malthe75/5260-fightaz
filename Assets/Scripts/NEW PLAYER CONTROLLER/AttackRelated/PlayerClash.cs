using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct ClashSpriteEntry
{
    public Sprite sprite;
    public string value;
}
public class PlayerClash : MonoBehaviour
{

    public List<ClashSpriteEntry> clashSprites;
    public List<ClashSpriteEntry> clashList = new List<ClashSpriteEntry>();
    private SpriteRenderer sr;
    private PlayerClash clashEnemy;
    
    [HideInInspector] public bool hasEnded = false;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void PlayClashEffect()
    {
        hasEnded = false;
        clashList.Clear();
        for (int i = 0; i < 4; i++)
        {
            int r = Random.Range(0, 4); // 0, 1, 2, or 3
            clashList.Add(clashSprites[r]);

        }

        sr.sprite = clashList[0].sprite;
    }

    public void OnCross(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if(context.performed)
        {
            CheckForClashHit(context.action.name);

            Debug.Log("Cross pressed");
        }
    }

    public void OnCircle(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        // Resolve the attack
        if (context.performed)
        {
            CheckForClashHit(context.action.name);
            Debug.Log("Circle pressed");
        } 

    }
    public void OnSquare(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        // Resolve the attack
        
        if (context.performed)
        {
            CheckForClashHit(context.action.name);
            Debug.Log("Square pressed");
        }

    }
    public void OnTriangle(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        // Resolve the attack
        if (context.performed)
        {
            CheckForClashHit(context.action.name);
             Debug.Log("Triangle pressed");
        }
       

    }

    public void CheckForClashHit(string actionName)
    {
        if(clashList[0].value == actionName)
            {
                clashList.RemoveAt(0);
                if(clashList.Count == 0)
                {
                    hasEnded = true;
                    clashEnemy.hasEnded = true;
                    sr.sprite = null;
                    clashEnemy.sr.sprite = null;
                    
                    return;
                }
                sr.sprite = clashList[0].sprite;
                Debug.Log("Clash Hit!");
            }
    }


    public void SetEnemy(PlayerClash enemy)
    {
        clashEnemy = enemy;
    }




    
}
