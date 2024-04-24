using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using Fusion.Addons.ConnectionManagerAddon;
using System;

public class PlayerStats : NetworkBehaviour
{

    [Networked] public NetworkString<_32> PlayerName { get; set; }
    [Networked] public bool NameChanged { get; set; }



    [SerializeField] TextMeshPro playerNameLabel;

    private ChangeDetector _changeDetector;

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        if (HasStateAuthority)
        {
            PlayerName = ConnectionManager.Instance._playerName;
        }
        PlayerName = ConnectionManager.Instance._playerName;
        Debug.Log("Player Stats, Player Name: " +  PlayerName);
        playerNameLabel.text = PlayerName.ToString();


    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(NameChanged):
                    PlayerName = ConnectionManager.Instance._playerName;
                    break;
            }
        }
/*        playerNameLabel.text = PlayerName.ToString();
        Debug.Log("Player Name is: " + PlayerName);*/

    }

}
