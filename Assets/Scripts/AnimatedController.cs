using System.Collections;
using UnityEngine;

using UnityEngine;
using System.Collections;

public class AnimatedController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float duckSpeed = 1f;
    [SerializeField] private float animationSpeed = 0.2f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float hitCooldown = 0.2f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 moveInput;
    private bool isDucking = false;
    private bool isGrounded = true;
    private bool isHitting = false;

    private Coroutine resetSpriteCoroutine;

    [Header("Sprites")]
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private Sprite[] hitSprites;
    [SerializeField] private Sprite[] kickSprites;
    [SerializeField] private Sprite[] duckSprites;
    [SerializeField] private Sprite[] duckHitSprites;
    [SerializeField] private Sprite[] duckKickSprites;
    [SerializeField] private Sprite[] shootSprites;
    [SerializeField] private Sprite[] tauntSprites;
    [SerializeField] private Sprite[] characterTraitSprites;
    [SerializeField] private Sprite[] specialSprites;

    private int hitSpriteIndex = 0;
    private float animationTimer;
    private bool usingFrame1 = true;

    private Sprite[] lastSprites = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        var input = PlayerInputHandler.Instance;

        input.OnMove += val => moveInput = val;
        input.OnDuckChanged += OnDuckChanged;
        input.OnJump += TryJump;
        input.OnHit += () => TryAttack(isDucking ? duckHitSprites : hitSprites);
        input.OnKick += () => TryAttack(isDucking ? duckKickSprites : kickSprites);
        input.OnShoot += () => TryAttack(shootSprites);
        input.OnTaunt += () => TryAttack(tauntSprites);
        input.OnSignature1 += () => TryAttack(characterTraitSprites);
        input.OnSignature2 += () => TryAttack(specialSprites);
    }

    private void Update()
    {
        ApplyMovement();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
    }

    private void ApplyMovement()
    {
        if (isGrounded && !isDucking && !isHitting && Mathf.Abs(moveInput.x) > 0.01f)
        {
            animationTimer += Time.deltaTime;
            if (animationTimer >= animationSpeed)
            {
                usingFrame1 = !usingFrame1;
                sr.sprite = usingFrame1 ? walkSprites[0] : walkSprites[1];
                animationTimer = 0f;
            }
        }
    }

    private void TryJump()
    {
        if (isGrounded && !isHitting)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    private void TryAttack(Sprite[] sprites)
    {
        if (isHitting) return;

        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogWarning("TryAttack called with empty or null sprites array!");
            return;
        }

        if(lastSprites != sprites)
        {
            hitSpriteIndex = 0;
            lastSprites = sprites;
        }
        isHitting = true;
        hitSpriteIndex = hitSpriteIndex % sprites.Length; // Just in case
        sr.sprite = sprites[hitSpriteIndex];
        hitSpriteIndex = (hitSpriteIndex + 1) % sprites.Length;

        if (resetSpriteCoroutine != null) StopCoroutine(resetSpriteCoroutine);
        resetSpriteCoroutine = StartCoroutine(ResetSpriteAfterDelay(0.5f));
    }

    private IEnumerator ResetSpriteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(hitCooldown);
        isHitting = false;
        yield return new WaitForSeconds(delay);
        sr.sprite = isDucking ? duckSprites[0] : walkSprites[0];
        hitSpriteIndex = 0;
        resetSpriteCoroutine = null;
    }

    private void OnDuckChanged(bool ducking)
    {
        isDucking = ducking;
        sr.sprite = ducking ? duckSprites[0] : walkSprites[0];
        moveSpeed = ducking ? duckSpeed : 5f;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground")) isGrounded = true;
    }
}
