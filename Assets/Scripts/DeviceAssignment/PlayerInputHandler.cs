using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name References")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string hit = "Hit";
    [SerializeField] private string kick = "Kick";
    [SerializeField] private string crouch = "Crouch";
    [SerializeField] private string shoot = "Shoot";
    [SerializeField] private string taunt = "Taunt";
    [SerializeField] private string signature = "Signature1";
    [SerializeField] private string signature2 = "Signature2";

    public static PlayerInputHandler Instance { get; private set; }

    public event Action OnJump;
    public event Action OnHit;
    public event Action OnKick;
    public event Action OnShoot;
    public event Action OnTaunt;
    public event Action OnSignature1;
    public event Action OnSignature2;
    public event Action<bool> OnCrouchChanged;
    public event Action<Vector2> OnMove;
    public event Action OnCross, OnCircle, OnSquare, OnTriangle;
    public event Action OnMenuConfirm, OnMenuCancel;
    public event Action<Vector2> OnMenuMove;


    private InputAction moveAction, jumpAction, hitAction, kickAction, crouchAction, shootAction, tauntAction, signature1Action, signature2Action;
    private InputAction clashCrossAction, clashCircleAction, clashSquareAction, clashTriangleAction;
    private InputAction menuMoveAction, menuConfirmAction, menuCancelAction;
    [SerializeField] private PlayerInputHandler inputHandler;

    private void Awake()
    {
        var input = GetComponent<PlayerInput>();
        var map = input.actions.FindActionMap("Player");
        moveAction = map.FindAction(move);
        jumpAction = map.FindAction(jump);
        hitAction = map.FindAction(hit);
        kickAction = map.FindAction(kick);
        crouchAction = map.FindAction(crouch);
        shootAction = map.FindAction(shoot);
        tauntAction = map.FindAction(taunt);
        signature1Action = map.FindAction(signature);
        signature2Action = map.FindAction(signature2);

        RegisterInputActions();

        RegisterClashInputActions(input);
        RegisterMenuInputActions(input);
    }

    private void RegisterInputActions()
    {
        moveAction.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        moveAction.canceled += ctx => OnMove?.Invoke(Vector2.zero);

        crouchAction.performed += ctx => OnCrouchChanged?.Invoke(true);
        crouchAction.canceled += ctx => OnCrouchChanged?.Invoke(false);

        jumpAction.performed += ctx => OnJump?.Invoke();
        hitAction.performed += ctx => OnHit?.Invoke();
        kickAction.performed += ctx => OnKick?.Invoke();
        shootAction.performed += ctx => OnShoot?.Invoke();
        tauntAction.performed += ctx => OnTaunt?.Invoke();
        signature1Action.performed += ctx => OnSignature1?.Invoke();
        signature2Action.performed += ctx => OnSignature2?.Invoke();
    }

    private void RegisterClashInputActions(PlayerInput input)
    {
        var clashMap = input.actions.FindActionMap("Clash");

        clashCrossAction = clashMap.FindAction("Cross");
        clashCircleAction = clashMap.FindAction("Circle");
        clashSquareAction = clashMap.FindAction("Square");
        clashTriangleAction = clashMap.FindAction("Triangle");

        GameObject clashObject = GameObject.FindWithTag("ClashEffect");

        clashCrossAction.performed += ctx => OnCross?.Invoke();
        clashCircleAction.performed += ctx => OnCircle?.Invoke();
        clashSquareAction.performed += ctx => OnSquare?.Invoke();
        clashTriangleAction.performed += ctx => OnTriangle?.Invoke();
    }

    private void RegisterMenuInputActions(PlayerInput input)
    {
        var menuMap = input.actions.FindActionMap("Menu");

        menuMoveAction = menuMap.FindAction("Move");
        menuConfirmAction = menuMap.FindAction("Confirm");
        menuCancelAction = menuMap.FindAction("Cancel");

        menuMoveAction.performed += ctx => OnMenuMove?.Invoke(ctx.ReadValue<Vector2>());
        menuConfirmAction.performed += ctx => OnMenuConfirm?.Invoke();
        menuCancelAction.performed += ctx => OnMenuCancel?.Invoke(); 

    }

    public void SwitchCurrentActionMap(string actionMap)
    {
        var input = GetComponent<PlayerInput>();
        input.SwitchCurrentActionMap(actionMap);
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        hitAction.Enable();
        kickAction.Enable();
        crouchAction.Enable();
        shootAction.Enable();
        tauntAction.Enable();
        signature1Action.Enable();
        signature2Action.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        hitAction.Disable();
        kickAction.Disable();
        crouchAction.Disable();
        shootAction.Disable();
        tauntAction.Disable();
        signature1Action.Disable();
        signature2Action.Disable();
    }
}

