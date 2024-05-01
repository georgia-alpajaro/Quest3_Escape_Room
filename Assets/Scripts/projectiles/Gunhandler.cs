using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;

public class Gunhandler : NetworkBehaviour
{
    PlayerRef shotByPlayerRef;
    string shotByPlayerName;

    TickTimer expiredTickTimer = TickTimer.None;

    NetworkObject networkObject;
    NetworkRigidbody3D networkRigidbody;

    public void shoot(Vector3 shootForce, PlayerRef shotByPlayerRef, string shotByPlayerName)
    {
        networkObject = GetComponent<NetworkObject>();
        networkRigidbody = GetComponent<NetworkRigidbody3D>();

        networkRigidbody.Rigidbody.AddForce(shootForce, ForceMode.Impulse);

        this.shotByPlayerName = shotByPlayerName;
        this.shotByPlayerRef = shotByPlayerRef;

        expiredTickTimer = TickTimer.CreateFromSeconds(Runner, 2);

    }



    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            if (expiredTickTimer.Expired(Runner))
            {
                Runner.Despawn(networkObject);

                expiredTickTimer = TickTimer.None;
            }
        }
    }


    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        
    }
}
