using Fusion.Sockets;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

namespace Fusion.Addons.ConnectionManagerAddon
{
    /**
     * 
     * Handles:
     * - connection launch (either with room name or matchmaking session properties)
     * - user representation spawn on connection
     **/
    public class ConnectionManager : MonoBehaviour, INetworkRunnerCallbacks
    {

        public static ConnectionManager Instance { get; private set; }

        [System.Flags]
        public enum ConnectionCriterias
        {
            RoomName = 1,
            SessionProperties = 2
        }

        [System.Serializable]
        public struct StringSessionProperty
        {
            public string propertyName;
            public string value;
        }

        [Header("Room configuration")]
        public GameMode gameMode = GameMode.Shared;
        public string roomName = "SampleFusion";
        [Tooltip("Set it to 0 to use the DefaultPlayers value, from the Global NetworkProjectConfig (simulation section)")]
        public int playerCount = 0;

        [Header("Room selection criteria")]
        public ConnectionCriterias connectionCriterias = ConnectionCriterias.RoomName;
        [Tooltip("If connectionCriterias include SessionProperties, additionalSessionProperties (editable in the inspector) will be added to sessionProperties")]
        public List<StringSessionProperty> additionalSessionProperties = new List<StringSessionProperty>();
        public Dictionary<string, SessionProperty> sessionProperties;

        [Header("Fusion settings")]
        [Tooltip("Fusion runner. Automatically created if not set")]
        public NetworkRunner runner;
        public INetworkSceneManager sceneManager;

        [Header("Local user spawner")]
        public NetworkObject userPrefab;

        [Header("Event")]
        public UnityEvent onWillConnect = new UnityEvent();

        [Header("Info")]
        public List<StringSessionProperty> actualSessionProperties = new List<StringSessionProperty>();

        // Dictionary of spawned user prefabs, to store them on the server for host topology, and destroy them on disconnection (for shared topology, use Network Objects's "Destroy When State Authority Leaves" option)
        private Dictionary<PlayerRef, NetworkObject> _spawnedUsers = new Dictionary<PlayerRef, NetworkObject>();

        bool ShouldConnectWithRoomName => (connectionCriterias & ConnectionManager.ConnectionCriterias.RoomName) != 0;
        bool ShouldConnectWithSessionProperties => (connectionCriterias & ConnectionManager.ConnectionCriterias.SessionProperties) != 0;

        public GameObject sessionCreateJoinCanvas;
        [SerializeField]
        private GameObject uIHelpers;
        [SerializeField]
        private NetworkObject ghostPrefab;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }

