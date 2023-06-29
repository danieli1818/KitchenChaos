using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{

    public static GameManager Instance { get; private set; }
    
    public enum State {
        WaitingToStart,
        Countdown,
        GamePlaying,
        GameOver
    }

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs {
        public State prevState;
        public State newState;
    }
    public event EventHandler<OnLocalPlayerReadyChangedEventArgs> OnLocalPlayerReadyChanged;
    public class OnLocalPlayerReadyChangedEventArgs : EventArgs {
        public bool isReady;
    }
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnpaused;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;

    [SerializeField] private float countdownTime;
    [SerializeField] private float gamePlayingTime;

    private readonly NetworkVariable<State> state = new(State.WaitingToStart);
    private readonly NetworkVariable<float> timer = new(0f);
    private readonly NetworkVariable<bool> isGamePaused = new(false);
    private bool isLocalPlayerReady;
    private bool isLocalGamePaused;
    private Dictionary<ulong, bool> playersReadyState;
    private Dictionary<ulong, bool> playersPauseState;

    [SerializeField] private bool isDebugMode;
    [SerializeField] private Transform playerPrefabTransform;

    private void Awake() {
        Instance = this;
        isLocalPlayerReady = false;
        playersReadyState = new Dictionary<ulong, bool>();
        playersPauseState = new Dictionary<ulong, bool>();
    }

    private void Start() {
        InputsHandler.Instance.OnPause += InputsHandler_OnPause;
        InputsHandler.Instance.OnInteract += InputsHandler_OnInteract;
    }

    private void InputsHandler_OnInteract(object sender, EventArgs e) {
        if (IsWaitingToStart() && !isLocalPlayerReady) {
            SetIsLocalPlayerReady(true);
        }
    }

    private void SetIsLocalPlayerReady(bool isLocalPlayerReady) {
        this.isLocalPlayerReady = isLocalPlayerReady;
        OnLocalPlayerReadyChanged?.Invoke(this, new OnLocalPlayerReadyChangedEventArgs() {
            isReady = isLocalPlayerReady
        });
        OnPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        playersReadyState[serverRpcParams.Receive.SenderClientId] = true;
        if (IsWaitingToStart() && AreAllPlayersReady()) {
            ChangeState(State.Countdown);
            timer.Value = countdownTime;
        }
    }

    private bool AreAllPlayersReady() {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!playersReadyState.ContainsKey(clientId) || !playersReadyState[clientId]) {
                return false;
            }
        }
        return true;
    }

    private void NetworkManager_OnClientDisconnectServerCallback(ulong clientId) {
        if (playersPauseState.ContainsKey(clientId)) {
            playersPauseState.Remove(clientId);
        }
        if (isGamePaused.Value && !IsAnyPlayerPaused()) {
            isGamePaused.Value = false;
        }
    }

    public override void OnNetworkSpawn() {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePause_OnValueChanged;

        if (IsServer) {
            NetworkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectServerCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += NetworkManagerSceneManager_OnLoadEventCompleted;
        }
    }

    private void NetworkManagerSceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
        foreach (ulong clientId in NetworkManager.ConnectedClientsIds) {
            Transform playerTransform = Instantiate(playerPrefabTransform);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    private void IsGamePause_OnValueChanged(bool previousValue, bool newValue) {
        if (newValue) {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        } else {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    private void State_OnValueChanged(State prevState, State newState) {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs() {
            prevState = prevState,
            newState = newState
        });
    }

    private void InputsHandler_OnPause(object sender, EventArgs e) {
        TogglePause();
    }

    private void Update() {
        if (!IsServer) {
            return;
        }
        switch (state.Value) {
            case State.WaitingToStart:
                break;
            case State.Countdown:
                UpdateTimer();
                if (timer.Value <= 0) {
                    ChangeState(State.GamePlaying);
                    timer.Value = gamePlayingTime;
                }
                break;
            case State.GamePlaying:
                UpdateTimer();
                if (timer.Value <= 0) {
                    ChangeState(State.GameOver);
                    timer.Value = 0f;
                }
                break;
            case State.GameOver:
                break;
        }
    }

    private void UpdateTimer() {
        timer.Value -= Time.deltaTime;
    }

    private void ChangeState(State newState) {
        state.Value = newState;
    }

    public float GetStateTimer() {
        return timer.Value;
    }

    public bool IsGamePlaying() {
        return state.Value == State.GamePlaying;
    }

    public bool IsCountdownToStartActive() {
        return state.Value == State.Countdown;
    }

    public bool IsWaitingToStart() {
        return state.Value == State.WaitingToStart;
    }

    public float GetNormalizedPlayingTimer() {
        return 1 - (timer.Value / gamePlayingTime);
    }

    public bool TogglePause() {
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused) {
            // Time.timeScale = 0f;

            OnPlayerPauseServerRpc();

            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
        } else {
            // Time.timeScale = 1f;

            OnPlayerUnpauseServerRpc();

            OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
        return isLocalGamePaused;
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnPlayerPauseServerRpc(ServerRpcParams serverRpcParams = default) {
        playersPauseState[serverRpcParams.Receive.SenderClientId] = true;

        if (!isGamePaused.Value) {
            isGamePaused.Value = true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnPlayerUnpauseServerRpc(ServerRpcParams serverRpcParams = default) {
        playersPauseState[serverRpcParams.Receive.SenderClientId] = false;

        if (!IsAnyPlayerPaused()) {
            isGamePaused.Value = false;
        }
    }

    private bool IsAnyPlayerPaused() {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (playersPauseState.ContainsKey(clientId) && playersPauseState[clientId]) {
                return true;
            }
        }
        return false;
    }

    public override void OnDestroy() {
        base.OnDestroy();
        if (isGamePaused.Value) {
            Time.timeScale = 1f;
        }
    }

}
