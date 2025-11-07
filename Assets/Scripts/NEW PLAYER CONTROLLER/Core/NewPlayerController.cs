using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerController : MonoBehaviour
{

    [HideInInspector] public float horizontalMultiplier = 0f;
    [Header("Move Map")]
    public MoveMap moveMap;

    #region State variables
    [Header("Idle state")]
    public Sprite[] idleSprites;

    [Header("Walk state")]
    public Sprite[] walkSprites;
    public float walkSpeed = 5f;
    public float animationSpeed = 0.2f;


    [Header("Attack state")]
    public List<AttackData> attackData;
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
        // Physics and movement updates
        isGrounded = CheckGrounded();
        Vector2 desiredMove = stateMachine.CurrentState.GetDesiredMovement();
        Vector2 finalMove = StopMovementForCollsions(desiredMove);
        rb.MovePosition(rb.position + finalMove);

        if (isGrounded)
        {
            gravity = 0f;
        }

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

    public LayerMask blockingLayers; // Assign in inspector (e.g. "Ground", "Walls", "Player")
    public LayerMask groundBlockLayer;
    public Vector2 feetOffset = new Vector2(0f, -0.5f); // Adjust based on player's feet position
    public Vector2 feetBoxSize = new Vector2(0.5f, 0.1f); // Width and height of the box for ground check


    public bool CheckGrounded()
    {
        // Position slightly below the player's feet
        Vector2 feetPos = (Vector2)rb.position + feetOffset;
        Vector2 boxSize = feetBoxSize;

        // OverlapBox for ground detection
        RaycastHit2D hit = Physics2D.BoxCast(rb.position + feetOffset, feetBoxSize, 0f, Vector2.down, 0.01f, groundBlockLayer);
        return hit.collider != null;
    }

    void OnDrawGizmosSelected()
    {
        if (rb == null) return;
        Gizmos.color = Color.green;
        Vector2 feetPos = (Vector2)rb.position + feetOffset;
        Gizmos.DrawWireCube(feetPos, feetBoxSize);
    }

    public Vector2 StopMovementForCollsions(Vector2 desiredMove)
    {
        if (desiredMove.sqrMagnitude < 0.0001f)
        {
            horizontalMultiplier = 1f;
            return desiredMove; // Nothing to do
        }

        // Prepare cast
        Vector2 moveDir = desiredMove.normalized;
        float moveDistance = desiredMove.magnitude + 0.001f; // tiny "skin" to prevent sticking
        RaycastHit2D[] hits = new RaycastHit2D[8];

        // Setup the contact filter
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(blockingLayers); // only collide with blocking layers
        filter.useTriggers = false;          // ignore triggers like hitboxes/hurtboxes

        int hitCount = rb.Cast(moveDir, filter, hits, moveDistance);

        if (hitCount > 0)
        {
            // Something in the way — block horizontal movement
            desiredMove.x = 0f;
            horizontalMultiplier = 0f;
        }
        else
        {
            horizontalMultiplier = 1f;
        }
            return desiredMove;
    }
}
