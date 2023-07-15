using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerReadyStatusManager : NetworkBehaviour
{

    public static PlayerReadyStatusManager Instance { get; private set; }

    public event EventHandler<OnPlayerReadyChangedEventArgs> OnPlayerReadyChanged;
    public class OnPlayerReadyChangedEventArgs : EventArgs {
        public ulong clientId;
        public bool isReady;
    }
    public event EventHandler<OnLocalPlayerReadyChangedEventArgs> OnLocalPlayerReadyChanged;
    public class OnLocalPlayerReadyChangedEventArgs : EventArgs {
        public bool isReady;
    }

    private bool isPlayerReady;
    private Dictionary<ulong, bool> playersReadyState;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Trying to create another instance of the PlayerReadyStatusManager");
            return;
        }
        Instance = this;
        isPlayerReady = false;
        playersReadyState = new Dictionary<ulong, bool>();
    }

    private void SetIsLocalPlayerReady(bool isReady) {
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
        OnPlayerReadyChangedClientRpc(serverRpcParams.Receive.SenderClientId, isReady);

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!playersReadyState.ContainsKey(clientId) || !playersReadyState[clientId]) {
                return;
            }
        }
        StartGame();
    }

    [ClientRpc]
    private void OnPlayerReadyChangedClientRpc(ulong clientId, bool isReady) {
        playersReadyState[clientId] = isReady;
        OnPlayerReadyChanged?.Invoke(this, new OnPlayerReadyChangedEventArgs() {
            clientId = clientId,
            isReady = isReady
        });
    }

    public bool TogglePlayerReadyState() {
        SetIsLocalPlayerReady(!isPlayerReady);
        return isPlayerReady;
    }

    private void StartGame() {
        if (LobbyManager.Instance.GetLobby() != null) {
            LobbyManager.Instance.DestroyLobby(LobbyManager.Instance.GetLobby().Id);
        }
        SceneLoader.LoadNetworkScene(SceneLoader.Scene.GameScene);
    }

    public bool IsLocalPlayerReady() {
        return isPlayerReady;
    }

    public bool IsPlayerReady(ulong clientId) {
        return playersReadyState.ContainsKey(clientId) && playersReadyState[clientId];
    }

}
