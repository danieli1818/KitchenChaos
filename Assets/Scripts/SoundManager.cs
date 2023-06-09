using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";

    public static SoundManager Instance { get; private set; }

    public event EventHandler<OnVolumeChangedEventArgs> OnVolumeChanged;
    public class OnVolumeChangedEventArgs : EventArgs {
        public float newVolume;
    }

    [SerializeField] private SoundsSO soundsSO;

    private float volume;

    private void Awake() {
        Instance = this;

        SetVolume(PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f));
    }

    private void Start() {
        CutterCounter.OnAnyCut += CutterCounter_OnAnyCut;
        DeliveryCounter.OnAnySuccessfulDelivery += DeliveryCounter_OnAnySuccessfulDelivery;
        DeliveryCounter.OnAnyIncorrectDelivery += DeliveryCounter_OnAnyIncorrectDelivery;
        PlayerSoundManager.OnAnyFootstepsPlaySound += PlayerSoundManager_OnAnyFootstepsPlaySound;
        BaseCounter.OnAnyDropKitchenObject += BaseCounter_OnAnyDropKitchenObject;
        PlayerController.OnAnyPickupKitchenObject += PlayerController_OnAnyPickupKitchenObject;
        StoveCounterSoundManager.OnAnySizzleModeChanged += StoveCounterSoundManager_OnAnySizzleModeChanged;
        StoveCounterSoundManager.OnAnyWarningAlertPlaySound += StoveCounterSoundManager_OnAnyWarningAlertPlaySound;
        TrashCounter.OnAnyKitchenObjectTrashed += TrashCounter_OnAnyKitchenObjectTrashed;
    }

    private void TrashCounter_OnAnyKitchenObjectTrashed(object sender, System.EventArgs e) {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(soundsSO.GetTrashAudioClips(), trashCounter.transform.position);
    }

    private void StoveCounterSoundManager_OnAnyWarningAlertPlaySound(object sender, StoveCounterSoundManager.OnAnyWarningAlertPlaySoundEventArgs e) {
        PlaySound(soundsSO.GetWarningAudioClips(), e.stoveCounter.transform.position);
    }

    private void StoveCounterSoundManager_OnAnySizzleModeChanged(object sender, StoveCounterSoundManager.OnSizzleModeChangedEventArgs e) {
        if (e.isSizzling) {
            PlayAudio(e.stoveCounterAudioSource, soundsSO.GetPanSizzleAudioClips());
        } else {
            PauseAudio(e.stoveCounterAudioSource);
        }
    }

    private void PlayerController_OnAnyPickupKitchenObject(object sender, System.EventArgs e) {
        PlayerController playerController = sender as PlayerController;
        PlaySound(soundsSO.GetObjectPickupAudioClips(), playerController.transform.position);
    }

    private void BaseCounter_OnAnyDropKitchenObject(object sender, System.EventArgs e) {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(soundsSO.GetObjectDropAudioClips(), baseCounter.transform.position);
    }

    private void PlayerSoundManager_OnAnyFootstepsPlaySound(object sender, PlayerSoundManager.OnAnyFootstepsPlaySoundEventArgs e) {
        PlayerController playerController = e.playerController;
        PlaySound(soundsSO.GetFootstepsAudioClips(), playerController.transform.position);
    }

    private void DeliveryCounter_OnAnyIncorrectDelivery(object sender, System.EventArgs e) {
        DeliveryCounter deliveryCounter = sender as DeliveryCounter;
        PlaySound(soundsSO.GetDeliveryFailAudioClips(), deliveryCounter.transform.position);
    }

    private void DeliveryCounter_OnAnySuccessfulDelivery(object sender, System.EventArgs e) {
        DeliveryCounter deliveryCounter = sender as DeliveryCounter;
        PlaySound(soundsSO.GetDeliverySuccessAudioClips(), deliveryCounter.transform.position);
    }

    private void CutterCounter_OnAnyCut(object sender, System.EventArgs e) {
        CutterCounter cutterCounter = sender as CutterCounter;
        PlaySound(soundsSO.GetChopAudioClips(), cutterCounter.transform.position);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f) {
        AudioSource.PlayClipAtPoint(audioClip, position, volume * volumeMultiplier);
    }

    private void PlaySound(AudioClip[] audioClips, Vector3 position, float volumeMultiplier = 1f) {
        PlaySound(audioClips[UnityEngine.Random.Range(0, audioClips.Length)], position, volumeMultiplier);
    }

    private void PlayAudio(AudioSource audioSource, AudioClip audioClip, float volumeMultiplier = 1f) {
        audioSource.clip = audioClip;
        audioSource.volume = volume * volumeMultiplier;
        audioSource.Play();
    }

    private void PlayAudio(AudioSource audioSource, AudioClip[] audioClips, float volumeMultiplier = 1f) {
        PlayAudio(audioSource, audioClips[UnityEngine.Random.Range(0, audioClips.Length)], volumeMultiplier);
    }

    private void PauseAudio(AudioSource audioSource) {
        audioSource.Pause();
    }

    private void SetVolume(float normalizedVolume) {
        volume = normalizedVolume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
        PlayerPrefs.Save();
        OnVolumeChanged?.Invoke(this, new OnVolumeChangedEventArgs() {
            newVolume = volume
        });
    }

    public void PlayCountdownSound() {
        PlaySound(soundsSO.GetWarningAudioClips(), Vector3.zero);
    }

    public bool ChangeVolume(float normalizedVolume) {
        if (normalizedVolume < 0 || normalizedVolume > 1) {
            Debug.LogError("Normalized Sound Effects Volume Isn't Between 0 And 1!");
            return false;
        }
        SetVolume(normalizedVolume);
        return true;
    }

    public float GetVolume() {
        return volume;
    }

}
