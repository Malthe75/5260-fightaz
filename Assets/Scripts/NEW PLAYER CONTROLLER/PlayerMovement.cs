using System;
using UnityEngine;

enum MovementState
{
    Idle,
    Walking,
    Jumping,
    Falling,
}
public class PlayerMovement : MonoBehaviour
{

    // Other stuff
    private Rigidbody2D rb;
    [SerializeField] private Transform body;
    private Collider2D pushbox;
    private PlayerMovement enemy;
    private MovementState currentMovement = MovementState.Idle;

    private Vector2 proposedMovement;

    public float minX = -5f;
    public float maxX = 5f;
    public float minY = 0f;
    public float gravity = 65f;
    public bool hasLanded = false;

    private float xVelocity;
    private float speed;
    private float yVelocity;

    void FixedUpdate()
    {
        switch (currentMovement)
        {
            case MovementState.Idle:
                break;
            case MovementState.Walking:
                proposedMovement = HandleMove();
                ApplyPhysics();
                break;
            case MovementState.Jumping:
                proposedMovement = HandleJump();
                ApplyPhysics();
                if (IsGrounded() && yVelocity <= 0f) // landed
                {
                    yVelocity = 0f;
                    hasLanded = true;
                    currentMovement = MovementState.Idle;
                }

                break;
            case MovementState.Falling:
                // Falling logic
                break;
        }

    }

    void ApplyPhysics()
    {
        Vector2 clampedNextPos = ClampedMovement(proposedMovement); // absolute next pos
        Vector2 desiredDisplacement = clampedNextPos - rb.position; // displacement for this tick

        // PushboxCalculator(desiredDisplacement) -> allowed displacement
        Vector2 allowedDisplacement = PushboxCalculator(desiredDisplacement);

        // Move to current position + allowed displacement (absolute position)
        Vector2 finalPos = rb.position + allowedDisplacement;

        Debug.Log("Final Position: " + finalPos);
        rb.MovePosition(finalPos);
    }


    // This class is 
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pushbox = body.GetComponent<Collider2D>();
    }
    private void Start()
    {
        SetEnemy();
    }

    public void SetIdle()
    {
        currentMovement = MovementState.Idle;
    }
    public void SetMove(float xVelocity, float speed)
    {
        this.xVelocity = xVelocity;
        this.speed = speed;
        currentMovement = MovementState.Walking;
    }

    public void SetJump(float xVelocity, float jumpForce)
    {
        this.xVelocity = xVelocity;
        this.yVelocity = jumpForce;
        currentMovement = MovementState.Jumping;
    }

    private Vector2 HandleMove()
    {
        Vector2 desiredMove = new Vector2(xVelocity * speed, 0f) * Time.fixedDeltaTime;
        return desiredMove;
    }

    public Vector2 HandleJump()
    {
        float xMovement = xVelocity * Time.fixedDeltaTime;
        yVelocity -= gravity * Time.fixedDeltaTime;
        float yMovement = yVelocity * Time.fixedDeltaTime;

        Vector2 movement = new Vector2(xMovement, yMovement);
        return movement;
    }

    private bool IsGrounded()
    {
        if (rb.position.y <= minY + 0.01f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }




    public void Fall()
    {
        // Fall logic
    }















    public void PushPlayer(Vector2 force)
    {
        // Push player logic
    }

    // public Vector2 PushboxCalculator(Vector2 movement)
    // {
    //     Vector2 rayOrigin = (Vector2)body.transform.position;
    // }
























    public Vector2 PushboxCalculator(Vector2 desiredMove)
    {
        // Raycast from the player origin
        // Vector2 rayOrigin = (Vector2)body.transform.position;


        // // Raycast in the direction of the of the enemy
        // Vector2 enemyPosition = (Vector2)enemy.body.transform.position;
        // Vector2 rayDirection = (enemyPosition - rayOrigin).normalized;

        // // Ray length changed the number 1 to anything for longer length
        // float rayLength = Mathf.Abs(desiredMove.x) + 1f;

        // // Draw ray the length of the ray
        // Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);

        // Raycasthits
        //RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, rayDirection, rayLength, playerLayerMask);

        // Foreach loop to iterate all raycasts and only getting the ones that hit the enemy player.
        // foreach (RaycastHit2D hit in hits)
        // {
        //     if (hit.collider != null && hit.collider != pushbox)
        //     {
        //         //PushPlayer();
        //         bool blockedRight = rayDirection.x > 0;
        //         bool blockedLeft = rayDirection.x < 0;

        //         if (blockedRight && desiredMove.x > 0f)
        //             desiredMove.x = 0f;          // stop rightward motion
        //         else if (blockedLeft && desiredMove.x < 0f)
        //             desiredMove.x = 0f;          // stop leftward motion

        //         Debug.DrawRay(hit.point, Vector2.up * 0.5f, Color.green);

        //     }   
        // }
        return desiredMove;
    }
    public Vector2 ClampedMovement(Vector2 desiredMove)
    {
        Vector2 nextPos = rb.position + desiredMove;

        nextPos.x = Mathf.Clamp(nextPos.x, minX, maxX);
        nextPos.y = Mathf.Max(nextPos.y, minY);
        return nextPos;
    }







    #region OTHER FUNCS
    private void SetEnemy()
    {
        if (transform.CompareTag("Player1"))
        {
            enemy = GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerMovement>();
        }
        else
        {
            enemy = GameObject.FindGameObjectWithTag("Player1").GetComponent<PlayerMovement>();
        }
    }
    #endregion
}
