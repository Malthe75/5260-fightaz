using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private PlayerState currentState;
    public PlayerState CurrentState => currentState;
    public void ChangeState(PlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }
}
