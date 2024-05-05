using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;

public class Gunhandler : NetworkBehaviour
{
    PlayerRef shotByPlayerRef;
    string shotByPlayerName;
    NetworkObject shotByNetworkObject;

    [Header("Collision detection")]
    public Transform checkForImpactPoint;
    public LayerMask collisionLayers;

    TickTimer expiredTickTimer = TickTimer.None;

    //Hit info
    List<LagCompensatedHit> hits = new List<LagCompensatedHit>();

    NetworkObject networkObject;
    NetworkRigidbody3D networkRigidbody;


    public void shoot(Vector3 shootForce, PlayerRef shotByPlayerRef, string shotByPlayerName, NetworkObject shotByNetworkObject)
    {
        networkObject = GetComponent<NetworkObject>();
        networkRigidbody = GetComponent<NetworkRigidbody3D>();

        networkRigidbody.Rigidbody.AddForce(shootForce, ForceMode.Impulse);

        this.shotByPlayerName = shotByPlayerName;
        this.shotByPlayerRef = shotByPlayerRef;
        this.shotByNetworkObject = shotByNetworkObject;

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

                return;
            }


            //checking if ball hit anything
            int hitCount = Runner.LagCompensation.OverlapSphere(checkForImpactPoint.position, 0.5f, shotByPlayerRef, hits, collisionLayers, HitOptions.IncludePhysX);



            bool isValidHit = false;

            if (hitCount > 0)
            {
                isValidHit = true;
            }

            for (int i = 0; i < hitCount; i++)
            {
                if (hits[i].Hitbox != null)
                {
                    Debug.Log("We HIT SOMETHING with a HITBOX");

                    //making sure we didn't just hit ourselves
                    if (hits[i].Hitbox.Root.GetBehaviour<NetworkObject>() == shotByNetworkObject)
                        isValidHit = false;
                }
            }

            if (isValidHit)
            {
                hitCount = Runner.LagCompensation.OverlapSphere(checkForImpactPoint.position, 0.5f, shotByPlayerRef, hits, collisionLayers, HitOptions.None);

                //add dealing damage to enemy
                for (int i = 0; i < hitCount; i++)
                {

                    HPHandler hpHandler = hits[i].Hitbox.transform.root.GetComponent<HPHandler>();

                    if (hpHandler != null)
                    {
                        hpHandler.OnTakeDamage(shotByPlayerName, 10);
                    }
                }

                Runner.Despawn(networkObject);
            }
        }

    }


    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        
    }
}
