using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class AnimatedController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5f;

    private Rigidbody2D rb;
    private PlayerInputHandler inputHandler;
    private float horizontalInput;

    private bool isGrounded = true;
    private bool shouldJump = false;

  
    private SpriteRenderer sr;
    [Header("Sprites")]
    [SerializeField] private Sprite[] hitSprites;
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private Sprite[] kickSprites;

    private bool isHitting = false;
    private int hitSpriteIndex = 0;
    private Coroutine resetSpriteCoroutine;


    private bool isKicking = false;

    [Header("Hitting")]
    [SerializeField] private float hitCooldown = 0.1f;
    [SerializeField] private float kickCooldown = 0.1f;

    [Header("Animation Settings")]
    [SerializeField] private float animationSpeed = 0.2f; // Speed of the animation frames

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        inputHandler = PlayerInputHandler.Instance;
    }

    private void Update()
    {
        horizontalInput = inputHandler.MoveInput.x;
        shouldJump = inputHandler.JumpInput && isGrounded;


        if (inputHandler.HitInput && !isHitting)
        {
            ApplyHit();
        }
        if (inputHandler.KickInput && !isHitting)
        {
            ApplyKick();
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
        if (isGrounded)
        {
            animateMovement();
        }

    }

    private float animationTimer;
    private bool usingFrame1 = true;
    void animateMovement()
    {
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            animationTimer += Time.deltaTime;
            if (animationTimer >= animationSpeed)
            {
                usingFrame1 = !usingFrame1;
                sr.sprite = usingFrame1 ? walkSprites[0] : walkSprites[1];
                animationTimer = 0f;
            }
        }
        /*
        else
        {
            // Reset to idle frame when not moving
            sr.sprite = walkSprites[0];
            usingFrame1 = true;
            animationTimer = 0f;
        }*/
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

    private void ApplyHit()
    {
        isHitting = true;
        sr.sprite = hitSprites[hitSpriteIndex];

        // Cycle to the next hit sprite
        hitSpriteIndex = ( hitSpriteIndex + 1 ) % hitSprites.Length;

        Debug.Log("HIT! " + hitSpriteIndex);

        // Stop any existing coroutine (To reset timer)
        if (resetSpriteCoroutine != null)
        {
            Debug.Log("Stopping coroutine!");
            StopCoroutine(resetSpriteCoroutine);
        }

        resetSpriteCoroutine = StartCoroutine(ResetSpriteAfterDelay(0.5f));
    }

    private void ApplyKick()
    {
        isHitting = true;
        sr.sprite = kickSprites[0];
        StartCoroutine(ResetSprite(0.5f));

    }

    private IEnumerator ResetSprite(float delay)
    {
        yield return new WaitForSeconds(delay);
        sr.sprite = walkSprites[0];
        isHitting = false;
    }

    private IEnumerator ResetSpriteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(hitCooldown);
        isHitting = false;
        yield return new WaitForSeconds(delay);

        hitSpriteIndex = 0; // Reset index to 0
        Debug.Log(" Resetiing sprite!");
        sr.sprite = walkSprites[0];
        isHitting = false;
        // Clear reference
        resetSpriteCoroutine = null;
    }

}
