using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClash : MonoBehaviour
{

    private SpriteRenderer sr;
    public bool shouldClash = false;
    public bool wonClash = false;
    public bool isInClash = false;
    [SerializeField] SpriteRenderer borderSr;
    [SerializeField] Sprite borderSprite;
    List<ClashSpriteEntry> clashList = new List<ClashSpriteEntry>();
    private AttackHitbox attackHitbox;
    
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        attackHitbox = GetComponentInParent<NewPlayerController>().GetComponentInChildren<AttackHitbox>();
    }

    public void PlayClash(List<ClashSpriteEntry> clashList)
    {
        borderSr.sprite = borderSprite;
        attackHitbox.cancelRoutine = true;
        this.clashList = clashList;
        shouldClash = false;
        isInClash = true;
        sr.sprite = this.clashList[0].sprite;
    }

    

    #region InputHandlers
    public void OnCross(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if(context.performed)
        {
            CheckForClashHit(context.action.name);

        }
    }

    public void OnCircle(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        // Resolve the attack
        if (context.performed)
        {
            CheckForClashHit(context.action.name);
        } 
    }
    public void OnSquare(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        // Resolve the attack
        
        if (context.performed)
        {
            CheckForClashHit(context.action.name);
        }
    }
    public void OnTriangle(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        // Resolve the attack
        if (context.performed)
        {
            CheckForClashHit(context.action.name);
        }

    }

    #endregion
    public void CheckForClashHit(string actionName)
    {
        if(clashList[0].value == actionName)
            {
                clashList.RemoveAt(0);
                if(clashList.Count == 0)
                {
                    wonClash = true;
                    return;
                }
                StartCoroutine(ClashRoutine(Color.green));
            }
        else
        {
            StartCoroutine(ClashRoutine(Color.red));
        }
    }

    private IEnumerator ClashRoutine(Color color)
    {
        borderSr.color = color;
        yield return new WaitForSeconds(0.25f);
        sr.sprite = clashList[0].sprite;
        borderSr.color = Color.white;

    }

    public void ResetClash()
    {
        borderSr.sprite = null;
        sr.sprite = null;
        isInClash = false;
        wonClash = false;
        clashList.Clear();
    }




    
}
