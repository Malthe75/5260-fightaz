using UnityEngine;

public enum AnimationState
{
    Nothing,
    Idle,
    Walking,
    Jumping,
    BackwardsJumping,
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
    [SerializeField] float animationSpeed = 0.2f;
    private SpriteRenderer sr;
    private AnimationState currentState = AnimationState.Idle;
    private float animationTimer;
    private bool shouldLoop = true;
    private float animationLength = 1f;
    private int currentIndex;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case AnimationState.Idle:
                SetSprite(idleSprites[0]);
                break;
            case AnimationState.Walking:
                AnimateSprites(walkSprites, animationSpeed);
                break;
            case AnimationState.Jumping:
                float speed = CalculateAnimationSpeed(animationLength, jumpSprites.Length);
                AnimateSprites(jumpSprites, speed, shouldLoop);
                break;
            case AnimationState.BackwardsJumping:
                float animSpeed = CalculateAnimationSpeed(animationLength, jumpSprites.Length);
                AnimateSpritesBackwards(jumpSprites, animSpeed, shouldLoop);
                break;
            case AnimationState.Falling:
                break;
            default:
                break;
        }
    }

    public void SetIdleAnimation()
    {
        currentState = AnimationState.Idle;
    }
    public void SetAnimation(AnimationState animationState, bool shouldLoop = true)
    {
        this.shouldLoop = shouldLoop;
        currentIndex = 0;
        currentState = animationState;
    }

    public void SetJumpAnimation(AnimationState animationState, float animationLength)
    {
        this.animationLength = animationLength;
        this.shouldLoop = false;
        currentIndex = 0;
        if(animationState == AnimationState.BackwardsJumping) currentIndex = jumpSprites.Length -1;
        currentState = animationState;
    }


    public void SetSprite(Sprite sprite)
    {
        currentState = AnimationState.Nothing;
        sr.sprite = sprite;
    }

    public void SetColor(Color color)
    {
        sr.color = color;
    }
    private void AnimateSprites(Sprite[] frames, float speed, bool shouldLoop = true)
    {
        if (frames.Length == 0) return;

        animationTimer += Time.deltaTime;

        if (animationTimer >= speed)
        {
            sr.sprite = frames[currentIndex];
            currentIndex++;

            if (currentIndex >= frames.Length)
            {
                if (shouldLoop)
                    currentIndex = 0;          // Loop back to start
                else
                    currentIndex = frames.Length - 1; // Stay on last frame
            }

            animationTimer = 0f;
        }
    }

    private void AnimateSpritesBackwards(Sprite[] frames, float speed, bool shouldLoop = true)
    {
        if (frames.Length == 0) return;

        animationTimer += Time.deltaTime;

        if (animationTimer >= speed)
        {
            sr.sprite = frames[currentIndex];
            currentIndex--;

            if (currentIndex < 0)
            {
                if (shouldLoop)
                    currentIndex = frames.Length - 1; // Loop back to end
                else
                    currentIndex = 0;          // Stay on first frame
            }

            animationTimer = 0f;
        }
    }


    private float CalculateAnimationSpeed(float duration, int frameCount)
    {
        if (frameCount <= 0) return animationSpeed;
        return duration / frameCount;
    }


    




}
