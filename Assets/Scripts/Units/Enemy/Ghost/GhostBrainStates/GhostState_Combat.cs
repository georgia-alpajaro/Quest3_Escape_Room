using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class GhostState_Combat : IState
{
    private GhostReferences ghostReferences;
    private StateMachine stateMachine;
    private Transform target;
    private float attackForSeconds;
    private float deadline;

    public GhostState_Combat(GhostReferences ghostReferences, float attackForSeconds)
    {
        this.ghostReferences = ghostReferences;
        this.attackForSeconds = attackForSeconds;

        stateMachine = new StateMachine();

        //STATES
        var delayAfterAttack = new GhostState_Delay(UnityEngine.Random.Range(1f, 4f));
        var attack = new GhostState_Attack(ghostReferences);
        var runToPlayer = new GhostState_FollowPlayer(ghostReferences);



        //TRANSITIONS
        At(attack, delayAfterAttack, () => attack.attackFinished());
        At(delayAfterAttack, attack, () => delayAfterAttack.IsDone());
        Any(runToPlayer, () => ghostReferences.targetMoved);
        At(runToPlayer, attack, () => runToPlayer.inRange());


        //START STATE
        stateMachine.SetState(attack);

        //Helper Functions
        void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);

    }

    public void OnEnter()
    {
        Debug.Log("Entered Combat State");
        ghostReferences.animator.SetBool("combat", true);
        deadline = Time.time + attackForSeconds;
        target = ghostReferences.target;


    }

    public void OnExit()
    {
        Debug.Log("Left Combat State");
        ghostReferences.animator.SetBool("combat", false);
        target = null;

    }

    public void Tick()
    {
        stateMachine.Tick();
    }

    public Color GizmoColor()
    {
        return stateMachine.GetGizmoColor();
    }

    public bool IsDone()
    {
        return Time.time >= deadline;
    }
}
