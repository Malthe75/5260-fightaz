using System;
using UnityEngine;


public enum RelativeDirection
{
    Neutral,
    Forward,
    Backward
}
public class MoveResolver
{

    public RelativeDirection GetRelativeDirection(float xInput, int facing)
    {
        if(Mathf.Abs(xInput) < 0.1f)
            return RelativeDirection.Neutral;

        // Multiply by facing to get relative direction
        float relative = xInput * facing;

        if (relative > 0)
        {
            return RelativeDirection.Forward;
        }
        else
        {
            return RelativeDirection.Backward;
        }
    }

    public AttackData ResolveAttack(string actionName, float xInput, int facing, MoveMap movemap)
    {
        RelativeDirection dir = GetRelativeDirection(xInput, facing);
        MoveInput moveInput;

        // Parse the action name to MoveInput enum
        if (Enum.TryParse(actionName, ignoreCase: true, out MoveInput input))
        {
            moveInput = input;
            movemap.GetAttack(MoveInput.Hit);
        }
        else
        {
            Debug.LogWarning($"Unknown action: {actionName}");
            return movemap.GetAttack(MoveInput.Nothing);
        }

        // Hits have directional variants
        if (input == MoveInput.Hit)
        {
            switch (dir)
            {
                case RelativeDirection.Neutral:
                    return movemap.GetAttack(MoveInput.Hit);
                case RelativeDirection.Forward:
                    return movemap.GetAttack(MoveInput.Hit_Run_Forward);
                case RelativeDirection.Backward:
                    return movemap.GetAttack(MoveInput.Hit_Run_Backward);
            }
        }

        // Kicks have directional variants
        if (input == MoveInput.Kick)
        {
            switch (dir)
            {
                case RelativeDirection.Neutral:
                    return movemap.GetAttack(MoveInput.Kick);
                case RelativeDirection.Forward:
                    return movemap.GetAttack(MoveInput.Kick_Run_Forward);
                case RelativeDirection.Backward:
                    return movemap.GetAttack(MoveInput.Kick_Run_Backward);
            }
        }
        return movemap.GetAttack(moveInput);

    }

    public AttackData SetAttack(MoveInput moveInput, MoveMap moveMap)
    {
        return moveMap.GetAttack(moveInput);
    }

    

}