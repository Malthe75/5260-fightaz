using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class CharacterSelectInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionAsset playerControls;
    [SerializeField] private string actionMapName = "CharacterSelection";
    [SerializeField] private string move = "Move";
    [SerializeField] private string confirm = "Confirm";

    public event Action<Vector2> OnMove;
    public event Action OnConfirm;

    private InputAction moveAction, confirmAction;

    private void Awake()
    {
        var map = playerControls.FindActionMap(actionMapName);

        moveAction = map.FindAction(move);
        confirmAction = map.FindAction(confirm);

        moveAction.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        moveAction.canceled += ctx => OnMove?.Invoke(Vector2.zero);
        confirmAction.performed += ctx => OnConfirm?.Invoke();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        confirmAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        confirmAction.Disable();
    }
}
