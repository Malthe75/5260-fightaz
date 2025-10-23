using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public List<AttackData> attackData;

    [Header("Block state")]
    public Sprite[] blockSprites;


    // References
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public SpriteRenderer sr;
    //[HideInInspector] public AttackHitbox attackHitbox;

    // Input
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public bool jumpInput;

    // State Machine
    public StateMachine stateMachine;

    public AttackHitbox attackHitbox;


    #endregion

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


        // Decrement combo cooldown timer
       

        // Update the current state. THe if statement is only there to avoid errors when recompiling.
        if(stateMachine != null)
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

    public void OnBlock(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Button pressed down  enter block state
            stateMachine.ChangeState(new BlockState(this));
        }
        else if (context.canceled)
        {
            // Button released  exit block state (e.g. back to idle)
            if (stateMachine.CurrentState is BlockState)
                stateMachine.ChangeState(new IdleState(this));
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        string actionName = context.action.name;
        Debug.Log(actionName);
        switch (actionName)
        {
            case "Hit":
                HandleAttackInput(AttackInput.Hit);
                break;
            case "Kick":
                HandleAttackInput(AttackInput.Kick);
                break;
            case "Shoot":
                HandleAttackInput(AttackInput.Shoot);
                break;
            default:
                Debug.LogWarning("Unknown attack action: " + actionName);
                break;
        }
    }


    private void HandleAttackInput(AttackInput input)
    {
        stateMachine.ChangeState(new AttackState(this, input));
    }

    #endregion


    public void TakeHit(int damage)
    {
        Debug.Log("IT did this damage");
        stateMachine.ChangeState(new HurtState(this));
    }
}
