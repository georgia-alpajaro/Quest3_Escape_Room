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
    public float radius = 15;


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
        Debug.Log("Escaped!!");
        State = GameState.Escaped;
    }



    public override void FixedUpdateNetwork()
    {
        if (Time.time >= deadline)
        {
            SpawnGhostsAround();
            deadline = Time.time + spawnRate;
            spawnAmount = Random.Range(1, 3);
        }
    }

    private void SpawnGhostsAround()
    {
        bool incorrectSpawn = true;

        for (int i = 0; i < spawnAmount; i++)
        {
            /* Get the spawn position */
            var spawnPos = RandomPointOnCircleEdge(radius);

            /* Now spawn */
            NetworkObject ghost = runner.Spawn(ghostPrefab, position: spawnPos, rotation: Quaternion.identity); //EDIT POSITION ONCE CREATING FINAL SCENE

            /* Adjust height */
            ghost.transform.Translate(new Vector3(0, ghost.transform.localScale.y / 2, 0));


        }
    }

    private Vector3 RandomPointOnCircleEdge(float radius)
    {

        Vector2 vector2 = new Vector2(0, 0);

        //Number of times we will try to search for an empty position
        int searchCount = 10;

        while (searchCount-- > 0)
        {
            vector2 = Random.insideUnitCircle.normalized * radius;

        }

        return new Vector3(vector2.x, 0.5f, vector2.y);
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
