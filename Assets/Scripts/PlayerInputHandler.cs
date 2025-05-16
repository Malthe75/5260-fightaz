using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private string duck = "Duck";

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction hitAction;
    private InputAction kickAction;
    private InputAction duckAction;

    public Vector2 MoveInput { get; private set; }
    public bool JumpInput { get; private set; }

    public bool HitInput { get; set; }
    public bool KickInput { get; set; }

    public bool DuckInput { get; private set; }

    public static PlayerInputHandler Instance { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        jumpAction = playerControls.FindActionMap(actionMapName).FindAction(jump);
        duckAction = playerControls.FindActionMap(actionMapName).FindAction(duck);
        hitAction = playerControls.FindActionMap(actionMapName).FindAction(hit);
        kickAction = playerControls.FindActionMap(actionMapName).FindAction(kick);

        RegisterInputActions();
        PrintDevices();
    }

    void PrintDevices()
    {
        foreach (var device in InputSystem.devices)
        {
            if (device.enabled)
            {
                Debug.Log("Active Device: " + device.name);
            }
        }
    }
    void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;
        jumpAction.performed += context => JumpInput = true;
        jumpAction.canceled += context => JumpInput = false;
        duckAction.performed += context => DuckInput = true;
        duckAction.canceled += context => DuckInput = false;

        //hitAction.canceled += context => HitInput = false;
        //kickAction.canceled += context => KickInput = false;
        //hitAction.performed += ContextMenu => HitInput = true;
        //kickAction.performed += context => KickInput = true;
        hitAction.performed += OnHitPerformed;
        kickAction.performed += OnKickPerformed;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        hitAction.Enable();
        kickAction.Enable();
        duckAction.Enable();


        InputSystem.onDeviceChange += OnDeviceChange;
    }


    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        hitAction.Disable();
        kickAction.Disable();
        duckAction.Disable();

        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnHitPerformed(InputAction.CallbackContext context)
    {
        Debug.Log($"Hit performed! Interaction: {context.interaction?.GetType().Name}");
        HitInput = true;
    }
    private void OnKickPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Kick Button Pressed Once");
        KickInput = true;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Disconnected:
                Debug.Log($"Device Disconnected: {device.displayName}");
                // HANDLE DISCONNECTION LOGIC HERE
                break;
            case InputDeviceChange.Reconnected:
                Debug.Log($"Device Reconnected: {device.displayName}");
                // HANDLE RECONNECTION LOGIC HERE
                break;
        }
    }
}
