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
    public float jumpForce = 15f;
    public AudioClip[] jumpSounds;
    [HideInInspector] public bool shouldJump = false;
    [HideInInspector] public bool isGrounded = true;

    [Header("Fall state")]
    public Sprite fallSprite;
    //[HideInInspector] public bool 

    [Header("Hurt state")]
    public AudioClip[] hurtSounds;



    [Header("Physics")]
    public LayerMask layerMask;

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

    //    void OnDrawGizmosSelected()
    //{
    //    if (capsule == null) return;

    //    Gizmos.color = Color.green;

    //    // Use the already calculated capsule size and center (from AdjustForCollisions)
    //    Vector2 size = capsuleSize;          // don’t scale again
    //    Vector2 pos = capsuleCenter;

    //    // Draw rectangle approximation
    //    Gizmos.DrawWireCube(pos, size);

    //    // Optional: draw small circles at top/bottom to better approximate the capsule ends
    //    if (capsule.direction == CapsuleDirection2D.Vertical)
    //    {
    //        float radius = size.x / 2f;
    //        Vector2 top = pos + Vector2.up * (size.y / 2f - radius);
    //        Vector2 bottom = pos - Vector2.up * (size.y / 2f - radius);
    //        Gizmos.DrawWireSphere(top, radius);
    //        Gizmos.DrawWireSphere(bottom, radius);
    //    }
    //    else
    //    {
    //        float radius = size.y / 2f;
    //        Vector2 left = pos - Vector2.right * (size.x / 2f - radius);
    //        Vector2 right = pos + Vector2.right * (size.x / 2f - radius);
    //        Gizmos.DrawWireSphere(left, radius);
    //        Gizmos.DrawWireSphere(right, radius);
    //    }
    //}

    //Vector2 capsuleSize;
    //Vector2 capsuleCenter;
    //Vector2 AdjustForCollisions(Vector2 position, Vector2 move, LayerMask mask)
    //{
    //    // Getting size of collider and its center and using it for the overlapping.
    //    capsuleSize = Vector2.Scale(capsule.size, transform.localScale);
    //    capsuleCenter = rb.position + Vector2.Scale(capsule.offset, transform.localScale);

    //    Collider2D hit = Physics2D.OverlapCapsule(capsuleCenter, capsuleSize, capsule.direction, 0f, layerMask);
    //    if (hit != null && hit.gameObject != gameObject)
    //    {
    //        Debug.Log("hitting");
    //        Debug.Log(hit);
    //    }
    //    return Vector2.zero;
    //}

    private void FixedUpdate()
    {
        //Vector2 da = AdjustForCollisions(transform.position, moveInput, enemyLayer);
        // Physics and movement updates
        //isGrounded = CheckGrounded();
        //Vector2 desiredMove = stateMachine.CurrentState.GetDesiredMovement();
        //rb.MovePosition(rb.position + desiredMove);


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





    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enter");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player1") || collision.CompareTag("Player2"))
        {
            Vector2 dir = (transform.position - collision.transform.position).normalized;
            transform.position += (Vector3)dir * 0.02f; // small push apart
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("exit");
    }

    //public GameObject floor;
    public float floorY = -3;
    public float gravity = -20f;
    public float verticalVelocity = 0f;
    public Vector2 velocity;
    public float skin = 0.01f;

    public bool IsGrounded()
    {
        if (transform.position.y > floorY)
        {
            // Apply gravity
            return false;
        }
        else
        {
            return true;
        }
    }

    public Transform feet;
    public Transform body; 
    
   
//    public void GroundCheck()
//    {
//        Debug.Log(GetCapsuleBottomY());

//        // The velocity?
//        verticalVelocity += gravity * Time.fixedDeltaTime;

//        // Figure out where it will land in the next frame?
//        float playerBottomY = GetCapsuleBottomY();
//        float nextY = playerBottomY + verticalVelocity * Time.fixedDeltaTime;

//        float centerY = GetCenterYFromBottomY(nextY);
//        rb.MovePosition(new Vector2(rb.position.x, centerY));

//        Debug.DrawLine(
//    new Vector2(rb.position.x - 0.5f, floorY),
//    new Vector2(rb.position.x + 0.5f, floorY),
//    Color.yellow
//);

    //    float bottom = GetCapsuleBottomY();
    //    Debug.DrawLine(
    //        new Vector2(rb.position.x - 0.5f, bottom),
    //        new Vector2(rb.position.x + 0.5f, bottom),
    //        Color.cyan
    //    );
    //    if (nextY < floorY)
    //    {
    //        nextY = floorY;
    //        Debug.Log("Under floor next time");
    //        verticalVelocity = 0f;
    //    }
    //     ApplyGravity(nextY);
    //}

}
