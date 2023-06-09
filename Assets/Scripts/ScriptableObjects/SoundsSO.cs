using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SoundsSO : ScriptableObject
{

    [SerializeField] private AudioClip[] chopAudioClips;
    [SerializeField] private AudioClip[] deliverySuccessAudioClips;
    [SerializeField] private AudioClip[] deliveryFailAudioClips;
    [SerializeField] private AudioClip[] footstepsAudioClips;
    [SerializeField] private AudioClip[] objectDropAudioClips;
    [SerializeField] private AudioClip[] objectPickupAudioClips;
    [SerializeField] private AudioClip[] panSizzleAudioClips;
    [SerializeField] private AudioClip[] trashAudioClips;
    [SerializeField] private AudioClip[] warningAudioClips;

    public AudioClip[] GetChopAudioClips() {
        return chopAudioClips;
    }

    public AudioClip[] GetDeliverySuccessAudioClips() {
        return deliverySuccessAudioClips;
    }

    public AudioClip[] GetDeliveryFailAudioClips() {
        return deliveryFailAudioClips;
    }

    public AudioClip[] GetFootstepsAudioClips() {
        return footstepsAudioClips;
    }

    public AudioClip[] GetObjectDropAudioClips() {
        return objectDropAudioClips;
    }

    public AudioClip[] GetObjectPickupAudioClips() {
        return objectPickupAudioClips;
    }

    public AudioClip[] GetPanSizzleAudioClips() {
        return panSizzleAudioClips;
    }

    public AudioClip[] GetTrashAudioClips() {
        return trashAudioClips;
    }

    public AudioClip[] GetWarningAudioClips() {
        return warningAudioClips;
    }

}
