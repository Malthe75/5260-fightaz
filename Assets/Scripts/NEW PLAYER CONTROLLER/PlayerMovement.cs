using System;
using UnityEngine;

public enum MovementState
{
    Idle,
    Walking,
    Jumping,
    Falling,
    YDashing,
    Knockup,
}
public class PlayerMovement : MonoBehaviour
{

    // Other stuff
    private Rigidbody2D rb;
    [SerializeField] private Transform body;
    private Collider2D pushbox;
    private MovementState currentMovement;
    private int playerLayerMask;

    private Vector2 proposedMovement;

    public float minX = -5f;
    public float maxX = 5f;
    public float minY = 0f;
    public float gravity = 65f;
    public bool hasLanded = true;
    private float gravityMultiplier = 1;

    private float xVelocity;
    private float speed;
    private float yVelocity;


    void FixedUpdate()
    {
        switch (currentMovement)
        {
            case MovementState.Idle:
                proposedMovement = Vector2.zero;
                break;
            case MovementState.Walking:
                proposedMovement = HandleMove();
                break;
            case MovementState.Jumping:
            case MovementState.Falling:
            case MovementState.YDashing:
            case MovementState.Knockup:
                // All these cases leads to handleJump
                proposedMovement = HandleJump();
                break;
        }
        ApplyPhysics();

    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pushbox = body.GetComponent<Collider2D>();
        playerLayerMask = LayerMask.GetMask("Player");

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
        this.gravityMultiplier = 1f;
        currentMovement = MovementState.Jumping;
    }

    public void SetFall(float xVelocity, float yVelocity)
    {
        this.xVelocity = xVelocity;
        this.yVelocity = yVelocity;
        this.gravityMultiplier = 1f;
        this.currentMovement = MovementState.Falling;
    }

    public void SetYDash(float force, float gravityMultiplier)
    {
        yVelocity = force;
        this.gravityMultiplier = gravityMultiplier;
        currentMovement = MovementState.YDashing;
    }

    public void SetKnockup(float xMovement, float yMovement, int facing)
    {
        this.xVelocity = xMovement * -facing;
        this.yVelocity = yMovement;
        this.gravityMultiplier = 1f;
        currentMovement = MovementState.Knockup;
    }

    private Vector2 HandleMove()
    {
        Vector2 desiredMove = new Vector2(xVelocity * speed, 0f) * Time.fixedDeltaTime;
        return desiredMove;
    }

    public Vector2 HandleJump()
    {
        // LANDING LOGIC ----------
        if (IsGrounded() && yVelocity <= 0f) // landed
        {
            yVelocity = 0f;
            xVelocity = 0f;
            hasLanded = true;
            currentMovement = MovementState.Idle;
            return Vector2.zero;
        }

        // APPLY MOVEMENT AND GRAVITY---
        yVelocity -= gravity * gravityMultiplier * Time.fixedDeltaTime;

        float xMovement = xVelocity * Time.fixedDeltaTime;
        float yMovement = yVelocity * Time.fixedDeltaTime;
        return new Vector2(xMovement, yMovement);
    }

    public float CalculateJumpTime(float jumpForce)
    {
        return (2 * jumpForce) / gravity;
    }

    public float CalculateAirTime(float jumpForce, float gravityMultiplier)
    {
        return (2 * jumpForce) / (gravity * gravityMultiplier);
    }

    public bool IsGrounded()
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

    public JumpInput GetJumpInput(float moveInput)
    {
        if (moveInput > 0f) return JumpInput.Right;
        else if (moveInput < 0f) return JumpInput.Left;
        else return JumpInput.Up;
    }



    #region Physic constraints


    void ApplyPhysics()
    {
        Vector2 clampedNextPos = ClampedMovement(proposedMovement); // absolute next pos
        Vector2 desiredDisplacement = clampedNextPos - rb.position; // displacement for this tick

        // PushboxCalculator(desiredDisplacement) -> allowed displacement
        Vector2 allowedDisplacement = PushboxCalculator(desiredDisplacement);

        // Move to current position + allowed displacement (absolute position)
        Vector2 finalPos = rb.position + allowedDisplacement;

        rb.MovePosition(finalPos);
    }

    public Vector2 PushboxCalculator(Vector2 desiredMove)
    {
        Vector2 rayOrigin = (Vector2)body.transform.position;
        Vector2 enemyPosition = (Vector2)enemy.body.transform.position;
        Vector2 rayDirection = (enemyPosition - rayOrigin).normalized;

        float rayLength = Mathf.Abs(desiredMove.x) + 1f;
        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);

        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, rayDirection, rayLength, playerLayerMask);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider != pushbox)
            {
                bool blockedRight = rayDirection.x > 0;
                bool blockedLeft = rayDirection.x < 0;

                if (blockedRight && desiredMove.x > 0f)
                    desiredMove.x = 0f;
                else if (blockedLeft && desiredMove.x < 0f)
                    desiredMove.x = 0f;
                Debug.DrawRay(hit.point, Vector2.up * 0.5f, Color.green);

            }
        }
        return desiredMove;
    }


    public Vector2 ClampedMovement(Vector2 desiredMove)
    {
        Vector2 nextPos = rb.position + desiredMove;

        nextPos.x = Mathf.Clamp(nextPos.x, minX, maxX);
        nextPos.y = Mathf.Max(nextPos.y, minY);
        return nextPos;
    }

    #endregion
    #region OTHER FUNCS

    private PlayerMovement enemy;

    public void SetEnemy(PlayerMovement enemy)
    {
        this.enemy = enemy;
    }

    public void SetFacing(int facing)
    {
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * facing; // +1 or -1
        transform.localScale = s;
    }
    #endregion
}
