using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateVisual : MonoBehaviour
{
    [Serializable]
    public struct KitchenObjectSOGameObjectPair {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectSOGameObjectPair> kitchenObjectSOGameObjectPairs;

    private void Start() {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
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
