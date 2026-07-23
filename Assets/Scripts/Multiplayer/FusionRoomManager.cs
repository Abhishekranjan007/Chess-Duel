using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Threading.Tasks;


public enum PlayerSide
{
    None,
    White,
    Black
}

public class FusionRoomManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static FusionRoomManager Instance;
    private NetworkRunner runner;

    [SerializeField]
    private NetworkPrefabRef networkPlayerPrefab;
    [SerializeField]
    private NetworkPrefabRef networkGameManagerPrefab;

    private NetworkGameManager gameManager;

    private List<SessionInfo> availableSessions = new List<SessionInfo>();

    // Players who joined before GameScene was loaded are queued here.
    // The host spawns their objects once OnSceneLoadDone fires for GameScene.
    private List<PlayerRef> pendingPlayers = new List<PlayerRef>();
    private bool gameSceneLoaded = false;
    private bool isLeavingGame = false;

    public bool ConnectedSuccessfully { get; private set; }
    
    // Add this field to FusionRoomManager
    private PlayerSide localPlayerSide = PlayerSide.None;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async void Connect(string roomName)
    {
        // Always create a fresh runner — Fusion does not allow reusing runners.
        // DestroyImmediate ensures the old runner is fully gone before
        // AddComponent creates a new one. Destroy() is deferred and can
        // cause "runner should not be reused" if Connect() is called again
        // before end of frame.
        NetworkRunner existingRunner = GetComponent<NetworkRunner>();
        if (existingRunner != null)
            DestroyImmediate(existingRunner);

        runner = gameObject.AddComponent<NetworkRunner>();

        runner.ProvideInput = true;

        runner.AddCallbacks(this);

        //Debug.Log("Connecting to room : " + roomName);

        var result = await runner.StartGame(
            new StartGameArgs()
            {
                GameMode = GameMode.Shared,

                SessionName = roomName,

                Scene = SceneRef.FromIndex(
                    SceneManager.GetActiveScene().buildIndex),

                SceneManager =
                    gameObject.AddComponent<NetworkSceneManagerDefault>()
            });

        if (result.Ok)
        {
            ConnectedSuccessfully = true;

            //Debug.Log("Connected Successfully");
        }
        else
        {
            Debug.LogError(result.ShutdownReason);
        }
    }

    // =========================
    // PLAYER EVENTS
    // =========================

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        //Debug.Log("[Fusion] Player Joined : " + player);

        if (!runner.IsSharedModeMasterClient)
            return;

        // Queue the player. We cannot spawn anything yet because
        // GameScene may not be loaded on both clients. Spawning happens
        // in OnSceneLoadDone once the host confirms GameScene is ready.
        pendingPlayers.Add(player);

        if (gameSceneLoaded)
            SpawnPendingPlayers();
    }

    // Called by MatchmakingManager only on the host after opponent is found.
    public async void LoadGameScene()
    {
        if (runner == null)
            return;

        //Debug.Log("Host loading GameScene...");

        await runner.LoadScene(
            SceneRef.FromIndex(
                SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/GameScene.unity")));
    }

    // Spawns NetworkGameManager + NetworkPlayer for every queued player.
    // Only ever called on the host, only after GameScene is confirmed loaded.
    private void SpawnPendingPlayers()
    {
        if (networkPlayerPrefab.Equals(NetworkPrefabRef.Empty))
        {
            Debug.LogError("[Fusion] NetworkPlayer Prefab NOT assigned!");
            return;
        }

        if (gameManager == null)
        {
            NetworkObject obj = runner.Spawn(
                networkGameManagerPrefab,
                Vector3.zero,
                Quaternion.identity);

            gameManager = obj.GetComponent<NetworkGameManager>();

            //Debug.Log("[Fusion] NetworkGameManager Spawned");
        }

        foreach (PlayerRef player in pendingPlayers)
        {
            //string playerName = "Player_" + player.PlayerId;
            string playerName = PlayerPrefs.GetString(Constants.PlayerName, Constants.GuestPlayer);

            gameManager.AssignPlayer(player, playerName);

            //Debug.Log("[Fusion] Spawning NetworkPlayer for " + player);

            runner.Spawn(
                networkPlayerPrefab,
                Vector3.zero,
                Quaternion.identity,
                player);
        }

        SetPlayerS();

        pendingPlayers.Clear();
    }

    public void SetPlayerS()
    {
        if (gameManager != null && runner != null)
        {
            if (runner.LocalPlayer == gameManager.WhitePlayer)
                localPlayerSide = PlayerSide.White;
            else if (runner.LocalPlayer == gameManager.BlackPlayer)
                localPlayerSide = PlayerSide.Black;

            //Debug.Log("[Fusion 2] ON GAMEENTRY : " + localPlayerSide);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (isLeavingGame)
            return;

        //Debug.Log("[Fusion] Player Left : " + player);

        // Ignore if we're not actually in a multiplayer game
        // Only handle this in GameScene — ignore lobby disconnects
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "GameScene")
            return;

        //Debug.Log("[Fusion 2] Player Left : " + player);

        // Don't trigger if we are the one leaving
        if (player == runner.LocalPlayer)
            return;

        //Debug.Log("[Fusion 3] Player Left : " + localPlayerSide +" :: "+ PlayerSide.White+" :: "+ PlayerSide.Black);

        // Opponent left — local player wins
        // Use cached side because NetworkGameManager may already be despawned
        //if (localPlayerSide == PlayerSide.White)
        //    GameOver.instance.GameEnded(0, "Opponent Left", "9");
        //else if (localPlayerSide == PlayerSide.Black)
        //    GameOver.instance.GameEnded(1, "Opponent Left", "9.1");
        GameOver.instance.UpdateGameOverFlag(true);
        UIManager.Instance.ShowGameOverUI(0, 0);
    }

    public async void ReturnToLobby()
    {
        if (runner != null)
        {
            await runner.Shutdown();
            // Give Photon's servers a moment to clean up the session
            // before we attempt a new connection. Reduces Code 104 errors.
            await System.Threading.Tasks.Task.Delay(500);
        }

        ResetConnectionState();

        SceneManager.LoadScene("Lobby");
    }

    public async Task LeaveMultiplayer()
    {
        if (runner != null)
        {
            await runner.Shutdown();

            Destroy(runner);

            runner = null;
        }

        ConnectedSuccessfully = false;
        gameSceneLoaded = false;

        pendingPlayers.Clear();

        gameManager = null;
    }

    public void ResetConnectionState()
    {
        isLeavingGame = false;
        ConnectedSuccessfully = false;
        gameSceneLoaded = false;
        pendingPlayers.Clear();
        gameManager = null;
        localPlayerSide = PlayerSide.None;

        // Don't destroy runner here — Connect() will replace it cleanly
        runner = null;
    }

    public bool IsHost()
    {
        return runner != null && runner.IsSharedModeMasterClient;
    }

    public int NumberOfPlayers()
    {
        int count = 0;

        foreach (var player in runner.ActivePlayers)
        {
            count++;
        }

        return count;
    }

    public async void LeaveRoom()
    {
        isLeavingGame = true;

        if (runner != null)
        {
            await runner.Shutdown();
        }
    }

    public bool IsConnected()
    {
        return runner != null && runner.IsConnectedToServer;
    }

    public async void Disconnect()
    {
        if (runner != null)
        {
            await runner.Shutdown();
        }
    }

    // =========================
    // REQUIRED CALLBACKS
    // =========================

    public void OnInput(NetworkRunner runner, NetworkInput input) { }

    public void OnInputMissing(
        NetworkRunner runner,
        PlayerRef player,
        NetworkInput input)
    { }

    public void OnShutdown(
        NetworkRunner runner,
        ShutdownReason shutdownReason)
    { }

    public void OnConnectedToServer(
        NetworkRunner runner)
    { }

    public void OnDisconnectedFromServer(
        NetworkRunner runner,
        NetDisconnectReason reason)
    { }

    public void OnConnectRequest(
        NetworkRunner runner,
        NetworkRunnerCallbackArgs.ConnectRequest request,
        byte[] token)
    { }

    public void OnConnectFailed(
        NetworkRunner runner,
        NetAddress remoteAddress,
        NetConnectFailedReason reason)
    { }

    public void OnUserSimulationMessage(
        NetworkRunner runner,
        SimulationMessagePtr message)
    { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        availableSessions = sessionList;

        //Debug.Log("Rooms Found : " + availableSessions.Count);

        foreach (SessionInfo session in availableSessions)
        {
            //Debug.Log($"Room : {session.Name}  Players : {session.PlayerCount}");
        }
    }

    public void OnCustomAuthenticationResponse(
        NetworkRunner runner,
        Dictionary<string, object> data)
    { }

    public void OnHostMigration(
        NetworkRunner runner,
        HostMigrationToken hostMigrationToken)
    { }

    public void OnReliableDataReceived(
        NetworkRunner runner,
        PlayerRef player,
        ReliableKey key,
        ArraySegment<byte> data)
    { }

    public void OnReliableDataProgress(
        NetworkRunner runner,
        PlayerRef player,
        ReliableKey key,
        float progress)
    { }

    // Fired once Fusion confirms ALL clients have finished loading a scene.
    // We guard on GameScene specifically because this also fires when
    // Fusion first connects and syncs the Lobby scene.
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        //Debug.Log("[Fusion] Scene load done. Scene: " + currentScene);

        if (!runner.IsSharedModeMasterClient)
            return;

        if (currentScene != "GameScene")
        {
            //Debug.Log("[Fusion] Ignoring scene load for: " + currentScene);
            return;
        }

        gameSceneLoaded = true;
        SpawnPendingPlayers();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    { }

    public void OnObjectEnterAOI(
        NetworkRunner runner,
        NetworkObject obj,
        PlayerRef player)
    { }

    public void OnObjectExitAOI(
        NetworkRunner runner,
        NetworkObject obj,
        PlayerRef player)
    { }

    //Getters
    public List<SessionInfo> GetAvailableSessions()
    {
        return availableSessions;
    }

    public NetworkGameManager GetNetworkGameManager()
    {
        return gameManager;
    }

    public void SetNetworkGameManager(NetworkGameManager manager)
    {
        gameManager = manager;
    }
}