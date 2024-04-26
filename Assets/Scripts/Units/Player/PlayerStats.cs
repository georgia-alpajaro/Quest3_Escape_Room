using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using Fusion.Addons.ConnectionManagerAddon;
using System;

public class PlayerStats : NetworkBehaviour
{

    [Networked] public string PlayerName { get; set; }
    public GameObject nameInputField;



    [SerializeField] TextMeshPro playerNameLabel;

    private ChangeDetector _changeDetector;




    public override void Spawned()
    {
        nameInputField = GameObject.FindGameObjectWithTag("PlayerNameInput");
        string nameInput = nameInputField.GetComponent<TMP_InputField>().text;
        print("The Name input is: " + nameInput);

        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        if (HasInputAuthority)
        {
            PlayerName = nameInput;
            RPC_PlayerName(PlayerName);


        }
        playerNameLabel.text = PlayerName;



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


}
