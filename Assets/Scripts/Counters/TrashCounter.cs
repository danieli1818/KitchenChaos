using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCounter : BaseCounter
{

    public static event EventHandler OnAnyKitchenObjectTrashed;

    new public static void ResetStaticData() {
        OnAnyKitchenObjectTrashed = null;
    }

    public override void Interact(PlayerController player) {
        if (player.HasHeldKitchenObject()) {
            player.GetKitchenObject().DestroySelf();
            OnAnyKitchenObjectTrashed?.Invoke(this, EventArgs.Empty);
        }
    }
}
