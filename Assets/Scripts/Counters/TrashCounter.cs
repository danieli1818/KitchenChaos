using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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

            InteractLogicServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void InteractLogicServerRpc() {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    public void InteractLogicClientRpc() {
        OnAnyKitchenObjectTrashed?.Invoke(this, EventArgs.Empty);
    }

}
