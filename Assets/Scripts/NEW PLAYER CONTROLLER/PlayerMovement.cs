using System;
using UnityEngine;

public struct MovementCommand
{
    public Vector2 velocity; // Units/sec
    public bool doJump; // one-shot
    public float jumpVelocity; // if doJump true
}
public class PlayerMovement : MonoBehaviour
{
    private MovementCommand currentCommand;
    private bool hasCommand;

    // Other stuff
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform body;
    private Collider2D pushbox;
    private PlayerMovement enemy;


    public float minX = -5f;
    public float maxX = 5f;
    public float minY = 0f;


    public void SetMovementCommand(MovementCommand command)
    {
        currentCommand = command;
        hasCommand = true;
    }
    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        if (hasCommand)
        {
            
            // Perform 

            // Move
            Vector2 movement = currentCommand.velocity * dt;
            Vector2 allowedMove = PushboxCalculator(movement);
            Vector2 clampedPos = ClampedMovement(allowedMove);
            rb.MovePosition(clampedPos);

            // Jump
            if (currentCommand.doJump)
            {
                Vector2 jumpMovement = new Vector2(0f, currentCommand.jumpVelocity) * dt;
                Vector2 allowedJumpMove = PushboxCalculator(jumpMovement);
                Vector2 clampedJumpPos = ClampedMovement(allowedJumpMove);
                rb.MovePosition(clampedJumpPos);
            }
                
            hasCommand = false;
        }


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

    public void Move(float direction, float speed)
    {
        // 1. Desired movement (e.g go left or right)
        Vector2 desiredMove = new Vector2(direction * speed, 0f) * Time.fixedDeltaTime;

        Vector2 allowedMove = PushboxCalculator(desiredMove);

        rb.MovePosition(allowedMove);
    }

    public void Jump(float xVelocity, float yVelocity ,float jumpForce)
    {
        Vector2 movement = new Vector2(xVelocity, yVelocity * jumpForce) * Time.fixedDeltaTime;

        Vector2 allowedMove = PushboxCalculator(movement);

        rb.MovePosition(allowedMove);
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
        Vector2 rayOrigin = (Vector2)body.transform.position;


        // Raycast in the direction of the of the enemy
        Vector2 enemyPosition = (Vector2)enemy.body.transform.position;
        Vector2 rayDirection = (enemyPosition - rayOrigin).normalized;

        // Ray length changed the number 1 to anything for longer length
        float rayLength = Mathf.Abs(desiredMove.x) + 1f;

        // Draw ray the length of the ray
        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);

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



    







    #region MIGHT BE MOVED OUT OF HERE LATER
    private void SetEnemy()
    {
        if(transform.CompareTag("Player1"))
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
