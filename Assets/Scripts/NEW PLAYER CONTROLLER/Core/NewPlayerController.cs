using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [HideInInspector] public bool shouldJump = false;
    [HideInInspector] public bool isGrounded = true;

    [Header("Fall state")]
    public Sprite fallSprite;
    //[HideInInspector] public bool 


    [Header("Physics")]
    public LayerMask layerMask;

    [Header("JumpAttack state")]


    // References
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public CapsuleCollider2D capsule;
    //[HideInInspector] public AttackHitbox attackHitbox;

    // Input
    [HideInInspector] public Vector2 moveInput;

    // State Machine
    public StateMachine stateMachine;

    public AttackHitbox attackHitbox;



    private LayerMask enemyLayer;
    #endregion
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        capsule = GetComponent<CapsuleCollider2D>();
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
        if (gameObject.layer == LayerMask.NameToLayer("Player1"))
        {
            Debug.Log("Im palyer 1 enemy is player 2");
            enemyLayer = LayerMask.NameToLayer("Player2");
            Debug.Log("Enemy layer name: " + LayerMask.LayerToName(enemyLayer));
        }
        else if (gameObject.layer == LayerMask.NameToLayer("Player2"))
        {
            Debug.Log("Im palyer 2 enemy is player 1");
            enemyLayer = LayerMask.NameToLayer("Player1");
            Debug.Log("Enemy layer name: " + LayerMask.LayerToName(enemyLayer));

        }
    }

        void OnDrawGizmosSelected()
    {
        if (capsule == null) return;

        Gizmos.color = Color.green;

        // Use the already calculated capsule size and center (from AdjustForCollisions)
        Vector2 size = capsuleSize;          // don’t scale again
        Vector2 pos = capsuleCenter;

        // Draw rectangle approximation
        Gizmos.DrawWireCube(pos, size);

        // Optional: draw small circles at top/bottom to better approximate the capsule ends
        if (capsule.direction == CapsuleDirection2D.Vertical)
        {
            float radius = size.x / 2f;
            Vector2 top = pos + Vector2.up * (size.y / 2f - radius);
            Vector2 bottom = pos - Vector2.up * (size.y / 2f - radius);
            Gizmos.DrawWireSphere(top, radius);
            Gizmos.DrawWireSphere(bottom, radius);
        }
        else
        {
            float radius = size.y / 2f;
            Vector2 left = pos - Vector2.right * (size.x / 2f - radius);
            Vector2 right = pos + Vector2.right * (size.x / 2f - radius);
            Gizmos.DrawWireSphere(left, radius);
            Gizmos.DrawWireSphere(right, radius);
        }
    }

    Vector2 capsuleSize;
    Vector2 capsuleCenter;
    Vector2 AdjustForCollisions(Vector2 position, Vector2 move, LayerMask mask)
    {
        // Getting size of collider and its center and using it for the overlapping.
        capsuleSize = Vector2.Scale(capsule.size, transform.localScale);
        capsuleCenter = rb.position + Vector2.Scale(capsule.offset, transform.localScale);

        Collider2D hit = Physics2D.OverlapCapsule(capsuleCenter, capsuleSize, capsule.direction, 0f, layerMask);
        if (hit != null && hit.gameObject != gameObject)
        {
            Debug.Log("hitting");
            Debug.Log(hit);
        }
        return Vector2.zero;
    }

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








    // Capsule stuff
    Vector2 GetCapsuleWorldCenter()
    {
        return rb.position + Vector2.Scale(capsule.offset, transform.localScale);
    }

    float GetCapsuleBottomY()
    {
        // capsuleSize already scaled to world:
        Vector2 capsuleSize = Vector2.Scale(capsule.size, transform.localScale);

        Vector2 center = GetCapsuleWorldCenter();
        if (capsule.direction == CapsuleDirection2D.Vertical)
        {
            float halfHeight = capsuleSize.y * 0.5f;
            return center.y - halfHeight;
        }
        else // Horizontal capsule: width is the long axis
        {
            float halfHeight = capsuleSize.x * 0.5f; // horizontal capsule's "height" along y
            return center.y - halfHeight;
        }
    }

    float GetCenterYFromBottomY(float bottomY)
    {
        Vector2 capsuleSize = Vector2.Scale(capsule.size, transform.localScale);
        float halfHeight = (capsule.direction == CapsuleDirection2D.Vertical)
            ? capsuleSize.y * 0.5f
            : capsuleSize.x * 0.5f;
        return bottomY + halfHeight;
    }


}
