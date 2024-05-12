using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//On collision check for key script, then for correct KeyCode -- if correct, trigger OnMatchedSocket() on key to ungrab it, play animation
//then here play sound, unlock drawer, disable socket
public class Socket : MonoBehaviour
{
    [SerializeField] private string acceptedKeyCode;
    [SerializeField] private Transform socketAttach;
    public UnityEvent OnMatchedKey;

    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        Debug.Log("AA Detected collision!");
        if (other.CompareTag("Key"))
        {
            Key key = other.gameObject.GetComponent<Key>();
            Debug.Log($"AA correct tag! Key is {key}");
            if (key.KeyCode == acceptedKeyCode)
            {
                Debug.Log($"AA Correct key code! {key.KeyCode}");
                key.OnMatchedSocket(socketAttach);
                OnMatchedKey.Invoke();
            }
        }
    }
}
