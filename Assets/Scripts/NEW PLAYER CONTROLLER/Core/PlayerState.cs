using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
    protected NewPlayerController player;

    public PlayerState(NewPlayerController player)
    {
        this.player = player;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }

    public virtual void FixedUpdate() { }

    public virtual void HandleNextState() { }

    public virtual Vector2 GetDesiredMovement()
    {
        return Vector2.zero; // Default implementation returns Vector2.zero
    }
}
