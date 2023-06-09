using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounterKitchenObjectFadeVisual : MonoBehaviour
{

    private const string SHADER_PROGRESS = "_Progress";

    [SerializeField] private Transform moveToTransform;
    [SerializeField] private float fadeTime;
    private float fadeTimer;

    private void Awake() {
        fadeTimer = 0;
    }

    private void Update() {
        fadeTimer += Time.deltaTime;
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
            foreach (Material material in renderer.materials) {
                material.SetFloat(SHADER_PROGRESS, fadeTimer / fadeTime);
            }
        }
        transform.position += ((moveToTransform.position - transform.position) / (fadeTime - fadeTimer)) * Time.deltaTime;
        if (fadeTimer >= fadeTime) {
            gameObject.GetComponent<PlateKitchenObject>().DestroySelf();
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
