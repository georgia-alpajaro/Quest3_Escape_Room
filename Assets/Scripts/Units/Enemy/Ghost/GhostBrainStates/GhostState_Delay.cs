using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostState_Delay : IState
{
    private float waitForSeconds;
    private float deadline;

    public GhostState_Delay(float waitForSeconds)
    {
        this.waitForSeconds = waitForSeconds;
    }

    public void OnEnter()
    {
        deadline = Time.time + waitForSeconds;

    }

    public void OnExit()
    {
        //Debug.Log("EnemyDelay onExit");

    }

    public void Tick()
    {
    }

    public Color GizmoColor()
    {
        return Color.white;
    }

    public bool IsDone()
    {
        return Time.time >= deadline;
    }
}
