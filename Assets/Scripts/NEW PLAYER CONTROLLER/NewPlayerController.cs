using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerController : MonoBehaviour
{

    [Header("Idle state")]
    public Sprite[] idleSprites;

    [Header("Walk state")]
    public Sprite[] walkSprites;
    public float walkSpeed = 5f;
    public float animationSpeed = 0.2f;

    // References
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public SpriteRenderer sr;

    // Input
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public bool jumpInput;

    // State Machine
    public StateMachine stateMachine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        // Initialize state machine
        stateMachine = new StateMachine();

        // Start with IdleState
        stateMachine.ChangeState(new IdleState(this));
    }

    private void Update()
    {
        // Update input
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);

        // Update the current state
        stateMachine.Update();
    }

}
