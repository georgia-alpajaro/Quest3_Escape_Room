using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    
    public static UIManager Singleton
    {

        get => _singleton;
        set
        {
            if (value == null)
                _singleton = null;
            else if(_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Destroy(value);
                Debug.LogError($"There should only ever be one instance of {nameof(UIManager)}");
            }
        }
    }

    private static UIManager _singleton;

    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private Image panel;


    private void Awake()
    {
        Singleton = this;
    }

    private void OnDestroy()
    {
        if(Singleton == this)
        {
            Singleton = null;
        }
    }

    public void SetWaitUI(GameState newState)
    {
        if (newState  == GameState.GameOver)
        {
            gameOverText.text = "GAME OVER";
            instructionText.text = "You have failed, you must restart the game manually because we are dissapointed in you";
        }

        gameOverText.enabled = newState == GameState.GameOver;
        instructionText.enabled = newState == GameState.GameOver;
        panel.enabled = newState == GameState.GameOver;
    }
}
