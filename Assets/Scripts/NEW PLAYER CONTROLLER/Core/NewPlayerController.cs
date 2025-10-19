using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerController : MonoBehaviour
{

    #region State variables
    [Header("Idle state")]
    public Sprite[] idleSprites;

    [Header("Walk state")]
    public Sprite[] walkSprites;
    public float walkSpeed = 5f;
    public float animationSpeed = 0.2f;

    [Header("Attack state")]
    public Sprite[] attackSprites; // SHOULD BE DELETED
    public ComboLibrary comboLibrary;

    #endregion
    // References
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public SpriteRenderer sr;

    // Input
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public bool jumpInput;

    // State Machine
    public StateMachine stateMachine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        // Initialize state machine
        stateMachine = new StateMachine();

        // Start with IdleState
        stateMachine.ChangeState(new IdleState(this));
    }

    private void Update()
    {
        // Update input
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);

        // Update the current state
        stateMachine.Update();
    }


    #region Input Actions
    // Input system.

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        stateMachine.CurrentState?.OnMove(input);
        //stateMachine.ChangeState(new WalkState(this));
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        string actionName = context.action.name;

        switch (actionName)
        {
            case "Hit":
                HandleAttackInput(AttackInput.Hit);
                break;
            case "Kick":
                HandleAttackInput(AttackInput.Kick);
                break;
            default:
                Debug.LogWarning("Unknown attack action: " + actionName);
                break;
        }
    }



    // Handle the attack inputs
    private void HandleAttackInput(AttackInput input)
    {
        stateMachine.ChangeState(new AttackState(this, input));
    }
    #endregion

}
