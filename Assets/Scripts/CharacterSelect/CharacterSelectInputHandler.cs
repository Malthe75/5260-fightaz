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
        if (context.performed || context.canceled)
        {
            OnMove?.Invoke(context.ReadValue<Vector2>());
        }
    }

    public void OnConfirmInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnConfirm?.Invoke();
        }
    }
}
