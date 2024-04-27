using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;


//basically a script that all other scripts on the enemy can easily reference at a central location
[DisallowMultipleComponent]
public class GhostReferences : MonoBehaviour
{

    [HideInInspector] public NavMeshAgent navMeshagent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public GameObject[] players;

    [Networked, OnChangedRender(nameof(UpdateHealth))] public float Health { get; set; }



    public bool attackFinished = false;
    public bool targetMoved = false;
    public Transform target;
    public Image healthBar;


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

    public void onDamagePlayer(float damage)
    {
        Debug.Log("Player Attacked with: " + damage + "Damage");
        target.GetComponent<PlayerStats>().Damage(damage);
    }

    public void adjustTarget()
    {
        var random = new System.Random();
        players = GameObject.FindGameObjectsWithTag("Player");
        int idx = random.Next(players.Length);
        target = players[idx].transform;
        Debug.Log("Target Adjusted");
    }

    public void UpdateHealth()
    {
        healthBar.transform.localScale = new Vector3(Health / 100, 1, 1);
    }

/*    public void Damage(float amount = 10)
    {
        if (HasInputAuthority)
        {
            Health -= amount;
            RPC_GhostHealth(Health);

        }
        //Debug.Log("Player Health: " + Health);
    }*/


    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_GhostHealth(float ghostHealth, RpcInfo info = default)
    {
        Health = ghostHealth;
    }
}
