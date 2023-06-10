using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{

    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public event EventHandler<OnFadeEffectProgressChangedEventArgs> OnFadeEffectProgressChanged;
    public class OnIngredientAddedEventArgs : EventArgs {
        public KitchenObjectSO ingredientKitchenObjectSO;
    }
    public class OnFadeEffectProgressChangedEventArgs : EventArgs {
        public float progress;
    }

    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOs;
    private List<KitchenObjectSO> kitchenObjectSOs;

    private void Awake() {
        kitchenObjectSOs = new List<KitchenObjectSO>();
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) {
        if (!validKitchenObjectSOs.Contains(kitchenObjectSO) ||  kitchenObjectSOs.Contains(kitchenObjectSO)) {
            return false;
        }
        kitchenObjectSOs.Add(kitchenObjectSO);
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs() {
            ingredientKitchenObjectSO = kitchenObjectSO
        });
        return true;
    }

    public List<KitchenObjectSO> GetKitchenObjectSOsOnPlate() {
        return kitchenObjectSOs;
    }

    public bool SetFadeProgress(float progress) {
        if (progress < 0 || progress > 1) {
            return false;
        }
        OnFadeEffectProgressChanged?.Invoke(this, new OnFadeEffectProgressChangedEventArgs() {
            progress = progress
        });
        return true;
    }

}
