using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Windows;

public class WalkState : PlayerState
{
    private float animationTimer;

    private float idleBuffer = 0.15f;
    private float idleTimer = 0f;

    public WalkState(NewPlayerController player) : base(player) {}
    // Start is called before the first frame update
    public override void Enter()
    {
        player.sr.sprite = player.walkSprites[1];
    }

    // Update is called once per frame
    public override void Update()
    {
        
        HandleNextState();
        PlayAnimation();

    }

    private Vector2 RaycastCalculator(Vector2 desiredMove)
    {
        // Raycast in the direction we want to move from an origin.
        // Uses 0 for Y for this example.
        float rayLength = Mathf.Abs(desiredMove.x) + 1f;
        //Vector2 rayDirection = new Vector2(Mathf.Sign(desiredMove.x), 0f);
        Vector2 rayOrigin = (Vector2)player.body.transform.position;

        Vector2 enemyPosition = (Vector2)(player.enemy.body.transform.position);
        Vector2 rayDirection = (enemyPosition - rayOrigin).normalized;


        // Draw ray in the scene view
        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);
        // LayerMask
        int playerLayerMask = LayerMask.GetMask("Player");

        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, rayDirection, rayLength, playerLayerMask);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.collider != player.body.GetComponent<Collider2D>())
                {
                    Debug.Log("IM hitting?");
                    Debug.Log(hit.collider.name);


                    PushPlayer();
                    desiredMove.x = 0f;

                    Debug.DrawRay(hit.point, Vector2.up * 0.5f, Color.green);
                }
            }
        }

        return desiredMove;
    }

    private void PushPlayer()
    {
        float dist = Vector2.Distance(player.body.transform.position, player.enemy.transform.position);

        float pushDistance = 1.0f;

        if(dist > pushDistance)
        {
            // Calculate push direction (away from each other)
            Vector2 pushDir = (player.body.transform.position - player.enemy.body.transform.position).normalized;

            // Apply a *tiny* push to both players
            player.rb.MovePosition(player.rb.position + pushDir * 0.02f);
            player.enemy.rb.MovePosition(player.enemy.rb.position - pushDir * 0.02f);
        }
    }

    public override void FixedUpdate()
    {
        Vector2 desiredMove = new Vector2(player.moveInput.x * player.walkSpeed, 0f) * Time.fixedDeltaTime;
        Vector2 actualMove = RaycastCalculator(desiredMove);
        player.rb.MovePosition(player.rb.position + actualMove);
    }

    public override void HandleNextState()
    {
        // Attack transition
        if (player.input != MoveInput.Nothing)
        {
            player.stateMachine.ChangeState(new AttackState(player, player.input));
            return;
        }
        // Jump transition
        if (player.shouldJump)
        {
            if (player.moveInput.x > 0f) player.stateMachine.ChangeState(new JumpState(player, JumpInput.Right));
            else if (player.moveInput.x < 0f) player.stateMachine.ChangeState(new JumpState(player, JumpInput.Left));
        }
    }

    private void PlayAnimation()
    {
        animationTimer += Time.deltaTime;
        if (animationTimer >= player.animationSpeed)
        {
            // Advance to the next sprite
            int currentIndex = System.Array.IndexOf(player.walkSprites, player.sr.sprite);
            int nextIndex = (currentIndex + 1) % player.walkSprites.Length;
            player.sr.sprite = player.walkSprites[nextIndex];
            // Reset timer
            animationTimer = 0f;
        }
        if (Mathf.Abs(player.moveInput.x) < 0.01)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleBuffer)
            {

                player.stateMachine.ChangeState(new IdleState(player));
            }
        }
        else
        {
            idleTimer = 0f;
        }
    }


}
