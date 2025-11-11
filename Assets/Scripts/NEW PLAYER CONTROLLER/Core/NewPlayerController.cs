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
    public float pushDistance = 1f;
    private Collider2D pushbox;

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

    private int playerLayerMask;
    #endregion
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        playerLayerMask = LayerMask.GetMask("Player");

        // Initialize state machine
        stateMachine = new StateMachine();

        if (stateMachine.CurrentState == null)
        {

            stateMachine.ChangeState(new FallState(this));
        }
        // Start with IdleState

    }

    private void Start()
    {
        if (gameObject.tag == "Player1")
        {
            GameObject enemy = GameObject.FindGameObjectWithTag("Player2");
            this.enemy = enemy.GetComponent<NewPlayerController>();
        }
        else if (gameObject.tag == "Player2")
        {
            GameObject enemy = GameObject.FindGameObjectWithTag("Player1");
            this.enemy = enemy.GetComponent<NewPlayerController>();

        }
        pushbox = body.GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (stateMachine != null)
            stateMachine.CurrentState?.FixedUpdate();
    }
    private void Update()
    {
        // Update the current state. THe if statement is only there to avoid errors when recompiling.
        if (stateMachine != null)
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
            if (Enum.TryParse(actionName, ignoreCase: true, out MoveInput moveInput))
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



    // THIS IS FOR PHYSICS PUSHING PLAYERS:

    public Vector2 PushboxCalculator(Vector2 desiredMove)
    {
        // Raycast from the player origin
        Vector2 rayOrigin = (Vector2)body.transform.position;


        // Raycast in the direction of the of the enemy
        Vector2 enemyPosition = (Vector2)enemy.body.transform.position;
        Vector2 rayDirection = (enemyPosition - rayOrigin).normalized;

        // Ray length changed the number 1 to anything for longer length
        float rayLength = Mathf.Abs(desiredMove.x) + 1f;

        // Draw ray the length of the ray
        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);

        // Raycasthits
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, rayDirection, rayLength, playerLayerMask);

        // Foreach loop to iterate all raycasts and only getting the ones that hit the enemy player.
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider != pushbox)
            {
                PushPlayer();
                bool blockedRight = rayDirection.x > 0;
                bool blockedLeft = rayDirection.x < 0;

                if (blockedRight && desiredMove.x > 0f)
                    desiredMove.x = 0f;          // stop rightward motion
                else if (blockedLeft && desiredMove.x < 0f)
                    desiredMove.x = 0f;          // stop leftward motion

                Debug.DrawRay(hit.point, Vector2.up * 0.5f, Color.green);
                
            }   
        }
        return desiredMove;
    }


    private void PushPlayer()
    {
     
        float dist = Vector2.Distance(body.transform.position, enemy.transform.position);


        if (dist > pushDistance)
        {
            // Calculate push direction (away from each other)
            Vector2 pushDir = (body.transform.position - enemy.body.transform.position).normalized;

            // Apply a *tiny* push to both players
            rb.MovePosition(rb.position + pushDir * 0.02f);
            enemy.rb.MovePosition(enemy.rb.position - pushDir * 0.02f);
        }
    }

}