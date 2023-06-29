using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    
    public static MultiplayerManager Instance { get; private set; }

    public event EventHandler OnStartingHost;
    public event EventHandler OnTryingToConnect;
    public event EventHandler OnClientDisconnected;

    private int maxPlayers;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Trying to create more than one instance of MultiplayerManager!");
            return;
        }
        Instance = this;

        DontDestroyOnLoad(this);
    }

    public void StartHost(int maxPlayers = 4) {
        if (maxPlayers <= 0) {
            Debug.LogError("Invalid max players amount, smaller or equal to 0");
            return;
        }
        this.maxPlayers = maxPlayers;
        OnStartingHost?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallbackMaxPlayersAndGameHasStarted;
        NetworkManager.Singleton.StartHost();
        SceneLoader.LoadNetworkScene(SceneLoader.Scene.CharacterSelectionScene);
    }

    private void NetworkManager_ConnectionApprovalCallbackMaxPlayersAndGameHasStarted(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
        if (response.Reason != null && response.Reason != "") {
            return;
        }
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= maxPlayers) {
            response.Approved = false;
            response.Reason = "The game is full";
        } else if (!SceneLoader.IsCurrentScene(SceneLoader.Scene.CharacterSelectionScene)) {
            response.Approved = false;
            response.Reason = "The game has already started";
        } else {
            response.Approved = true;
        }
    }

    public void StartClient() {
        OnTryingToConnect?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        OnClientDisconnected?.Invoke(this, EventArgs.Empty);
    }

    public void ShutdownAndDestroyNetworkManager() {
        Debug.Log("Destroying Network Manager and Multiplayer Manager!");
        Destroy(NetworkManager.Singleton.gameObject);
        NetworkManager.Singleton.Shutdown();
        Destroy(gameObject);
    }

}
