using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
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
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;

    [SerializeField] private float countdownTime;
    [SerializeField] private float gamePlayingTime;

    private State state;
    private float timer;
    private bool isGamePaused;

    [SerializeField] private bool isDebugMode;

    private void Awake() {
        Instance = this;
        state = State.WaitingToStart;
    }

    private void Start() {
        InputsHandler.Instance.OnPause += InputsHandler_OnPause;
        InputsHandler.Instance.OnInteract += InputsHandler_OnInteract;

        if (isDebugMode) {
            ChangeState(State.Countdown);
            timer = 1f;
        }
    }

    private void InputsHandler_OnInteract(object sender, EventArgs e) {
        if (state == State.WaitingToStart) {
            ChangeState(State.Countdown);
            timer = countdownTime;
        }
    }

    private void InputsHandler_OnPause(object sender, EventArgs e) {
        TogglePause();
    }

    private void Update() {
        switch (state) {
            case State.WaitingToStart:
                break;
            case State.Countdown:
                UpdateTimer();
                if (timer <= 0) {
                    ChangeState(State.GamePlaying);
                    timer = gamePlayingTime;
                }
                break;
            case State.GamePlaying:
                UpdateTimer();
                if (timer <= 0) {
                    ChangeState(State.GameOver);
                    timer = 0f;
                }
                break;
            case State.GameOver:
                break;
        }
    }

    private void UpdateTimer() {
        timer -= Time.deltaTime;
    }

    private void ChangeState(State newState) {
        State prevState = state;
        state = newState;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs() {
            prevState = prevState,
            newState = state
        });
    }

    public float GetStateTimer() {
        return timer;
    }

    public bool IsGamePlaying() {
        return state == State.GamePlaying;
    }

    public bool IsCountdownToStartActive() {
        return state == State.Countdown;
    }

    public float GetNormalizedPlayingTimer() {
        return 1 - (timer / gamePlayingTime);
    }

    public bool TogglePause() {
        isGamePaused = !isGamePaused;
        if (isGamePaused) {
            Time.timeScale = 0f;

            OnGamePaused?.Invoke(this, EventArgs.Empty);
        } else {
            Time.timeScale = 1f;

            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
        return isGamePaused;
    }

}
