using Fusion.XR.Host.Grabbing;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.Addons.Physics;

public class Fake_GunHandler : MonoBehaviour
{

    Rigidbody rb;
    private float timer = 0f;
    private float deadline = 0.2f;


    public void shoot(Vector3 shootForce)
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(shootForce, ForceMode.Impulse);

        timer = 0;

    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > deadline)
        {
            Destroy(this.gameObject);

            return;
        }
    }
}
