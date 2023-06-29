using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerReadyStatusManager : NetworkBehaviour
{

    public static PlayerReadyStatusManager Instance { get; private set; }

    public event EventHandler<OnLocalPlayerReadyChangedEventArgs> OnLocalPlayerReadyChanged;
    public class OnLocalPlayerReadyChangedEventArgs : EventArgs {
        public bool isReady;
    }

    private bool isPlayerReady;
    private Dictionary<ulong, bool> playersReadyState;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Trying to create anoter instance of the PlayerReadyStatusManager");
            return;
        }
        Instance = this;
        isPlayerReady = false;
        playersReadyState = new Dictionary<ulong, bool>();
    }

    private void SetIsLocalPlayerReady(bool isReady) {
        Debug.Log("IsPlayerReady: " + isPlayerReady + " isReady: " + isReady);
        if (isPlayerReady == isReady) {
            return;
        }
        isPlayerReady = isReady;
        OnLocalPlayerReadyChanged?.Invoke(this, new OnLocalPlayerReadyChangedEventArgs() {
            isReady = isPlayerReady
        });
        SetPlayerReadyServerRpc(isPlayerReady);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(bool isReady, ServerRpcParams serverRpcParams = default) {
        playersReadyState[serverRpcParams.Receive.SenderClientId] = isReady;
        Debug.Log("ClientId: " + serverRpcParams.Receive.SenderClientId + " is now " + (isReady ? "" : "not") + " ready!");

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!playersReadyState.ContainsKey(clientId) || !playersReadyState[clientId]) {
                Debug.Log("ClientId: " + clientId + " isn't ready!");
                return;
            }
        }
        StartGame();
    }

    public bool TogglePlayerReadyState() {
        Debug.Log("Toggling Player Ready State: " + !isPlayerReady);
        SetIsLocalPlayerReady(!isPlayerReady);
        return isPlayerReady;
    }

    private void StartGame() {
        SceneLoader.LoadNetworkScene(SceneLoader.Scene.GameScene);
    }

    public bool IsLocalPlayerReady() {
        return isPlayerReady;
    }

}
