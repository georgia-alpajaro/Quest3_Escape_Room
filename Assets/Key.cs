using Fusion.XR.Host.Grabbing;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Key : MonoBehaviour
{
    //detect collision with socket
    public string KeyCode = "goldKey";
    
    private Rigidbody rb;
    private PhysicsGrabbable grabbable;
    private BoxCollider _collider;
    [SerializeField] private Transform attach;
    [SerializeField] private Animator anim;
    public Vector3 offset;
    public Vector3 rotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabbable = GetComponent<PhysicsGrabbable>();
        _collider = GetComponent<BoxCollider>();
    }


    public void OnMatchedSocket(Transform socketAttach)
    {
        Debug.Log($"Key with code {KeyCode} unlocked something!");
        //ungrab and attach both attach transforms
        grabbable.Ungrab();
        grabbable.enabled = false;
        rb.isKinematic = true;
        transform.SetParent(socketAttach);
        transform.SetLocalPositionAndRotation(Vector3.zero + offset, Quaternion.Euler(rotation));
        anim.SetTrigger("Turn");
        _collider.enabled = false;
    }
}
