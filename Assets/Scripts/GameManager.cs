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
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnpaused;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;
    public event EventHandler<OnLocalPlayerReadyChangedEventArgs> OnLocalPlayerReadyChanged;
    public class OnLocalPlayerReadyChangedEventArgs : EventArgs {
        public bool isReady;
    }

    [SerializeField] private float countdownTime;
    [SerializeField] private float gamePlayingTime;

    private readonly NetworkVariable<State> state = new(State.WaitingToStart);
    private readonly NetworkVariable<float> timer = new(0f);
    private readonly NetworkVariable<bool> isGamePaused = new(false);
    private bool isLocalGamePaused;
    private bool isPlayerReady;
    private Dictionary<ulong, bool> playersReadyState;
    private Dictionary<ulong, bool> playersPauseState;

    [SerializeField] private bool isDebugMode;

    private void Awake() {
        Instance = this;
        isPlayerReady = false;
        playersReadyState = new Dictionary<ulong, bool>();
        playersPauseState = new Dictionary<ulong, bool>();
    }

    private void Start() {
        InputsHandler.Instance.OnPause += InputsHandler_OnPause;
        InputsHandler.Instance.OnInteract += InputsHandler_OnInteract;
    }

    public override void OnNetworkSpawn() {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePause_OnValueChanged;
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

    private void InputsHandler_OnInteract(object sender, EventArgs e) {
        if (state.Value == State.WaitingToStart) {
            SetIsLocalPlayerReady(true);
        }
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

        foreach (ulong clientId in NetworkManager.ConnectedClientsIds) {
            if (!playersReadyState.ContainsKey(clientId) || !playersReadyState[clientId]) {
                return;
            }
        }
        ChangeState(State.Countdown);
        timer.Value = countdownTime;
    }

}
