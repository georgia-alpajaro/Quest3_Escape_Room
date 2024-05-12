using Fusion;
using Fusion.Addons.ConnectionManagerAddon;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JoinConnection : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private NetworkRunner runner;

    private void Awake()
    {
        /*
        runner = ConnectionManager.Instance.GetComponent<NetworkRunner>();
        if (runner.IsRunning)
        {
            HideCanvas();
        }
        */
    }

    public void CreateRoom()
    {
        ConnectionManager.Instance.CreateSession(inputField.text, nameInputField.text);
        HideCanvas();
    }

    public void JoinRoom()
    {
        ConnectionManager.Instance.JoinSession(inputField.text, nameInputField.text);
        HideCanvas();

    }

    private void HideCanvas()
    {
        transform.position = new Vector3(500, 500, 500);
    }
}
