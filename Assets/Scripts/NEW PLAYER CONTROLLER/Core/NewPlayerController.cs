using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;

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
    public float jumpForce = 25f;
    public AudioClip[] jumpSounds;
    [HideInInspector] public bool shouldJump = false;
    [HideInInspector] public bool isGrounded = true;

    [Header("Fall state")]
    public Sprite fallSprite;
    public AudioClip[] fallSounds;
    public float gravity = -65f;
    public float floorY = -3;
    public float verticalVelocity = 0f;
    public Vector2 velocity; 

    [Header("Hurt state")]
    public AudioClip[] hurtSounds;



    [Header("Physics")]
    public LayerMask layerMask;
    public Transform feet;
    public Transform body;

    [Header("JumpAttack state")]


    // References
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public SpriteRenderer sr;
    //[HideInInspector] public CapsuleCollider2D capsule;
    //[HideInInspector] public AttackHitbox attackHitbox;

    // Input
    [HideInInspector] public Vector2 moveInput;

    // State Machine
    public StateMachine stateMachine;

    public AttackHitbox attackHitbox;

    //public GameObject enemy
    public NewPlayerController enemy;

    private LayerMask enemyLayer;
    #endregion
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        //capsule = GetComponentInChildren<CapsuleCollider2D>();
        // Initialize state machine
        stateMachine = new StateMachine();

        if(stateMachine.CurrentState == null)
        {

            stateMachine.ChangeState(new FallState(this));
        }
        // Start with IdleState

    }

    private void Start()
    {
        if(gameObject.tag == "Player1")
        {
            GameObject enemy = GameObject.FindGameObjectWithTag("Player2");
            this.enemy = enemy.GetComponent<NewPlayerController>();
        }else if(gameObject.tag == "Player2")
        {
            GameObject enemy = GameObject.FindGameObjectWithTag("Player1");
            this.enemy = enemy.GetComponent<NewPlayerController>();
        }
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

    public LayerMask blockingLayers; // Assign in inspector (e.g. "Ground", "Walls", "Player")
    public LayerMask groundBlockLayer;
    public Vector2 feetOffset = new Vector2(0f, -0.5f); // Adjust based on player's feet position
    public Vector2 feetBoxSize = new Vector2(0.5f, 0.1f); // Width and height of the box for ground check
    

}
