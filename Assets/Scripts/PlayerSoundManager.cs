using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{

    public static event EventHandler<OnAnyFootstepsPlaySoundEventArgs> OnAnyFootstepsPlaySound;
    public class OnAnyFootstepsPlaySoundEventArgs : EventArgs {
        public PlayerController playerController;
    }

    [SerializeField] private PlayerController playerController;
    [SerializeField] private float footstepsSoundPlayInterval;

    private float footstepsTimer;

    public static void ResetStaticData() {
        OnAnyFootstepsPlaySound = null;
    }

    private void Awake() {
        ResetFootstepsTimer();
    }

    private void Update() {
        if (footstepsTimer < footstepsSoundPlayInterval) {
            UpdateFootstepsTimer();
        }
        if (playerController.IsWalking() && footstepsTimer >= footstepsSoundPlayInterval) {
            OnAnyFootstepsPlaySound?.Invoke(this, new OnAnyFootstepsPlaySoundEventArgs() {
                playerController = playerController
            });
            ResetFootstepsTimer();
        }
    }

    private void ResetFootstepsTimer() {
        footstepsTimer = 0f;
    }

    private void UpdateFootstepsTimer() {
        footstepsTimer += Time.deltaTime;
    }

}
