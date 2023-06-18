using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{

    public static event EventHandler OnAnySuccessfulDelivery;
    public static event EventHandler OnAnyIncorrectDelivery;

    public event EventHandler OnSuccessfulDelivery;
    public event EventHandler OnIncorrectDelivery;

    [SerializeField] private Transform moveToTransform;
    [SerializeField] private float fadeTime = 10;

    new public static void ResetStaticData() {
        OnAnySuccessfulDelivery = null;
        OnAnyIncorrectDelivery = null;
    }

    public override void Interact(PlayerController player) {
        if (player.HasHeldKitchenObject()) {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                if (DeliveryManager.Instance.DeliverDish(plateKitchenObject)) {
                    NotifySuccessfulDeliveryServerRpc();
                } else {
                    NotifyIncorrectDeliveryServerRpc();
                }
                plateKitchenObject.SetKitchenObjectHolder(this);
                FadePlateKitchenObjectServerRpc(plateKitchenObject.NetworkObject);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void FadePlateKitchenObjectServerRpc(NetworkObjectReference plateKitchenObjectNetworkObjectReference) {
        FadePlateKitchenObjectClientRpc(plateKitchenObjectNetworkObjectReference);
    }

    [ClientRpc]
    private void FadePlateKitchenObjectClientRpc(NetworkObjectReference plateKitchenObjectNetworkObjectReference) {
        if (!plateKitchenObjectNetworkObjectReference.TryGet(out NetworkObject plateKitchenObjectNetworkObject)) {
            Debug.Log("Couldn't get network object of plate kitchen object from reference!");
        }
        PlateKitchenObject plateKitchenObject = plateKitchenObjectNetworkObject.GetComponent<PlateKitchenObject>();
        plateKitchenObject.ClearFollowTransform();
        FadeEffect fadeEffect = plateKitchenObjectNetworkObject.GetComponent<FadeEffect>();
        fadeEffect.SetFadeTime(fadeTime);
        fadeEffect.SetMoveToTransform(moveToTransform);
        fadeEffect.StartFadeEffect();
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifySuccessfulDeliveryServerRpc() {
        NotifySuccessfulDeliveryClientRpc();
    }

    [ClientRpc]
    private void NotifySuccessfulDeliveryClientRpc() {
        OnSuccessfulDelivery?.Invoke(this, EventArgs.Empty);
        OnAnySuccessfulDelivery?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyIncorrectDeliveryServerRpc() {
        NotifyIncorrectDeliveryClientRpc();
    }

    [ClientRpc]
    private void NotifyIncorrectDeliveryClientRpc() {
        OnIncorrectDelivery?.Invoke(this, EventArgs.Empty);
        OnAnyIncorrectDelivery?.Invoke(this, EventArgs.Empty);
    }

    public override bool SetHeldKitchenObject(KitchenObject kitchenObject) {
        return true;
    }

}
