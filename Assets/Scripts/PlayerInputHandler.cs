using UnityEngine;
using UnityEngine.InputSystem;
using System;

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

    private InputAction moveAction, jumpAction, hitAction, kickAction, crouchAction, shootAction, tauntAction, signature1Action, signature2Action;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        var map = playerControls.FindActionMap(actionMapName);

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

