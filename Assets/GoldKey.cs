using Fusion.XR.Host.Grabbing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldKey : MonoBehaviour
{
    public Vector3 offset;
    public Vector3 rotation;

    public void UnlockWithKey(Transform socketAttach)
    {
        transform.SetParent(socketAttach);
        transform.SetLocalPositionAndRotation(Vector3.zero + offset, Quaternion.Euler(rotation));
    }
}
