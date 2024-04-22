using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostState_Cover : IState
{
    private GhostReferences ghostReferences;
    private StateMachine stateMachine;
    private float waitForSeconds;
    private float deadline;

    public GhostState_Cover(GhostReferences ghostReferences, float waitForSeconds)
    {
        this.ghostReferences = ghostReferences;
        this.waitForSeconds = waitForSeconds;
    }

    public void OnEnter()
    {
        ghostReferences.animator.SetBool("cover", true);
        deadline = Time.time + waitForSeconds;

    }

    public void OnExit()
    {
        ghostReferences.animator.SetBool("cover", false);

    }

    public void Tick()
    {
    }

    public Color GizmoColor()
    {
        return Color.gray;
    }

    public bool IsDone()
    {
        return Time.time >= deadline;
    }
}
