using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class NamedAttack
{
    public string attackName;
    public AttackData attackData;
}

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
    private GameObject uiObject;
    private UIHandler uiHandler;
    private CombatManager combatManager; // Reference to the CombatManager script for managing combat state

    [Header("Blocking Settings")]
    [SerializeField] private Sprite[] blockSprites;


    // COMBO SETTINGS
    private int comboCounter = 0;
    private List<ComboData> currentCombos;
    private bool isAttacking = false;
    private bool queuedNextAttack = false;
    private AttackInput currentComboInput = AttackInput.Hit;

    private HitFeedback hitFeedback; // Reference to the HitFeedback script for visual effects
    private FightManager fightManager; // Reference to the FightManager script for managing fight state
    private FightManagerTest fightManagerTest; // Reference to the FightManager test script for managing fight state


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
        uiObject = GameObject.Find("UIHandler");
        uiHandler = uiObject.GetComponent<UIHandler>();
        GameObject fightManagerObject = GameObject.Find("Fightmanager");
        hitFeedback = fightManagerObject.GetComponent<HitFeedback>();
        fightManagerTest = fightManagerObject.GetComponent<FightManagerTest>();
        combatManager = fightManagerObject.GetComponent<CombatManager>();
    }

    private void Start()
    {
        moveSpeed = walkSpeed;

        comboCounter = 0;


        // Combo setup
        currentCombos = comboLibrary.combos.ToList();

        // Set up hitbox collider
        hitboxCollider = hitboxObject.GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        walkAnimation();
    }

    private void FixedUpdate()
    {
        if (!overrideMovement)
        {
            rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        }
    }

    // ################################################################################################################//
    // ################################################################################################################//
    // ################################################################################################################//
    // INPUT SYSTEM, UNITY EVOKED FUNCTIONS //
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TryJump();
        }
    }
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnCrouchChanged(true);
        }
        else if (context.canceled)
        {
            OnCrouchChanged(false);
        }
    }

    // This might be able to be put in only one function
    public void onHit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Attack(isCrouching ? AttackInput.CrouchHit : AttackInput.Hit);
        }
    }
    public void onKick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Attack(isCrouching ? AttackInput.CrouchKick : AttackInput.Kick);
        }
    }
    public void onSignature1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Attack(AttackInput.Signature1);
        }
    }
    public void onSignature2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Attack(AttackInput.Signature2);
        }
    }

    public void onBlock(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            sr.sprite = blockSprites[0]; // Change to block sprite
        }
        else if (context.canceled)
        {
            sr.sprite = isCrouching ? crouchSprites[0] : walkSprites[0]; // Change back to normal sprite
        }
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
        if (isAttacking || !isGrounded)
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
            float speed = 10f;
            rb.MovePosition(rb.position + new Vector2(-speed * transform.localScale.x, 0) * Time.fixedDeltaTime);
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
        FinishCombo();
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
            // Return true (i.e., remove) if we've gone past the length of this combo
            if (combo.inputSequence.Count <= comboCounter)
            {
                return true;
            }

            // Return true (i.e., remove) if the current input doesn't match
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
        int attackDamage = 1; // your damage value

        // Set hitbox size and position
        hitboxObject.transform.localPosition = offset;
        hitboxCollider.size = size;
        hitboxObject.SetActive(true);

        string attackerTag = hitboxObject.tag; // e.g. "Player1Attack" or "Player2Attack"

        Vector2 hitboxWorldPos = hitboxObject.transform.position;

        LayerMask hitboxLayerMask = LayerMask.GetMask("HitboxTarget");

        Collider2D[] hits = Physics2D.OverlapBoxAll(hitboxWorldPos, size, 0f, hitboxLayerMask);

        Collider2D bestHit = null;
        float minDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            // Skip own attack hitbox collider
            if (hit.tag == attackerTag + "Upper" || hit.tag == attackerTag + "Lower")
                continue;

            // Calculate distance to find best hitbox to apply damage
            float distance = Vector2.Distance(hitboxWorldPos, hit.bounds.center);
            if (distance < minDistance)
            {
                minDistance = distance;
                bestHit = hit;
            }
        }
        if (bestHit != null)
        {
            switch (bestHit.tag)
            {
                case "P1Upper":
                    Debug.Log("Player 1 Upper hit");
                    uiHandler.TakeDamage1(attackDamage);
                    hitFeedback.TriggerHitEffect(); // Trigger hit feedback effect
                    fightManagerTest.p1sr.color = Color.red; // Temporary visual feedback for hit
                    break;
                case "P1Lower":
                    Debug.Log("Player 1 Lower hit");
                    uiHandler.TakeDamage1(attackDamage);
                    hitFeedback.TriggerHitEffect(); // Trigger hit feedback effect
                    fightManagerTest.p1sr.color = Color.red; // Temporary visual feedback for hit

                    break;
                case "P2Upper":
                    Debug.Log("Player 2 Upper hit");
                    uiHandler.TakeDamage2(attackDamage);
                    hitFeedback.TriggerHitEffect(); // Trigger hit feedback effect
                    fightManagerTest.p2sr.color = Color.red;

                    break;
                case "P2Lower":
                    Debug.Log("Player 2 Lower hit");
                    uiHandler.TakeDamage2(attackDamage);
                    hitFeedback.TriggerHitEffect(); // Trigger hit feedback effect
                    fightManagerTest.p2sr.color = Color.red; // Temporary visual feedback for hit



                    break;
                default:
                    Debug.Log("Hit something untagged or unexpected");
                    break;
            }

            var trigger = hitboxObject.GetComponent<HitboxTrigger>();
            if (trigger != null)
            {
                trigger.damage = attackDamage;
            }
        }

        StartCoroutine(DisableHitboxAfter(0.1f));
    }


    IEnumerator DisableHitboxAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        hitboxObject.SetActive(false);

        // Reset the colors after hit - just here for now
        fightManagerTest.p1sr.color = Color.white;
        fightManagerTest.p2sr.color = Color.white;
    }

}

