using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounterKitchenObjectFadeVisual : MonoBehaviour
{

    [SerializeField] private Transform moveToTransform;
    [SerializeField] private float fadeTime;
    private float fadeTimer;

    private PlateKitchenObject plateKitchenObject;

    private void Awake() {
        fadeTimer = 0;
        plateKitchenObject = GetComponent<PlateKitchenObject>();
    }

    private void Update() {
        fadeTimer += Time.deltaTime;
        plateKitchenObject.SetFadeProgress(fadeTimer / fadeTime);
        transform.position += ((moveToTransform.position - transform.position) / (fadeTime - fadeTimer)) * Time.deltaTime;
        if (fadeTimer >= fadeTime) {
            plateKitchenObject.DestroySelf();
        }
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

}
