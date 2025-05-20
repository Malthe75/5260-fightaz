using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum ComboInputs
{
    Hit,
    Kick,
    CrouchHit,
    CrouchKick,
    Shoot,

}
public class PlayerController : MonoBehaviour
{
    [Header("Attack related")]
    public List<NamedAttack> attackLibrary = new List<NamedAttack>();
    public ComboLibrary comboLibrary;
    [SerializeField] private GameObject hitboxObject;
    private BoxCollider2D hitboxCollider;


    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Animation Settings")]
    [SerializeField] private float animationSpeed = 0.2f;

    [Header("Non attack sprites")]
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private Sprite[] crouchSprites;


    // COMBO SETTINGS
    private int comboCounter = 0;
    private List<ComboData> currentCombos;
    private bool isAttacking = false;
    private bool queuedNextAttack = false;
    private ComboInput currentComboInput = ComboInput.Hit;


    // ALL OTHER PRIVATE VARIABLES
    SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }


    private void Start()
    {
        comboCounter = 0;
        var input = PlayerInputHandler.Instance;

        input.OnHit += () => Attack(ComboInput.Hit);
        input.OnKick += () => Attack(ComboInput.Kick);

        // Combo setup
        currentCombos = comboLibrary.combos.ToList();

        // Set up hitbox collider
        hitboxCollider = hitboxObject.GetComponent<BoxCollider2D>();
    }


    // ################################################################################################################//
    // ################################################################################################################//
    // ################################################################################################################//
    // ATTACK SECTION //
    private void Attack(ComboInput input)
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
        float comboBufferTime = attack.frames[attack.frames.Count - 1].frameDuration;
        float timer = 0f;

        while(timer < comboBufferTime)
        {
            // another attack was queued, during the buffer time, so go to that.
            if (queuedNextAttack)
            {
                queuedNextAttack = false;
                isAttacking = false;
                Attack(currentComboInput);
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        sr.sprite = walkSprites[0];
        isAttacking = false;
    }









    // ################################################################################################################//
    // ################################################################################################################//
    // ################################################################################################################//
    // COMBO SECTION //
    private void CheckForCombos(ComboInput input)
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
                //uiHandler.TakeDamage(attackDamage);

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

