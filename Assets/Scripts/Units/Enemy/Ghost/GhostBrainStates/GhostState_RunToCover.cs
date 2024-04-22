using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostState_RunToCover : IState
{

    private GhostReferences ghostReferences;
    private CoverArea coverArea;

    public GhostState_RunToCover(GhostReferences ghostReferences, CoverArea coverArea)
    {
        this.ghostReferences = ghostReferences;
        this.coverArea = coverArea;
    }

    public void OnEnter()
    {
        Cover nextCover = this.coverArea.GetRandomCover(ghostReferences.transform.position);
        ghostReferences.navMeshagent.SetDestination(nextCover.transform.position);
        ghostReferences.navMeshagent.stoppingDistance = 0f;

    }

    public void OnExit()
    {
        ghostReferences.animator.SetFloat("Speed", 0f);
    }

    public void Tick()
    {
        ghostReferences.animator.SetFloat("Speed", ghostReferences.navMeshagent.desiredVelocity.sqrMagnitude);
    }

    public Color GizmoColor()
    {
        return Color.blue;
    }

    public bool HasArrivedAtDestination()
    {
        //Debug.Log("remaining Distance: " + ghostReferences.navMeshagent.remainingDistance);
        return ghostReferences.navMeshagent.remainingDistance < 0.1f && ghostReferences.navMeshagent.remainingDistance != 0f;
    }
}
