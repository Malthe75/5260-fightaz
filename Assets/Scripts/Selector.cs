using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Selector : MonoBehaviour
{
    [SerializeField] private List<Transform> selectables;
    private int currentIndex = 0;

    public void OnMenuMove(InputAction.CallbackContext context)
    {
        Debug.Log("Selector Move Input: " + context.ReadValue<Vector2>());
    }
    public void OnMenuConfirm(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Selector Confirm Input");
        }
    }
    public void OnMenuCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Selector Cancel Input");
        }
    }


    public void MoveNext()
    {
        currentIndex = (currentIndex - 1 + selectables.Count) % selectables.Count;
        UpdatePosition();
    }
    public void MovePrevious()
    {
        currentIndex = (currentIndex + 1) % selectables.Count;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        transform.position = selectables[currentIndex].position;
    }
}
