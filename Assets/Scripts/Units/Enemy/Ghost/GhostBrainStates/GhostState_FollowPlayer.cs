using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GhostState_FollowPlayer : IState
{
    private GhostReferences ghostReferences;
   

    public GhostState_FollowPlayer(GhostReferences ghostReferences)
    {
        this.ghostReferences = ghostReferences;
    }

    public void OnEnter()
    {
        //Debug.Log("Run to Player Entered");
        ghostReferences.navMeshagent.SetDestination(ghostReferences.target.transform.position);
        ghostReferences.navMeshagent.stoppingDistance = 2f;

    }

    public void OnExit()
    {
       // Debug.Log("Run to Player Exited");
        ghostReferences.animator.SetFloat("Speed", 0f);
    }

    public void Tick()
    {
        ghostReferences.animator.SetFloat("Speed", ghostReferences.navMeshagent.desiredVelocity.sqrMagnitude);
        Vector3 lookPos = ghostReferences.target.position - ghostReferences.transform.position; //for our direction vector
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        ghostReferences.transform.rotation = Quaternion.Slerp(ghostReferences.transform.rotation, rotation, 0.2f); //so ghost gradually turns and doesnt instantly turn


        if(ghostReferences.targetMoved)
        {
            adjustDestination();

        }
    }

    private void adjustDestination()
    {
        ghostReferences.navMeshagent.SetDestination(ghostReferences.target.transform.position);
    }

    public Color GizmoColor()
    {
        return Color.yellow;
    }


    public bool inRange() 
    {
        //Debug.Log("Distance from Player: " + ghostReferences.navMeshagent.remainingDistance);
        if (ghostReferences.navMeshagent.remainingDistance != 0) //weird bug where I have to do this, since when I first setDestination, for a couple frames the target position is 0 so ghost automatically exits this state
        {
            return ghostReferences.navMeshagent.remainingDistance < 2f;

        }
        else
        {
            return false;
        }
    }
}
