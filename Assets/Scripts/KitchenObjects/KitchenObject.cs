using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectHolder kitchenObjectHolder;
    private FollowTransform followTransform;

    protected virtual void Awake() {
        followTransform = GetComponent<FollowTransform>();
    }

    public KitchenObjectSO GetKitchenObjectSO() {
        return kitchenObjectSO;
    }

    public void SetKitchenObjectHolder(IKitchenObjectHolder kitchenObjectHolder) {
        Debug.Log("Kitchen Object Holder: " + kitchenObjectHolder + " and its Network Object: " + kitchenObjectHolder.GetNetworkObject());
        SetKitchenObjectHolderServerRpc(kitchenObjectHolder.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjectHolderServerRpc(NetworkObjectReference kitchenObjectHolderNetworkObjectReference) {
        SetKitchenObjectHolderClientRpc(kitchenObjectHolderNetworkObjectReference);
    }

    [ClientRpc]
    private void SetKitchenObjectHolderClientRpc(NetworkObjectReference kitchenObjectHolderNetworkObjectReference) {
        if (!kitchenObjectHolderNetworkObjectReference.TryGet(out NetworkObject kitchenObjectHolderNetworkObject)) {
            Debug.LogError("Couldn't get kitchen object holder network object from reference!");
            return;
        }
        IKitchenObjectHolder kitchenObjectHolder = kitchenObjectHolderNetworkObject.GetComponent<IKitchenObjectHolder>();
        this.kitchenObjectHolder?.ClearHeldKitchenObject();

        if (kitchenObjectHolder.HasHeldKitchenObject()) {
            Debug.LogError("Trying to set kitchen object to an holder which already got a kitchen object!");
            return;
        }

        this.kitchenObjectHolder = kitchenObjectHolder;
        this.kitchenObjectHolder.SetHeldKitchenObject(this);

        followTransform.SetFollowTransform(this.kitchenObjectHolder.GetHoldingPoint());

        return;
    }

    public void DestroySelf() {
        KitchenObjectMultiplayerGameObject.Instance.DestroyKitchenObject(this);
    }

    public void ClearKitchenObjectFromHolder() {
        kitchenObjectHolder?.ClearHeldKitchenObject();
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {
        if (this is PlateKitchenObject) {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        plateKitchenObject = null;
        return false;
    }

    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectHolder holder) {
        KitchenObjectMultiplayerGameObject.Instance.SpawnKitchenObject(kitchenObjectSO, holder);
    }

}
