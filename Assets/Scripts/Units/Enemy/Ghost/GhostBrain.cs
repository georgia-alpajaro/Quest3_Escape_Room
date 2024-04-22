using System;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GhostBrain : NetworkBehaviour
{

    private GhostReferences ghostReferences;
    private StateMachine stateMachine;
    private Vector3 lastPos;


    public override void Spawned()
    {
        ghostReferences = GetComponent<GhostReferences>();
        stateMachine = new StateMachine();
        lastPos = ghostReferences.target.transform.position;


        CoverArea coverArea = FindObjectOfType<CoverArea>();

        //STATES
        var runToCover = new GhostState_RunToCover(ghostReferences, coverArea);
        var delayAfterRun = new GhostState_Delay(2f);
        var cover = new GhostState_Cover(ghostReferences, UnityEngine.Random.Range(1f, 4f));
        var runToPlayer = new GhostState_FollowPlayer(ghostReferences);
        var combat = new GhostState_Combat(ghostReferences, UnityEngine.Random.Range(10f, 20f));


        //TRANSITIONS
        At(runToCover, delayAfterRun, () => runToCover.HasArrivedAtDestination());
        At(delayAfterRun, cover, () => delayAfterRun.IsDone());
        At(cover, runToPlayer, () => cover.IsDone());
        At(runToPlayer, combat, () => runToPlayer.inRange());
        At(combat, runToCover, () => combat.IsDone());

        //START STATE
        stateMachine.SetState(runToCover);

        //Helper Functions
        void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);
    }
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        

    }
    public override void FixedUpdateNetwork()
    {
        
        if(ghostReferences.target != null)
        {
            if (ghostReferences.target.transform.position != lastPos)
            {
                ghostReferences.targetMoved = true;
            }
            else
            {
                ghostReferences.targetMoved = false;
            }

            lastPos = ghostReferences.target.transform.position;
        } else
        {
            ghostReferences.adjustTarget();
        }
        stateMachine.Tick();

    }


    private void OnDrawGizmos()
    {
        if( stateMachine != null )
        {
            Gizmos.color = stateMachine.GetGizmoColor();
            Gizmos.DrawSphere(transform.position + Vector3.up * 3, 0.4f);
        }    
    }

}
