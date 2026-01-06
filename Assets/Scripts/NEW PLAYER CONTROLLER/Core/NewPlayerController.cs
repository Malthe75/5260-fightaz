using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerController : MonoBehaviour
{
    public PlayerMovement Movement { get; private set; }
    public PlayerAnimation Animation { get; private set; }
    [Header("Move Map")]
    public MoveMap moveMap;


    #region State variables
    [Header("Idle state")]
    public Sprite[] idleSprites;

    [Header("Walk state")]
    public Sprite[] walkSprites;
    public float walkSpeed = 5f;
    public float animationSpeed = 0.2f;


    [Header("Attack state")]
    [HideInInspector] public MoveInput input = MoveInput.Nothing;
    [HideInInspector] public AttackData attack;
    [HideInInspector] public bool shouldAttack = false;
    [SerializeField] float attackBufferTime = 0.2f;
    private float lastAttackInputTime;

    [Header("Block state")]
    public Sprite[] blockSprites;
    [HideInInspector] public bool isBlocking = false;

    [Header("Jump state")]
    public Sprite[] jumpSprites;
    public float jumpForce = 25f;
    public AudioClip[] jumpSounds;
    [HideInInspector] public bool shouldJump = false;
    [HideInInspector] public bool isGrounded = true;

    [Header("Fall state")]
    public Sprite fallSprite;
    public AudioClip[] fallSounds;

    [Header("Hurt state")]
    public AudioClip[] hurtSounds;


    [Header("Physics")]
    public AttackHitbox attackHitbox;
    [Header("JumpAttack state")]


    // References
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public SpriteRenderer sr;
    //[HideInInspector] public CapsuleCollider2D capsule;

    // Input
    [HideInInspector] public Vector2 moveInput;
    private MoveResolver moveResolver;

    // State Machine
    public StateMachine stateMachine;


    // ENEMY REFERENCE
    private PlayerMovement enemy;
    #endregion
    private void Awake()
    {
        moveResolver = new MoveResolver();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        Movement = GetComponent<PlayerMovement>();
        Animation = GetComponentInChildren<PlayerAnimation>();


        // Initialize state machine
        stateMachine = new StateMachine();

        if (stateMachine.CurrentState == null)
        {

            stateMachine.ChangeState(new IdleState(this));
        }
        // Start with IdleState

    }

    public void SetEnemy(NewPlayerController enemy)
    {
        this.enemy = enemy.GetComponent<PlayerMovement>();
        Debug.Log($"Enemy set to {enemy.gameObject.name}");
        Movement.SetEnemy(this.enemy);
    }
    public void SetClashState()
    {
        stateMachine.ChangeState(new ClashState(this));
    }

    public int facing = 1;
    private void FixedUpdate()
    {
        stateMachine?.CurrentState?.FixedUpdate();

        SetFacing();
    }

    private void SetFacing()
    {
        if (shouldAttack || shouldJump) return;
        facing = transform.position.x < enemy.transform.position.x ? 1 : -1;
        Movement.SetFacing(facing);
    }
    private void Update()
    {
        if (shouldAttack && Time.time - lastAttackInputTime > attackBufferTime)
        {
            shouldAttack = false;
        }
        // Update the current state. THe if statement is only there to avoid errors when recompiling.
        stateMachine?.Update();
        input = MoveInput.Nothing;
    }


    #region Input Actions
    // Input system.

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.action.ReadValue<Vector2>();

        moveInput = new Vector2(
            Mathf.Abs(input.x) > 0 ? Mathf.Sign(input.x) : 0,
            Mathf.Abs(input.y) > 0 ? Mathf.Sign(input.y) : 0
        );
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

    [Header("Input Buffer")]
    public float jumpBufferTime = 0.2f;
    public float jumpPressedTime = -1f;
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpPressedTime = Time.time;
            shouldJump = true;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        // Resolve the attack
        if (context.performed)
        {
            attack = moveResolver.ResolveAttack(
                context.action.name,
                moveInput.x,
                facing,
                moveMap
            );
            shouldAttack = true;
            lastAttackInputTime = Time.time;
        }
    }

    #endregion

    public void TakeHit(int damage, AttackFrameData attack)
    {
        stateMachine.ChangeState(new HurtState(this, attack.xKnockback, attack.yKnockup));
    }


    public void SetAttack(MoveInput moveInput)
    {
        attack = moveResolver.SetAttack(moveInput, moveMap);
    }


}