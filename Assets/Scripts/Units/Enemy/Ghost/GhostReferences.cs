using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;


//basically a script that all other scripts on the enemy can easily reference at a central location
[DisallowMultipleComponent]
public class GhostReferences : MonoBehaviour
{

    [HideInInspector] public NavMeshAgent navMeshagent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public GameObject[] players;

    public bool attackFinished = false;
    public bool targetMoved = false;
    public Transform target;

    [Header("Stats")]

    public float pathUpdateDelay = 0.2f; //used so that we do not have to compute the navmesh path every frame

    private void Awake()
    {
        navMeshagent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        adjustTarget();
    }

    public void onAttackFinish()
    {
        attackFinished = true;
    }

    public void adjustTarget()
    {
        var random = new System.Random();
        players = GameObject.FindGameObjectsWithTag("Player");
        int idx = random.Next(players.Length);
        target = players[idx].transform;
    }
}
