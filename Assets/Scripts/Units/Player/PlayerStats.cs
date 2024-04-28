using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using Fusion.Addons.ConnectionManagerAddon;
using System;
using UnityEngine.UI;

public class PlayerStats : NetworkBehaviour
{

    [Networked] public string PlayerName { get; set; }
    public GameObject nameInputField;

    [Networked, OnChangedRender(nameof(UpdateHealth))] public float Health { get; set; } = 100;



    [SerializeField] TextMeshPro playerNameLabel;

    private ChangeDetector _changeDetector;


    public Image healthBar;



    public override void Spawned()
    {
        nameInputField = GameObject.FindGameObjectWithTag("PlayerNameInput");
        string nameInput = nameInputField.GetComponent<TMP_InputField>().text;

        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        if (HasInputAuthority)
        {
            PlayerName = nameInput;
            RPC_PlayerName(PlayerName);
            RPC_PlayerHealth(Health);
        }
        playerNameLabel.text = PlayerName;
        UpdateHealth();
        

    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {
            Damage(10);
        }
    }

    


    public void UpdateHealth()
    {
        //Debug.Log("UPDATE HEALTH CALLED");
        healthBar.transform.localScale = new Vector3(Health / 100, 1, 1);
    }

    public void Damage(float amount = 10)
    {
        if (HasInputAuthority)
        {
            Debug.Log("HasINputAuthority for damaged method");
            Health -= amount;
            RPC_PlayerHealth(Health);

        }
        //Debug.Log("Player Health: " + Health);
    }

    public void Attack()
    {

    }



    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(PlayerName):
                    playerNameLabel.text = PlayerName;
                    break;

            }
        }

    }


    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_PlayerName(string playerName, RpcInfo info = default)
    {
        PlayerName = playerName;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_PlayerHealth(float playerHealth, RpcInfo info = default)
    {
        Health = playerHealth;
    }

}
