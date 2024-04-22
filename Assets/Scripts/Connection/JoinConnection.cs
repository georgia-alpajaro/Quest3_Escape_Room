using Fusion.Addons.ConnectionManagerAddon;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JoinConnection : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_InputField nameInputField;

    public void CreateRoom()
    {
        ConnectionManager.Instance.CreateSession(inputField.text, nameInputField.text);
    }

    public void JoinRoom()
    {
        ConnectionManager.Instance.JoinSession(inputField.text, nameInputField.text);

    }
}
