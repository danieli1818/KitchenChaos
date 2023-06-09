using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{

    public static event EventHandler OnAnySuccessfulDelivery;
    public static event EventHandler OnAnyIncorrectDelivery;

    public event EventHandler OnSuccessfulDelivery;
    public event EventHandler OnIncorrectDelivery;

    new public static void ResetStaticData() {
        OnAnySuccessfulDelivery = null;
        OnAnyIncorrectDelivery = null;
    }

    public override void Interact(PlayerController player) {
        if (player.HasHeldKitchenObject()) {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                if (DeliveryManager.Instance.DeliverDish(plateKitchenObject)) {
                    OnSuccessfulDelivery?.Invoke(this, EventArgs.Empty);
                    OnAnySuccessfulDelivery?.Invoke(this, EventArgs.Empty);
                } else {
                    OnIncorrectDelivery?.Invoke(this, EventArgs.Empty);
                    OnAnyIncorrectDelivery?.Invoke(this, EventArgs.Empty);
                }
                plateKitchenObject.DestroySelf();
            }
        }
    }

}
