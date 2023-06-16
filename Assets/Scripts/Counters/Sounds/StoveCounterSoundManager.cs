using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSoundManager : MonoBehaviour
{

    public static event EventHandler<OnSizzleModeChangedEventArgs> OnAnySizzleModeChanged;
    public static event EventHandler<OnAnyWarningAlertPlaySoundEventArgs> OnAnyWarningAlertPlaySound;
    public class OnSizzleModeChangedEventArgs : EventArgs {
        public StoveCounter stoveCounter;
        public AudioSource stoveCounterAudioSource;
        public bool isSizzling;
    }
    public class OnAnyWarningAlertPlaySoundEventArgs : EventArgs {
        public StoveCounter stoveCounter;
    }

    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private float warningAlertPlaySoundInterval;

    private float warningAlertPlaySoundTimer;

    private AudioSource stoveCounterAudioSource;

    public static void ResetStaticData() {
        OnAnySizzleModeChanged = null;
        OnAnyWarningAlertPlaySound = null;
    }

    private void Awake() {
        stoveCounterAudioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
        SoundManager.Instance.OnVolumeChanged += SoundManager_OnVolumeChanged;
    }

    private void Update() {
        if (stoveCounter.IsWarningAlertOn()) {
            UpdateWarningAlertPlaySoundTimer();
            if (warningAlertPlaySoundTimer < 0) {
                OnAnyWarningAlertPlaySound?.Invoke(this, new OnAnyWarningAlertPlaySoundEventArgs() {
                    stoveCounter = stoveCounter
                });
                warningAlertPlaySoundTimer = warningAlertPlaySoundInterval;
            }
        } else {
            warningAlertPlaySoundTimer = 0f;
        }
    }

    private void UpdateWarningAlertPlaySoundTimer() {
        warningAlertPlaySoundTimer -= Time.deltaTime;
    }

    private void SoundManager_OnVolumeChanged(object sender, SoundManager.OnVolumeChangedEventArgs e) {
        stoveCounterAudioSource.volume = e.newVolume;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e) {
        if (e.prevState == StoveCounter.State.Idle && (e.nextState == StoveCounter.State.Frying || e.nextState == StoveCounter.State.Burning)) {
            OnAnySizzleModeChanged?.Invoke(this, new OnSizzleModeChangedEventArgs() {
                stoveCounter = stoveCounter,
                stoveCounterAudioSource = stoveCounterAudioSource,
                isSizzling = true
            });
        } else {
            if ((e.prevState == StoveCounter.State.Frying || e.prevState == StoveCounter.State.Burning) && (e.nextState == StoveCounter.State.Idle || e.nextState == StoveCounter.State.Burned)) {
                OnAnySizzleModeChanged?.Invoke(this, new OnSizzleModeChangedEventArgs() {
                    stoveCounter = stoveCounter,
                    stoveCounterAudioSource = stoveCounterAudioSource,
                    isSizzling = false
                });
            }
        }
    }
}
