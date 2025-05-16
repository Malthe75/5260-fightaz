using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimatedController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5f;

    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;
    private float horizontalInput;

    private bool isGrounded = false;
    private bool shouldJump = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        inputHandler = PlayerInputHandler.Instance;
    }

    private void Update()
    {
        horizontalInput = inputHandler.MoveInput.x;
        shouldJump = inputHandler.JumpInput && isGrounded;


        if (horizontalInput != 0)
        {
            FlipSprite(horizontalInput);
        }
    }


    private void FixedUpdate()
    {
        ApplyMovement();
        // Jumping
        if (shouldJump)
        {
            ApplyJump();
        }
    }

    void ApplyMovement()
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

    }

    void ApplyJump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
        shouldJump = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void FlipSprite(float horizontalMovement)
    {

        if(horizontalMovement < 0)
        {
            transform.localScale = new Vector3(0.7f, 0.7f, 1);
        }
        else if(horizontalMovement > 0)
        {
            transform.localScale = new Vector3(-0.7f, 0.7f, 1);
        }
    }

}
