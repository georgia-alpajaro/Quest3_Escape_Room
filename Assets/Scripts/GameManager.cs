using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private NetworkObject ghostPrefab;
    private float spawnRate;
    private int spawnAmount;
    private float deadline;


    private GameObject connectionManager;
    private NetworkRunner runner;



    // Start is called before the first frame update
    void Start()
    {
        connectionManager = GameObject.FindGameObjectWithTag("connectionManager");
        runner = connectionManager.GetComponent<NetworkRunner>();
        spawnRate = Random.Range(10f, 20f);
        spawnAmount = Random.Range(1, 3);
        deadline = Time.time + spawnRate;

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= deadline)
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                NetworkObject ghost = runner.Spawn(ghostPrefab, position: new Vector3(12f, 1.2f, -7f), rotation: Quaternion.identity);

            }
            deadline = Time.time + spawnRate;
            spawnAmount = Random.Range(1, 3);
        }
    }
}
