using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";

    public static MusicManager Instance { get; private set; }

    public event EventHandler<OnVolumeChangedEventArgs> OnVolumeChanged;
    public class OnVolumeChangedEventArgs {
        public float newVolume;
    }

    private AudioSource musicAudioSource;
    private float volume;

    private void Awake() {
        Instance = this;

        musicAudioSource = GetComponent<AudioSource>();
        SetVolume(PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, 1f));
    }

    private void SetVolume(float normalizedVolume) {
        volume = normalizedVolume;
        musicAudioSource.volume = volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
        OnVolumeChanged?.Invoke(this, new OnVolumeChangedEventArgs() {
            newVolume = volume
        });
    }

    public bool ChangeVolume(float normalizedVolume) {
        if (normalizedVolume < 0 || normalizedVolume > 1) {
            Debug.LogError("Normalized Music Volume Isn't Between 0 And 1!");
            return false;
        }
        SetVolume(normalizedVolume);
        return true;
    }

    public float GetVolume() {
        return volume;
    }

}