            // Check if a runner exist on the same game object
            if (runner == null) runner = GetComponent<NetworkRunner>();
        }

        private async void Start()
        {
            // Launch the connection at start
            //if (connectOnStart) await Connect();
        }

        

        Dictionary<string, SessionProperty> AllConnectionSessionProperties
        {
            get
            {
                var propDict = new Dictionary<string, SessionProperty>();
                actualSessionProperties = new List<StringSessionProperty>();
                if (sessionProperties != null)
                {
                    foreach (var prop in sessionProperties)
                    {
                        propDict.Add(prop.Key, prop.Value);
                        actualSessionProperties.Add(new StringSessionProperty { propertyName = prop.Key, value = prop.Value });
                    }
                }
                if (additionalSessionProperties != null)
                {
                    foreach (var additionalProperty in additionalSessionProperties)
                    {
                        propDict[additionalProperty.propertyName] = additionalProperty.value;
                        actualSessionProperties.Add(additionalProperty);
                    }

                }
                return propDict;
            }
        }

        public virtual NetworkSceneInfo CurrentSceneInfo()
        {
            var activeScene = SceneManager.GetActiveScene();
            SceneRef sceneRef = default;

            if (activeScene.buildIndex < 0 || activeScene.buildIndex >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogError("Current scene is not part of the build settings");
            }
            else
            {
                sceneRef = SceneRef.FromIndex(activeScene.buildIndex);
            }

            var sceneInfo = new NetworkSceneInfo();
            if (sceneRef.IsValid)
            {
                sceneInfo.AddSceneRef(sceneRef, LoadSceneMode.Single);
            }
            return sceneInfo;
        }

        public async void CreateSession(string roomCode, string playerName)
        {

            Debug.Log("Create Session, Player Name is: " + playerName);

            //ConnectSession
            await Connect(roomCode, GameMode.Host, playerName);



        }

        public async void JoinSession(string roomCode, string playerName)
        {


            Debug.Log("Join Session, Player Name is: " + playerName);

            //ConnectSession
            await Connect(roomCode, GameMode.Client, playerName );

        }

        public async Task LoadScene(string sceneName)
        {
            if (runner.IsServer)
            {
                runner.LoadScene(sceneName);
            }
        }




        public async Task Connect(string nameOfRoom, GameMode gM, string playerName)
        {

            // Create the scene manager if it does not exist
            if (sceneManager == null) sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();
            if (onWillConnect != null) onWillConnect.Invoke();

            // Start or join (depends on gamemode) a session with a specific name
            var args = new StartGameArgs()
            {
                GameMode = gM,
                SessionName = nameOfRoom,
                SessionProperties = AllConnectionSessionProperties,
                Scene = CurrentSceneInfo(),
                SceneManager = sceneManager
            };
            // Room details
            if (playerCount > 0)
            {
                args.PlayerCount = playerCount;
            }

            sessionCreateJoinCanvas.transform.position = new Vector3(500, 500, 500);
            uIHelpers.SetActive(false);


            await runner.StartGame(args);

            string prop = "";
            if (runner.SessionInfo.Properties != null && runner.SessionInfo.Properties.Count > 0)
            {
                prop = "SessionProperties: ";
                foreach (var p in runner.SessionInfo.Properties) prop += $" ({p.Key}={p.Value.PropertyValue}) ";
            }
            Debug.Log($"Session info: Room name {runner.SessionInfo.Name}. Region: {runner.SessionInfo.Region}. {prop}");
            if ((connectionCriterias & ConnectionManager.ConnectionCriterias.RoomName) == 0)
            {
                roomName = runner.SessionInfo.Name;
            }

        }

        #region Player spawn
        public void OnPlayerJoinedSharedMode(NetworkRunner runner, PlayerRef player)
        {
            if (player == runner.LocalPlayer && userPrefab != null)
            {
                // Spawn the user prefab for the local user
                NetworkObject networkPlayerObject = runner.Spawn(userPrefab, position: transform.position, rotation: transform.rotation, player, (runner, obj) => {
                });
            }
        }

        public void OnPlayerJoinedHostMode(NetworkRunner runner, PlayerRef player)
        {
            // The user's prefab has to be spawned by the host
            if (runner.IsServer && userPrefab != null)
            {
                Debug.Log($"OnPlayerJoined. PlayerId: {player.PlayerId}");
                // We make sure to give the input authority to the connecting player for their user's object
                NetworkObject networkPlayerObject = runner.Spawn(userPrefab, position: transform.position, rotation: transform.rotation, inputAuthority: player, (runner, obj) => {
                });


                // Keep track of the player avatars so we can remove it when they disconnect
                _spawnedUsers.Add(player, networkPlayerObject);
            }
        }

        // Despawn the user object upon disconnection
        public void OnPlayerLeftHostMode(NetworkRunner runner, PlayerRef player)
        {
            // Find and remove the players avatar (only the host would have stored the spawned game object)
            if (_spawnedUsers.TryGetValue(player, out NetworkObject networkObject))
            {
                runner.Despawn(networkObject);
                _spawnedUsers.Remove(player);
            }
        }

        #endregion

        #region INetworkRunnerCallbacks
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if(runner.Topology == Topologies.ClientServer)
            {
                OnPlayerJoinedHostMode(runner, player);
                NetworkObject ghost = runner.Spawn(ghostPrefab, position: new Vector3(12f, 1.2f, -7f), rotation: Quaternion.identity); 
                //Instantiate(ghostPrefab, new Vector3(12f, 1.2f, -7f), Quaternion.identity );
                //ghost.GetComponent<GhostController>().target = GameObject.FindGameObjectWithTag("Player").transform;
            }
            else
            {
                OnPlayerJoinedSharedMode(runner, player);
            }
        }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
            if (runner.Topology == Topologies.ClientServer)
            {
                OnPlayerLeftHostMode(runner, player);
            }
        }
        #endregion

        #region INetworkRunnerCallbacks (debug log only)
        public void OnConnectedToServer(NetworkRunner runner) {
            Debug.Log("OnConnectedToServer");

        }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log("Shutdown: " + shutdownReason);
        }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {
            Debug.Log("OnDisconnectedFromServer: "+ reason);
        }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {
            Debug.Log("OnConnectFailed: " + reason);
        }
        #endregion

        #region Unused INetworkRunnerCallbacks 

        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        #endregion
    }

}
