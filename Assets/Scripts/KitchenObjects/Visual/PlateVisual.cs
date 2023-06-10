using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateVisual : MonoBehaviour
{

    private const string SHADER_PROGRESS = "_Progress";

    [Serializable]
    public struct KitchenObjectSOGameObjectPair {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectSOGameObjectPair> kitchenObjectSOGameObjectPairs;

    private void Start() {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
        plateKitchenObject.OnFadeEffectProgressChanged += PlateKitchenObject_OnFadeEffectProgressChanged;
    }

    private void PlateKitchenObject_OnFadeEffectProgressChanged(object sender, PlateKitchenObject.OnFadeEffectProgressChangedEventArgs e) {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
            foreach (Material material in renderer.materials) {
                material.SetFloat(SHADER_PROGRESS, e.progress);
            }
        }
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e) {
        GameObject gameObject = GetGameObjectForKitchenObjectSO(e.ingredientKitchenObjectSO);
        if (gameObject != null) {
            gameObject.SetActive(true);
        }
    }

    private GameObject GetGameObjectForKitchenObjectSO(KitchenObjectSO kitchenObjectSO) {
        foreach (KitchenObjectSOGameObjectPair kitchenObjectSOGameObjectPair in kitchenObjectSOGameObjectPairs) {
            if (kitchenObjectSOGameObjectPair.kitchenObjectSO == kitchenObjectSO) {
                return kitchenObjectSOGameObjectPair.gameObject;
            }
        }
        return null;
    }

}
