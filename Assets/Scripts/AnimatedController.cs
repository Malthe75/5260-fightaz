using System.Collections;
using UnityEngine;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class NamedAttack
{
    public string attackName;
    public AttackData attackData;
}

public class AnimatedController : MonoBehaviour
{

    [Header("Experimental")]
    public List<NamedAttack> attackLibrary = new List<NamedAttack>();
    [SerializeField] private GameObject hitboxObject;
    private BoxCollider2D hitboxCollider;
    [SerializeField] private GameObject uiObject;
    private UIHandler uiHandler;

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
        uiHandler = uiObject.GetComponent<UIHandler>();
    }

    private void Start()
    {
        var input = PlayerInputHandler.Instance;

        input.OnMove += val => moveInput = val;
        input.OnDuckChanged += OnDuckChanged;
        input.OnJump += TryJump;
        //input.OnHit += () => TryAttack(isDucking ? duckHitSprites : hitSprites);
        input.OnKick += () => TryAttack(isDucking ? duckKickSprites : kickSprites);
        input.OnShoot += () => TryAttack(shootSprites);
        input.OnTaunt += () => TryAttack(tauntSprites);
        //input.OnSignature1 += () => TryAttack(characterTraitSprites);
        //input.OnSignature2 += () => TryAttack(specialSprites);
        input.OnSignature1 += () => PlayAttack(GetAttackByName("Signature1"));
        input.OnSignature2 += () => PlayAttack(GetAttackByName("Signature2"));

        //Combo system
        input.OnHit += () =>
        {
            if (isAttacking)
            {
                if (canQueueNext)
                    queuedNextAttack = true;
            }
            else
            {
                StartComboChain();
            }
        };


        // Experimental
        hitboxCollider = hitboxObject.GetComponent<BoxCollider2D>();
        hitboxObject.SetActive(false);
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

    [SerializeField] private int attackDamage = 1;

    // EXPERIMENTAL SECTION 

    private int currentComboIndex = 0;
    private float comboTimer = 0f;
    public float comboResetTime = 1f;

    private bool isAttacking = false;
    private bool canQueueNext = false;
    private bool queuedNextAttack = false;

    public List<string> comboSequence = new List<string> { "Hit1", "Hit2", "Hit3" };

    void StartComboChain()
    {
        currentComboIndex = 0;
        PlayAttack(GetAttackByName(comboSequence[currentComboIndex]));
    }

    public void PlayAttack(AttackData attack)
    {
        StartCoroutine(PlayAttackCoroutine(attack));
    }
    void ActivateHitbox(Vector2 size, Vector2 offset)
    {
        // Set hitbox size and position
        hitboxObject.transform.localPosition = offset;
        hitboxCollider.size = size;
        hitboxObject.SetActive(true);

        // Manual collision detection
        Vector2 hitboxWorldPos = hitboxObject.transform.position;
        Collider2D[] hits = Physics2D.OverlapBoxAll(hitboxWorldPos, size, 0f);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                uiHandler.TakeDamage(attackDamage);
                Debug.Log("Hit enemy for " + attackDamage + " damage!");

                var trigger = hitboxObject.GetComponent<HitboxTrigger>();
                if (trigger != null)
                {
                    trigger.damage = attackDamage;
                }

            }
        }

        StartCoroutine(DisableHitboxAfter(0.1f));
    }


    IEnumerator DisableHitboxAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        hitboxObject.SetActive(false);
    }

    private IEnumerator PlayAttackCoroutine(AttackData attack)
    {
        isAttacking = true;
        queuedNextAttack = false;
        canQueueNext = false;

        foreach (var frame in attack.frames)
        {
            sr.sprite = frame.frameSprite;

            if (frame.hasHitbox)
            {
                ActivateHitbox(frame.hitboxSize, frame.hitboxOffset);
            }

            if (frame.attackSound != null)
            {
                AudioSource.PlayClipAtPoint(frame.attackSound, transform.position);
            }

            yield return new WaitForSeconds(frame.frameDuration);
        }

        // Open combo window
        canQueueNext = true;
        float timer = 0f;
        while(timer < comboResetTime)
        {
            if (queuedNextAttack)
            {
                canQueueNext = false;
                currentComboIndex++;

                if (currentComboIndex < comboSequence.Count){
                    PlayAttack(GetAttackByName(comboSequence[currentComboIndex]));
                }
                else
                {
                    currentComboIndex = 0;
                    isAttacking = false;
                }
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        // No next hit queued in time
        isAttacking = false;
        currentComboIndex = 0;
        sr.sprite = walkSprites[0];
    }

    /// //////////////////// EXPERIMENTAL SECTION ENDS HERE

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













    // JUST DIctionary stuff

    private Dictionary<string, AttackData> attackDict;

    private void BuildAttackDict()
    {
        attackDict = new Dictionary<string, AttackData>();
        foreach (var entry in attackLibrary)
        {
            if (!attackDict.ContainsKey(entry.attackName))
            {
                attackDict[entry.attackName] = entry.attackData;
            }
        }
    }

    public AttackData GetAttackByName(string name)
    {
        if (attackDict == null)
            BuildAttackDict();

        attackDict.TryGetValue(name, out var data);
        return data;
    }
}
