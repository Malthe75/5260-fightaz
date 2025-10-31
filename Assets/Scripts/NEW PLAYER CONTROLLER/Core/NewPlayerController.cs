using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerController : MonoBehaviour
{
    [Header("Move Map")]
    public MoveMap moveMap;

    #region State variables
    [Header("Idle state")]
    public Sprite[] idleSprites;

    [Header("Walk state")]
    public Sprite[] walkSprites;
    public float walkSpeed = 5f;
    public float animationSpeed = 0.2f;
    public Vector2 blockedDirection;

    [Header("Attack state")]
    public List<AttackData> attackData;
    //[HideInInspector] public AttackInput input = AttackInput.Nothing;
    [HideInInspector] public MoveInput input = MoveInput.Nothing;

    [Header("Block state")]
    public Sprite[] blockSprites;
    [HideInInspector] public bool isBlocking = false;

    [Header("Jump state")]
    public Sprite[] jumpSprites;
    [HideInInspector] public bool shouldJump = false;
    [HideInInspector] public bool isGrounded = true;

    [Header("JumpAttack state")]


    // References
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public SpriteRenderer sr;
    //[HideInInspector] public AttackHitbox attackHitbox;

    // Input
    [HideInInspector] public Vector2 moveInput;

    // State Machine
    public StateMachine stateMachine;

    public AttackHitbox attackHitbox;

    public float gravity = -9.81f;

    #endregion
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        // Initialize state machine
        stateMachine = new StateMachine();

        if(stateMachine.CurrentState == null)
        {

            stateMachine.ChangeState(new IdleState(this));
        }
        // Start with IdleState

    }

    private void FixedUpdate()
    {
        if (stateMachine != null)
            stateMachine.CurrentState?.FixedUpdate();
    }
    private void Update()
    {
        // Update the current state. THe if statement is only there to avoid errors when recompiling.
        if(stateMachine != null)
            stateMachine.Update();
        input = MoveInput.Nothing;
    }


    #region Input Actions
    // Input system.

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveInput = input;
    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isBlocking = true;
        }
        else if (context.canceled)
        {
            isBlocking = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            shouldJump = true;
        }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (context.performed)
        {

            string actionName = context.action.name;
            if(Enum.TryParse(actionName, ignoreCase: true, out MoveInput moveInput))
            {
                this.input = moveInput;
                //moveMap.GetAttack(moveInput);
            }
            else
            {
                Debug.LogWarning($"Unknown action: {actionName}");
            }
            
        }
    }

    #endregion


    public void TakeHit(int damage, AttackFrameData attack)
    {
        Debug.Log("IT did this damage");
        stateMachine.ChangeState(new HurtState(this, attack));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Collions against ground.
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            gravity = 0f;
        }

        // Collisions against players.
        if (collision.gameObject.CompareTag("Player1") || collision.gameObject.CompareTag("Player2"))
        {
            Vector2 contactNormal = collision.contacts[0].normal;

            // Horizontal collision
            if (contactNormal.x > 0.1f) blockedDirection.x = -1;   // Something on right
            else if (contactNormal.x < -0.1f) blockedDirection.x = 1; // Something on left

            // Optional: vertical collision
            //if (contactNormal.y > 0.1f) blockedDirection.y = -1;   // Something above
            //else if (contactNormal.y < -0.1f) blockedDirection.y = 1; // Something below
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Collions against ground.
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }

        // Collisions against players.
        if (collision.gameObject.CompareTag("Player1") || collision.gameObject.CompareTag("Player2"))
        {
            blockedDirection = Vector2.zero; // Reset blocked direction
        }
    }



}
