using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKitchenObjectHolder
{

    public bool SetHeldKitchenObject(KitchenObject kitchenObject);
    public bool HasHeldKitchenObject();
    public bool ClearHeldKitchenObject();
    public Transform GetHoldingPoint();

    public KitchenObject GetKitchenObject();

}
