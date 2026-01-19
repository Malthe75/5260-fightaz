using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Selector : MonoBehaviour
{
    [SerializeField] private List<MenuItemBase> selectables;
    private bool isLocked = false;
    private int currentIndex = 0;

    public void OnMenuMove(InputValue value)
    {
        if(isLocked) return;
        Vector2 input = value.Get<Vector2>();
        Debug.Log("Selector Move Input: " + input);
        if(input.x > 0)
        {
            MoveNext();
        }
        else if(input.x < 0)
        {
            MovePrevious();
        }
    }
    public void OnConfirm(InputValue value)
    {
        if (value.isPressed)
        {
            
            Debug.Log("Selector Confirm at index: " + currentIndex);
            var item = selectables[currentIndex];
            item.Confirm();

            // Lock
            if(item is ICancelable)
            {
            Debug.Log("Selector Locked at index: " + currentIndex);
             isLocked = true;   
            }
        }
    }
    public void OnCancel(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log("Selector Cancel at index: " + currentIndex);
            if (selectables[currentIndex] is ICancelable cancelable)
            {
                cancelable.Cancel();
                isLocked = false;
            }
        }
    }

    public void MoveNext()
    {
        currentIndex = (currentIndex + 1) % selectables.Count;
        UpdatePosition();
    }
    public void MovePrevious()
    {
        currentIndex = (currentIndex - 1 + selectables.Count) % selectables.Count;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        transform.position = selectables[currentIndex].transform.position;
    }
}
