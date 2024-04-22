using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostState_Attack : IState
{
    private GhostReferences ghostReferences;
    private StateMachine stateMachine;

    public GhostState_Attack(GhostReferences ghostReferences)
    {
        this.ghostReferences = ghostReferences;
    }

    public void OnEnter()
    {
        ghostReferences.animator.SetTrigger("attack");

    }

    public void OnExit()
    {
        ghostReferences.attackFinished = false;
    }

    public void Tick()
    {
    }

    public Color GizmoColor()
    {
        return Color.red;
    }

    public bool attackFinished()
    {
        return ghostReferences.attackFinished;
    }
}
