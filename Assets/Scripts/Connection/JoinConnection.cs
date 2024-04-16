using Fusion.Addons.ConnectionManagerAddon;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JoinConnection : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    public void CreateRoom()
    {
        ConnectionManager.Instance.CreateSession(inputField.text);
    }

    public void JoinRoom()
    {
        ConnectionManager.Instance.JoinSession(inputField.text);

    }
}
