using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


public enum AttackInput
{
    Nothing,
    Hit,
    Kick,
    CrouchHit,
    CrouchKick,
    Shoot,
    Signature1,
    Signature2,
    JumpHit,

}
public class NewPlayerController : MonoBehaviour
{

    #region State variables
    [Header("Idle state")]
    public Sprite[] idleSprites;

    [Header("Walk state")]
    public Sprite[] walkSprites;
    public float walkSpeed = 5f;
    public float animationSpeed = 0.2f;

    [Header("Attack state")]
    public List<AttackData> attackData;
    [HideInInspector] public AttackInput input = AttackInput.Nothing;

    [Header("Block state")]
    public Sprite[] blockSprites;
    [HideInInspector] public bool isBlocking = false;

    [Header("Jump state")]
    public Sprite[] jumpSprites;
    [HideInInspector] public bool shouldJump = false;
    [HideInInspector] public bool isGrounded = true;

    [Header("JumpAttack state")]


    // References
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public SpriteRenderer sr;
    //[HideInInspector] public AttackHitbox attackHitbox;

    // Input
    [HideInInspector] public Vector2 moveInput;

    // State Machine
    public StateMachine stateMachine;

    public AttackHitbox attackHitbox;


    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        // Initialize state machine
        stateMachine = new StateMachine();

        if(stateMachine.CurrentState == null)
        {

            stateMachine.ChangeState(new IdleState(this));
        }
        // Start with IdleState

    }

    private void Update()
    {
        // Update input
        //moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);


        // Decrement combo cooldown timer

        // Update the current state. THe if statement is only there to avoid errors when recompiling.
        if(stateMachine != null)
            stateMachine.Update();
        input = AttackInput.Nothing;
    }


    #region Input Actions
    // Input system.

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveInput = input;
    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isBlocking = true;
        }
        else if (context.canceled)
        {
            isBlocking = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            shouldJump = true;
        }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        Debug.Log("I ATTACK");
        if (!context.performed) return;

        if (context.performed)
        {
            string actionName = context.action.name;
            Debug.Log(actionName);
            switch (actionName)
            {
                case "Hit":
                    HandleAttackInput(AttackInput.Hit);
                    break;
                case "Kick":
                    HandleAttackInput(AttackInput.Kick);
                    break;
                case "Shoot":
                    HandleAttackInput(AttackInput.Shoot);
                    break;
                default:
                    Debug.LogWarning("Unknown attack action: " + actionName);
                    break;
            }
        }
    }


    private void HandleAttackInput(AttackInput input)
    {
        this.input = input;
        //stateMachine.ChangeState(new AttackState(this, input));
    }

    #endregion


    public void TakeHit(int damage, AttackFrameData attack)
    {
        Debug.Log("IT did this damage");
        stateMachine.ChangeState(new HurtState(this, attack));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Why?");
            isGrounded = true;
        }
    }

    //void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Ground"))
    //    {
    //        isGrounded = false;
    //    }
    //}
}
