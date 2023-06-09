using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{

    public override void Interact(PlayerController player) {
        if (HasHeldKitchenObject()) {
            if (!player.HasHeldKitchenObject()) {
                GetKitchenObject().SetKitchenObjectHolder(player);
            } else {
                if (GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) {
                        player.GetKitchenObject().DestroySelf();
                    }
                } else {
                    if (player.GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
                        if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                            GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
        } else {
            if (player.HasHeldKitchenObject()) {
                player.GetKitchenObject().SetKitchenObjectHolder(this);
            }
        }
    }

}
