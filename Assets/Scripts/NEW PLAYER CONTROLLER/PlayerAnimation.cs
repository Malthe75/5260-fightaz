using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationState
{
    Idle,
    Walking,
    Jumping,
    Falling,
}
public class PlayerAnimation : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite[] idleSprites;
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private Sprite[] jumpSprites;
    [SerializeField] private Sprite[] blockSprites;
    [SerializeField] private Sprite[] fallSprites;
    [SerializeField] private Sprite[] hurtSprites;

    [Header("Animation Settings")]
    [SerializeField] float walkAnimationSpeed = 0.2f;
    private SpriteRenderer sr;
    private AnimationState currentMovement = AnimationState.Idle;
    private float animationTimer;
    private float animationSpeed = 0.14f;
    private bool shouldLoop = true;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        switch (currentMovement)
        {
            case AnimationState.Idle:
                SetSprite(idleSprites[0]);
                break;
            case AnimationState.Walking:
                AnimateSprites(walkSprites, walkAnimationSpeed);
                break;
            case AnimationState.Jumping:
                AnimateSprites(jumpSprites, animationSpeed, shouldLoop);
                break;
            case AnimationState.Falling:
                break;
        }
    }

    public void SetIdleAnimation()
    {
        currentMovement = AnimationState.Idle;
    }
    public void SetAnimation(AnimationState animationState, bool shouldLoop = true)
    {
        this.shouldLoop = shouldLoop;
        currentMovement = animationState;
    }

    private void SetSprite(Sprite sprite)
    {
        sr.sprite = sprite;
    }
    private void AnimateSprites(Sprite[] frames, float speed, bool shouldLoop = true)
    {
        if (frames.Length == 0) return;

        animationTimer += Time.deltaTime;

        if (animationTimer >= speed)
        {
            int currentIndex = System.Array.IndexOf(frames, sr.sprite);
            int nextIndex = currentIndex + 1;

            if (nextIndex >= frames.Length)
            {
                if (shouldLoop)
                    nextIndex = 0;          // Loop back to start
                else
                    nextIndex = frames.Length - 1; // Stay on last frame
            }

            sr.sprite = frames[nextIndex];
            animationTimer = 0f;
        }
    }


    




}
