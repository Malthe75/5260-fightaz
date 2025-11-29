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
    private MovementState currentMovement = MovementState.Idle;
    private int playerLayerMask;

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
                proposedMovement = Vector2.zero;
                break;
            case MovementState.Walking:
                Debug.Log("Walking");
                proposedMovement = HandleMove();
                break;
            case MovementState.Jumping:
                proposedMovement = HandleJump();
                if (IsGrounded() && yVelocity <= 0f) // landed
                {
                    yVelocity = 0f;
                    xVelocity = 0f;
                    hasLanded = true;
                    currentMovement = MovementState.Idle;
                }

                break;
            case MovementState.Falling:
                // Falling logic
                break;
        }

        ApplyPhysics();

    }

    void ApplyPhysics()
    {
        Vector2 clampedNextPos = ClampedMovement(proposedMovement); // absolute next pos
        Vector2 desiredDisplacement = clampedNextPos - rb.position; // displacement for this tick

        // PushboxCalculator(desiredDisplacement) -> allowed displacement
        Vector2 allowedDisplacement = PushboxCalculator(desiredDisplacement);

        // Move to current position + allowed displacement (absolute position)
        Vector2 finalPos = rb.position + allowedDisplacement;

        // Debug.Log("Final Position: " + finalPos);
        rb.MovePosition(finalPos);
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
        currentMovement = MovementState.Jumping;
    }

    private Vector2 HandleMove()
    {
        Vector2 desiredMove = new Vector2(xVelocity * speed, 0f) * Time.fixedDeltaTime;
        return desiredMove;
    }

    public Vector2 HandleJump()
    {
        if (IsGrounded() && yVelocity <= 0f) // landed
        {
            yVelocity = 0f;
            xVelocity = 0f;
            hasLanded = true;
            currentMovement = MovementState.Idle;
            return Vector2.zero;
        }
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

    public void SetFacing(int facing)
    {
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * facing; // +1 or -1
        transform.localScale = s;
    }










    public void PushPlayer(Vector2 force)
    {
        // Push player logic
    }





    #region Physic constraints


// private void PushPlayer()
//     {

//         float dist = Vector2.Distance(body.transform.position, enemy.transform.position);


//         if (dist > pushDistance)
//         {
//             Vector2 pushDir = (body.transform.position - enemy.body.transform.position).normalized;

//             // Small push deltas
//             Vector2 myDelta = pushDir * 0.02f;
//             Vector2 enemyDelta = -pushDir * 0.02f;

//             // Compute clamped world positions WITHOUT invoking pushbox/push recursion
//             Vector2 myNext = GetClampedPositionForDelta(myDelta);
//             Vector2 enemyNext = enemy.GetClampedPositionForDelta(enemyDelta);

//             // Only MovePosition if there's an actual change (avoid tiny redundant calls)
//             if ((myNext - rb.position).sqrMagnitude > 1e-6f)
//                 rb.MovePosition(myNext);

//             if ((enemyNext - enemy.rb.position).sqrMagnitude > 1e-6f)
//                 enemy.rb.MovePosition(enemyNext);
//         }
//     }


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
                //PushPlayer();
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


    public Vector2 ClampedMovement(Vector2 desiredMove)
    {
        Vector2 nextPos = rb.position + desiredMove;

        nextPos.x = Mathf.Clamp(nextPos.x, minX, maxX);
        nextPos.y = Mathf.Max(nextPos.y, minY);
        return nextPos;
    }


//  public Vector2 PushboxFeetCalculator(Vector2 velocity)
//     {
//         Vector2 rayOrigin = (Vector2)feet.transform.position;
//         Vector2 rayDirection = Vector2.down; // Only look downwards
//         float rayLength = 0.2f; // Short ray, just under your feet

//         Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.blue);

//         int playerLayerMask = LayerMask.GetMask("Player");
//         RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, rayDirection, rayLength, playerLayerMask);

//         foreach (RaycastHit2D hit in hits)
//         {

//             if (hit.collider != null && hit.collider != pushbox)
//             {
//                 // We hit the opponent's body collider from above
//                 Debug.DrawRay(hit.point, Vector2.up * 0.3f, Color.yellow);

//                 // Push the jumper slightly away
//                 float dir = Mathf.Sign(transform.position.x - hit.collider.transform.position.x);
//                 rb.MovePosition(rb.position + Vector2.right * dir * 0.05f);

//                 // Optional tiny bounce up
//                 velocity.y = Mathf.Abs(velocity.y) * 0.5f;

//                 // Optional: stop falling state and transition to fall or idle again
//                 // (depending if they are still in the air)
//                 break;
//             }
//         }

//         return velocity;
//     }

    #endregion





    #region OTHER FUNCS

    private PlayerMovement enemy;

    public void SetEnemy(PlayerMovement enemy) {
        this.enemy = enemy;
    }
    #endregion
}
