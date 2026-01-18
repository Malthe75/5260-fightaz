using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(PlayerInput))]
public class CharacterSelectInputHandler : MonoBehaviour
{
    public event Action<Vector2> OnMove;
    public event Action OnConfirm;

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Move input detected: " + context.ReadValue<Vector2>());
            OnMove?.Invoke(context.ReadValue<Vector2>());
        }
    }

    public void OnConfirmInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("What?");
            OnConfirm?.Invoke();
        }
    }
}
