using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using UnityEngine.UI;

public class HPHandler : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHPChanged))] public float HP { get; set; }



    [Networked, OnChangedRender(nameof(OnStateChanged))] public bool isDead { get; set; }

    public Image healthBar;


    bool isInitialized = false;
    public bool isPlayer = false;

    const float startingHP = 100;


    // Start is called before the first frame update
    void Start()
    {
        HP = startingHP;
        isDead = false;
    }

    public void OnTakeDamage(string damageCausedByPlayerName, float damageAmount)
    {

        if (isDead) return;

        HP -= damageAmount;

        Debug.Log($"{Time.time} {transform.name} took damage got {HP} left");
        if (HP <= 0)
        {
            Debug.Log($"{Time.time}{transform.name} died");
             isDead = true;
            GameOver();
        }

    }

    public void GameOver()
    {
        if (isPlayer)
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().ChangeToGameOver();
        }
    }

    public void OnHPChanged()
    {
        Debug.Log($"{Time.time} OnHPChanged value {HP}");
        healthBar.transform.localScale = new Vector3(HP / 100, 1, 1);
    }

    public void OnStateChanged()
    {
        Debug.Log($"{Time.time} OnStateChanged isDead {isDead}");

    }

}
