using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FadeEffect : NetworkBehaviour
{

    public event EventHandler<OnFadeEffectProgressChangedEventArgs> OnFadeEffectProgressChanged;
    public class OnFadeEffectProgressChangedEventArgs : EventArgs {
        public float progress;
    }

    [SerializeField] private Transform moveToTransform;
    [SerializeField] private float fadeTime;
    private float fadeTimer;
    private bool shouldFade;
    private float fadeEffectProgress;

    private IHasFadeEffect fadeEffectObject;

    private void Awake() {
        fadeTimer = 0;
        shouldFade = false;
    }

    private void Start() {
        fadeEffectObject = GetComponent<IHasFadeEffect>();
    }

    private void Update() {
        if (shouldFade) {
            fadeTimer += Time.deltaTime;
            if (fadeTimer > fadeTime) {
                fadeTimer = fadeTime;
            }
            UpdateFadeEffectProgress(fadeTimer / fadeTime);
        }
    }

    private bool UpdateFadeEffectProgress(float fadeEffectProgress) {
        if (this.fadeEffectProgress == fadeEffectProgress) {
            return false;
        }
        OnFadeEffectProgressChanged?.Invoke(this, new OnFadeEffectProgressChangedEventArgs() {
            progress = fadeEffectProgress
        });
        float distance = fadeTime != fadeTimer ? fadeTime - fadeTimer : 1;
        transform.position += ((moveToTransform.position - transform.position) / distance) * Time.deltaTime;
        if (IsServer) {
            if (fadeTimer >= fadeTime) {
                fadeEffectObject.DestroySelf();
            }
        }
        return true;
    }

    public bool SetFadeTime(float fadeTime) {
        if (fadeTime < 0) {
            return false;
        }
        this.fadeTime = fadeTime;
        return true;
    }

    public void SetMoveToTransform(Transform moveToTransform) {
        this.moveToTransform = moveToTransform;
    }

    public bool StartFadeEffect() {
        if (shouldFade) {
            return false;
        }
        shouldFade = true;
        return true;
    }

    public bool StopFadeEffect() {
        if (!shouldFade) {
            return false;
        }
        shouldFade = false;
        return true;
    }

}
