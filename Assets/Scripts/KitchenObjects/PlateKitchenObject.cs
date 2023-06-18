using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject, IHasFadeEffect
{

    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs {
        public KitchenObjectSO ingredientKitchenObjectSO;
    }

    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOs;
    private List<KitchenObjectSO> kitchenObjectSOs;

    protected override void Awake() {
        base.Awake();
        kitchenObjectSOs = new List<KitchenObjectSO>();
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) {
        if (!validKitchenObjectSOs.Contains(kitchenObjectSO) ||  kitchenObjectSOs.Contains(kitchenObjectSO)) {
            return false;
        }
        AddIngredientServerRpc(KitchenObjectMultiplayerGameObject.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));
        return true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex) {
        AddIngredientClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex) {
        KitchenObjectSO kitchenObjectSO = KitchenObjectMultiplayerGameObject.Instance.GetKitchenObjectSO(kitchenObjectSOIndex);
        kitchenObjectSOs.Add(kitchenObjectSO);
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs() {
            ingredientKitchenObjectSO = kitchenObjectSO
        });
    }

    public List<KitchenObjectSO> GetKitchenObjectSOsOnPlate() {
        return kitchenObjectSOs;
    }

}
