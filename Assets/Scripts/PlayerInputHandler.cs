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

    private InputAction moveAction;
    private InputAction jumpAction;
    
    public Vector2 MoveInput { get; private set; }
    public bool JumpInput { get; private set; }

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
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();

        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();

        InputSystem.onDeviceChange -= OnDeviceChange;
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
