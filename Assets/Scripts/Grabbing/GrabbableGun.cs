using Fusion.XR.Host.Grabbing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Fusion;

public class GrabbableGun : NetworkBehaviour
{

    NetworkGrabber currentPlayerGrabber;
    bool isGrabbed = false;

    public Gunhandler ballPrefab;
    public float velocity = 50;

    bool fire = false;


    TickTimer fireDelay = TickTimer.None;

    private void Awake()
    {
        var grabbable = GetComponentInParent<NetworkGrabbable>();
        grabbable.onDidGrab.AddListener(OnDidGrab);
        grabbable.onDidUngrab.AddListener(OnDidUngrab);
    }

    private void DebugLog(string debug)
    {
        Debug.Log(debug);
    }



    public override void FixedUpdateNetwork()
    {
        if (isGrabbed && Object.HasInputAuthority)
        {
            if (OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0.9f && fire == false)
            {
                DebugLog($"{gameObject.name} fired by {currentPlayerGrabber.Object.InputAuthority} {currentPlayerGrabber.hand.side} hand");

                fire = true;
                fireGun(transform.TransformDirection(new Vector3(0, 0, velocity)));
                //Rigidbody clone = Instantiate(ballPrefab, transform.position, transform.rotation) as Rigidbody; //may need to use runner.spawn
                
            }
            if (fire && OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) < 0.1f)
            {
                fire = false;
            }
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
            Runner.Spawn(ballPrefab, transform.position, transform.rotation, Object.InputAuthority, (runner, spawnedProjectile) =>
            {
                spawnedProjectile.GetComponent<Gunhandler>().shoot(fireForce, currentPlayerGrabber.Object.InputAuthority, currentPlayerGrabber.Object.InputAuthority.ToString());
            });

            //To avoid spamming the projectiles, could be removed later or just lower the delay
            fireDelay = TickTimer.CreateFromSeconds(Runner, 0.5f);
        }
    }
}
