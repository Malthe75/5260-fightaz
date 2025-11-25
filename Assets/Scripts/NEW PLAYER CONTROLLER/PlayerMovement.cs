using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Static list to keep track of all player instances
    public static readonly List<PlayerMovement> players = new List<PlayerMovement>();
    private void OnEnable() => players.Add(this);
    private void OnDisable() => players.Remove(this);

    private void FindEnemy()
    {
        enemy = All.FirstOrDefault(players => p != this);
        if(enemy == null) Debug.Log("Enemy not found yet: Try again later.");
    }

    // Other stuff
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform body;
    private PlayerMovement enemy;



    private void Start()
    {
        FindEnemy();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(float direction, float speed)
    {
        // 1. Desired movement (e.g go left or right)
        Vector2 desiredMove = new Vector2(direction * speed, 0f) * Time.fixedDeltaTime;

        Vector2 allowedMove = PushboxCalculator(desiredMove);

        player.rb.MovePosition(allowedMove);
    }

    public void Jump(float xVelocity, float yVelocity ,float jumpForce)
    {
        Vector2 movement = new Vector2(xVelocity, yVelocity * jumpForce) * Time.fixedDeltaTime;

        Vector2 allowedMove = PushboxCalculator(movement);

        player.rb.MovePosition(allowedMove);
    }

    public void Fall()
    {
        // Fall logic
    }

    public void PushPlayer(Vector2 force)
    {
        // Push player logic
    }

    public Vector2 PushboxCalculator(Vector2 movement)
    {
        Vector2 rayOrigin = (Vector2)body.transform.position;
    }


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
    public Vector2 ClampedMovement()
    {
        return null;
    }

    
}
