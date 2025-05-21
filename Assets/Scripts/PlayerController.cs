using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum AttackInput
{
    Hit,
    Kick,
    CrouchHit,
    CrouchKick,
    Shoot,
    Signature1,
    Signature2,

}
public class PlayerController : MonoBehaviour
{
    [Header("Attack related")]
    public List<NamedAttack> attackLibrary = new List<NamedAttack>();
    public ComboLibrary comboLibrary;
    [SerializeField] private GameObject hitboxObject;
    //Private attack related
    private BoxCollider2D hitboxCollider;


    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float jumpForce = 5f;
    // Private movement related
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded = true;
    private bool isCrouching = false;
    private float moveSpeed;
    private bool overrideMovement = false;


    [Header("Animation Settings")]
    [SerializeField] private float animationSpeed = 0.2f;
    // Private animation related
    private float animationTimer;
    private bool usingFrame1 = true;

    [Header("Non attack sprites")]
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private Sprite[] crouchSprites;

    [Header("UI Settings")]
    [SerializeField] private GameObject uiObject;
    private UIHandler uiHandler;


    // COMBO SETTINGS
    private int comboCounter = 0;
    private List<ComboData> currentCombos;
    private bool isAttacking = false;
    private bool queuedNextAttack = false;
    private AttackInput currentComboInput = AttackInput.Hit;


    // ALL OTHER PRIVATE VARIABLES
    SpriteRenderer sr;



    // ################################################################################################################//
    // ################################################################################################################//
    // ################################################################################################################//
    // AWAKE, START, UPDATE, FIXEDUPDATE SECTION //

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        uiHandler = uiObject.GetComponent<UIHandler>();
    }


    private void Start()
    {
        moveSpeed = walkSpeed;

        comboCounter = 0;
        var input = PlayerInputHandler.Instance;

        //Move inputs
        input.OnMove += val => moveInput = val;
        input.OnJump += TryJump;
        input.OnCrouchChanged += OnCrouchChanged;

        // Not sure how to handle taunt yet, with this setup
        


        // ----- Attack inputs ----- //
        // Combos
        input.OnHit += () => Attack(isCrouching ? AttackInput.CrouchHit : AttackInput.Hit);
        input.OnKick += () => Attack(isCrouching ? AttackInput.CrouchKick : AttackInput.Kick);
        // Singular combos
        input.OnSignature1 += () => Attack(AttackInput.Signature1);
        input.OnSignature2 += () => Attack(AttackInput.Signature2);


        // Combo setup
        currentCombos = comboLibrary.combos.ToList();

        // Set up hitbox collider
        hitboxCollider = hitboxObject.GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        if (!overrideMovement)
        {
            rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        }
    }

    private void Update()
    {
        walkAnimation();
    }

    // ################################################################################################################//
    // ################################################################################################################//
    // ################################################################################################################//
    // MOVEMENT SECTION //
    // ########## MIGHT NEED REDOING?
    private void walkAnimation()
    {
        // Only walk animation when moving and being grounded
        if (Mathf.Abs(moveInput.x) > 0.01f && isGrounded && !isCrouching)
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
        if (isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    // Might be moved to some sort of collision title. This is just for making isGrounded true when touching the ground.
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground")) isGrounded = true;
    }

    // Either changes isCrouching to true/false and the sprite/speed
    private void OnCrouchChanged(bool crouching)
    {
        isCrouching = crouching;
        sr.sprite = crouching ? crouchSprites[0] : walkSprites[0];
        moveSpeed = crouching ? crouchSpeed : walkSpeed;
    }




    // ################################################################################################################//
    // ################################################################################################################//
    // ################################################################################################################//
    // ATTACK SECTION //
    private void Attack(AttackInput input)
    {
        // Check if the player is already attacking, if so, put the next attack in queue.
        if (isAttacking)
        {
            currentComboInput = input;
            queuedNextAttack = true;
            return;
        }
        CheckForCombos(input);
    }

    private IEnumerator PlayAttackCoroutine(AttackData attack)
    {
        isAttacking = true;
        overrideMovement = true;

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

            // Simple test — feel free to make frame-specific movement later
            rb.MovePosition(rb.position + new Vector2(-0.3f, 0));
            Debug.Log("Attack force applied");



            yield return new WaitForSeconds(frame.frameDuration);
        }
        float comboBufferTime = attack.frames[attack.frames.Count - 1].frameDuration;
        float timer = 0f;

        while(timer < comboBufferTime)
        {
            // another attack was queued, during the buffer time, so go to that.
            if (queuedNextAttack)
            {
                queuedNextAttack = false;
                isAttacking = false;
                overrideMovement = false;
                Attack(currentComboInput);
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        // Reset to "idle" sprite:
        sr.sprite = isCrouching ? crouchSprites[0] : walkSprites[0];
        isAttacking = false;
        overrideMovement = false;
    }









    // ################################################################################################################//
    // ################################################################################################################//
    // ################################################################################################################//
    // COMBO SECTION //
    private void CheckForCombos(AttackInput input)
    {
       


        // Removes all combos that don't match the current input
        currentCombos.RemoveAll(combo =>
        {
            return combo.inputSequence[comboCounter] != input;
        });

        // Check if input is possible for any of the combos
        if (currentCombos.Count == 0)
        {
            // Reset combo and play attack from start
            FinishCombo();
            CheckForCombos(input);
            return;
        }

        comboCounter++;


        StartCoroutine(PlayAttackCoroutine(currentCombos[0].attacks[comboCounter - 1]));

        // Finish combo
        if (currentCombos.Count <= 1 && comboCounter >= currentCombos[0].inputSequence.Count)
        {
            FinishCombo();
        }
    }

    private void FinishCombo()
    {
        comboCounter = 0;
        currentCombos = comboLibrary.combos.ToList();
    }




    // ################################################################################################################//
    // ################################################################################################################//
    // ################################################################################################################//
    // Hitbox related methods //
    void ActivateHitbox(Vector2 size, Vector2 offset)
    {
        int attackDamage = 1; // Set your attack damage here

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

}

