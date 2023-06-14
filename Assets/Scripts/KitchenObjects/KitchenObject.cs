using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectHolder kitchenObjectHolder;

    public KitchenObjectSO GetKitchenObjectSO() {
        return kitchenObjectSO;
    }

    public bool SetKitchenObjectHolder(IKitchenObjectHolder kitchenObjectHolder) {
        this.kitchenObjectHolder?.ClearHeldKitchenObject();

        if (kitchenObjectHolder.HasHeldKitchenObject()) {
            Debug.LogError("Trying to set kitchen object to an holder which already got a kitchen object!");
            return false;
        }

        this.kitchenObjectHolder = kitchenObjectHolder;
        this.kitchenObjectHolder.SetHeldKitchenObject(this);

        /* transform.SetParent(this.kitchenObjectHolder.GetHoldingPoint());
        transform.localPosition = Vector3.zero; */

        return true;
    }

    public void DestroySelf() {
        kitchenObjectHolder?.ClearHeldKitchenObject();
        Destroy(gameObject);
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
