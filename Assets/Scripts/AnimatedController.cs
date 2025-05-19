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

public enum ComboInput
{
    Hit,
    Kick,
    CrouchHit,
    CrouchKick
}

public class AnimatedController : MonoBehaviour
{

    [Header("Experimental")]
    public List<NamedAttack> attackLibrary = new List<NamedAttack>();
    [SerializeField] private GameObject hitboxObject;
    private BoxCollider2D hitboxCollider;
    [SerializeField] private GameObject uiObject;
    private UIHandler uiHandler;
    public ComboLibrary comboLibrary;

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
    //[SerializeField] private Sprite[] hitSprites;
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
        //prepareComboDictionary();

        //foreach (var item in collection)
        //{

        //}

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
        //input.OnKick += () => TryAttack(isDucking ? duckKickSprites : kickSprites);
        input.OnShoot += () => TryAttack(shootSprites);
        input.OnTaunt += () => TryAttack(tauntSprites);
        //input.OnSignature1 += () => TryAttack(characterTraitSprites);
        //input.OnSignature2 += () => TryAttack(specialSprites);
        input.OnSignature1 += () => PlayAttack(GetAttackByName("Signature1"));
        input.OnSignature2 += () => PlayAttack(GetAttackByName("Signature2"));

        //Combo system
        //input.OnHit += () =>
        //{
        //    var chosenCombo = isDucking ? comboSequenceCrouchHit : comboSequenceHit;

        //    if (isAttacking && chosenCombo == currentComboSequence)
        //    {
        //        if (canQueueNext)
        //            queuedNextAttack = true;
        //    }
        //    else
        //    {
        //        StartComboChain(chosenCombo);
        //    }
        //};



        //input.OnKick += () =>
        //{
        //    var chosenCombo = isDucking ? comboSequenceCrouchKick : comboSequenceKick;

        //    if (isAttacking && chosenCombo == currentComboSequence)
        //    {
        //        if (canQueueNext)
        //            queuedNextAttack = true;
        //    }
        //    else
        //    {
        //        StartComboChain(chosenCombo);
        //    }
        //};

        input.OnHit += () => RegisterInput(ComboInput.Hit);
        input.OnKick += () => RegisterInput(ComboInput.Kick);


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
        if (!isAttacking && isGrounded && !isDucking && !isHitting && Mathf.Abs(moveInput.x) > 0.01f)
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

    private List<ComboInput> currentInputSequence = new List<ComboInput>();
    private List<ComboData> potentialCombos = new List<ComboData>();
    private bool isExecutingCombo = false;


    private int currentComboIndex = 0;
    private float comboTimer = 0f;
    public float comboResetTime = 1f;

    private bool isAttacking = false;
    private bool canQueueNext = false;
    private bool queuedNextAttack = false;

    public List<string> comboSequenceHit = new List<string> { "Hit1", "Hit2", "Hit3" };
    public List<string> comboSequenceKick = new List<string> { "Kick1", "Kick2" };
    public List<string> comboSequenceCrouchHit = new List<string> { "CrouchHit1", "CrouchHit2" };
    public List<string> comboSequenceCrouchKick = new List<string> { "CrouchKick1", "CrouchKick2" };

    private List<string> currentComboSequence = new List<string>();


    // EXTRA EXPERIENMTAL 
    public void RegisterInput(ComboInput input)
{
    Debug.Log("Input received: " + input);

    if (isExecutingCombo) return;

    currentInputSequence.Add(input);

    Debug.Log("Current input sequence: " + string.Join(", ", currentInputSequence));

    var matchingCombos = new List<ComboData>();
    ComboData fullMatch = null;

    foreach (var combo in comboLibrary.combos)
    {
        if (combo.inputSequence.Count < currentInputSequence.Count)
            continue;

        bool matches = true;
        for (int i = 0; i < currentInputSequence.Count; i++)
        {
            if (combo.inputSequence[i] != currentInputSequence[i])
            {
                matches = false;
                break;
            }
        }

        if (matches)
        {
            matchingCombos.Add(combo);

            // Check if this is a full match
            if (combo.inputSequence.Count == currentInputSequence.Count)
            {
                fullMatch = combo;
            }
        }
    }

    potentialCombos = matchingCombos;

    if (fullMatch != null)
    {
        Debug.Log("Full combo matched! Playing combo: " + string.Join(", ", fullMatch.inputSequence));
        StartCoroutine(PlayCombo(fullMatch));
        ResetComboState();
    }
    else if (potentialCombos.Count == 0)
    {
        Debug.Log("No combos match this sequence. Resetting.");
        ResetComboState();
    }
    else
    {
        Debug.Log("Partial combo match. Awaiting next input...");
    }
}

    private void ResetComboState()
    {
        currentInputSequence.Clear();
        potentialCombos.Clear();
    }

    private IEnumerator PlayCombo(ComboData combo)
    {
        isExecutingCombo = true;
        for(int i = 0; i < combo.attacks.Count; i++)
        {
            var attack = combo.attacks[i];
            PlayAttack(attack);
            yield return new WaitForSeconds(attack.frames[0].frameDuration); // Wait for the first frame duration before proceeding to the next attack
        }
        isExecutingCombo = false;
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

        // Combo finished?
        if (currentComboIndex >= currentComboSequence.Count - 1)
        {
            Debug.Log("Combo finished.");
            isAttacking = false;
            currentComboIndex = 0;
            sr.sprite = walkSprites[0];
            if (isDucking) sr.sprite = duckSprites[0];
            yield break;
        }

        // Open combo window for next input
        canQueueNext = true;
        float timer = 0f;

        while (timer < comboResetTime)
        {
            if (queuedNextAttack)
            {
                queuedNextAttack = false;
                canQueueNext = false;
                currentComboIndex++;

                PlayAttack(GetAttackByName(currentComboSequence[currentComboIndex]));
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // No input received within combo window
        isAttacking = false;
        currentComboIndex = 0;
        sr.sprite = walkSprites[0];
        if(isDucking) sr.sprite = duckSprites[0];
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
    private Dictionary<string, ComboData> comboDict;

    //private void prepareComboDictionary()
    //{
    //    comboDict = new Dictionary<string, ComboData>();
    //    foreach (var combo in comboLibrary.combos)
    //    {
    //        comboDict[combo.comboName] = combo;
    //    }
    //}

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
