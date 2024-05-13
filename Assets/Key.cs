using Fusion.XR.Host.Grabbing;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class Key : MonoBehaviour
{
    //detect collision with socket
    public string KeyCode = "";
    public UnityEvent OnMatch;

    public Vector3 offset;
    public Vector3 rotation;

    public void UnlockWithKey(Transform socketAttach)
    {
        transform.SetParent(socketAttach);
        transform.SetLocalPositionAndRotation(Vector3.zero + offset, Quaternion.Euler(rotation));
    }

    public void OnMatchedSocket(Transform socketAttach)
    {
        Debug.Log($"Key with code {KeyCode} unlocked something!");
        if (KeyCode == "goldKey" || KeyCode == "coin")
        {
            UnlockWithKey(socketAttach);
        }
        OnMatch.Invoke();
    }
}
