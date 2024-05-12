using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;


public enum GameState
{
    Waiting,
    Playing,
    GameOver,
    Escaped
}


public class GameManager : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField]
    private NetworkObject ghostPrefab;
    private float spawnRate;
    private int spawnAmount;
    private float deadline;


    private GameObject connectionManager;
    public NetworkRunner runner;


    [Networked, OnChangedRender(nameof(GameStateChanged))] private GameState State { get; set; }



    // Start is called before the first frame update
    void Start()
    {

        
    }

    public override void Spawned()
    {
        connectionManager = GameObject.FindGameObjectWithTag("ConnectionManager");
        runner = connectionManager.GetComponent<NetworkRunner>();
        spawnRate = Random.Range(20f, 40f);
        spawnAmount = Random.Range(1, 3);
        deadline = Time.time + spawnRate;
        State = GameState.Waiting;
        UIManager.Singleton.SetWaitUI(State);
    }

    private void GameStateChanged()
    {
        UIManager.Singleton.SetWaitUI(State);
    }

    public void ChangeToGameOver()
    {
        State = GameState.GameOver;
    }
        
    public void ChangeToEscaped()
    {
        State = GameState.Escaped;
    }



    public override void FixedUpdateNetwork()
    {
        if (Time.time >= deadline)
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                //May need to put a check for inputAuthority here
                //NetworkObject ghost = runner.Spawn(ghostPrefab, position: new Vector3(12f, 1.2f, -7f), rotation: Quaternion.identity); //EDIT POSITION ONCE CREATING FINAL SCENE

            }
            deadline = Time.time + spawnRate;
            spawnAmount = Random.Range(1, 3);
        }


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayerJoined(PlayerRef player)
    {
        
    }

    public void PlayerLeft(PlayerRef player)
    {
        
    }
}
