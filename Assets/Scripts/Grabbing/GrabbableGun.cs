using Fusion.XR.Host.Grabbing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Fusion;
using System;

public class GrabbableGun : NetworkBehaviour
{

    NetworkGrabber currentPlayerGrabber;
    NetworkObject networkObject;
    bool isGrabbed = false;

    public Gunhandler ballPrefab;
    public Fake_GunHandler fakeBallPrefab;
    public float velocity = 50;

    bool fire = false;


    TickTimer fireDelay = TickTimer.None;

    private float fakeTimer = 0f;
    private float fakeDeadline = 0.5f;

    private void Awake()
    {
        var grabbable = GetComponentInParent<NetworkGrabbable>();
        networkObject = GetComponentInParent<NetworkObject>();
        grabbable.onDidGrab.AddListener(OnDidGrab);
        grabbable.onDidUngrab.AddListener(OnDidUngrab);
    }

    private void DebugLog(string debug)
    {
        Debug.Log(debug);
    }

    //when time, check which hand is grabbing during OnDidGrab and set isLeftHand bool so that we only check correct trigger

    public override void FixedUpdateNetwork()
    {
        if (isGrabbed && Object.HasInputAuthority)
        {
            if ((OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0.9f || OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger) > 0.9f) && !fire)
            {
                DebugLog($"{gameObject.name} fired by {currentPlayerGrabber.Object.InputAuthority} {currentPlayerGrabber.hand.side} hand");

                fire = true;
                RPC_PlayerShoot();
                
                
            }
            if (fire && (OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0.9f || OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger) > 0.9f))
            {
                fire = false;
            }
        }

    }

    //for Local use only, used to spawn fake bullet on client side \
    //currently disabled it because there is some offset between the real and fake bullets, so not worth my time to fix
    private void Update()
    {
/*        fakeTimer += Time.deltaTime;

        if (isGrabbed && Object.HasInputAuthority && fakeTimer > fakeDeadline)
        {
            if ((OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0.9f || OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger) > 0.9f) && !fire)
            {

                fire = true;
                fireFakeBullet(transform.TransformDirection(new Vector3(0, 0, velocity)));

            }
            if (fire && (OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0.9f || OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger) > 0.9f))
            {
                fire = false;
            }
        }*/

        
    }

    private void fireFakeBullet(Vector3 fireForce)
    {
        if (!Object.HasStateAuthority)
        {

            Fake_GunHandler fakeBullet = Instantiate(fakeBallPrefab, transform.position, transform.rotation);
            fakeBullet.shoot(fireForce);

            fakeTimer = 0f;

        }

    }

    void OnDidUngrab()
    {
        isGrabbed = false;
/*
        currentPlayerGrabber.GetComponent<PlayerStats>().isGrabbing = false;*/
        DebugLog($"{gameObject.name} ungrabbed");
    }

    void OnDidGrab(NetworkGrabber newGrabber)
    {
        currentPlayerGrabber = newGrabber;
        isGrabbed = true;
        DebugLog($"{gameObject.name} grabbed by {newGrabber.Object.InputAuthority} {newGrabber.hand.side} hand");
/*        currentPlayerGrabber.GetComponent<PlayerStats>().isGrabbing = true;
        currentPlayerGrabber.GetComponent<PlayerStats>().grabbableName = gameObject.name;*/
    }

    void fireGun(Vector3 fireForce)
    {
        if (fireDelay.ExpiredOrNotRunning(Runner))
        {
            Debug.Log("SPAWNING PROJECTILE");
            Runner.Spawn(ballPrefab, transform.position, transform.rotation, Object.InputAuthority, (runner, spawnedProjectile) =>
            {
                spawnedProjectile.GetComponent<Gunhandler>().shoot(fireForce, currentPlayerGrabber.Object.InputAuthority, currentPlayerGrabber.Object.InputAuthority.ToString(), networkObject);
            });

            //To avoid spamming the projectiles, could be removed later or just lower the delay
            fireDelay = TickTimer.CreateFromSeconds(Runner, 0.5f);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_PlayerShoot( RpcInfo info = default)
    {
        Debug.Log("RPC_PlayerShoot CALLED");
        RPC_FireGun();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_FireGun()
    {
        Debug.Log("RPC_FIREGUN CALLED");

        fireGun(transform.TransformDirection(new Vector3(0, 0, velocity)));

    }


}
