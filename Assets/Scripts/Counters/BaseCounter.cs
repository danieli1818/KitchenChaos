using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCounter : MonoBehaviour, IInteractable, IKitchenObjectHolder
{

    public static event EventHandler OnAnyDropKitchenObject;

    public event EventHandler OnSelect;
    public event EventHandler OnUnSelect;

    [SerializeField] private Transform counterTopPoint;

    private KitchenObject kitchenObject;

    public abstract void Interact(PlayerController player);

    public virtual void InteractAlternate(PlayerController player) {}

    public static void ResetStaticData() {
        OnAnyDropKitchenObject = null;
    }

    public void Select() {
        OnSelect?.Invoke(this, EventArgs.Empty);
    }

    public void UnSelect() {
        OnUnSelect?.Invoke(this, EventArgs.Empty);
    }

    public bool SetHeldKitchenObject(KitchenObject kitchenObject) {
        if (kitchenObject != null && HasHeldKitchenObject()) {
            Debug.LogError("Trying to set kitchen object while already having one!");
            return false;
        }
        this.kitchenObject = kitchenObject;
        if (this.kitchenObject != null) {
            OnAnyDropKitchenObject?.Invoke(this, EventArgs.Empty);
        }
        return true;
    }

    public bool HasHeldKitchenObject() {
        return kitchenObject != null;
    }

    public bool ClearHeldKitchenObject() {
        if (HasHeldKitchenObject()) {
            SetHeldKitchenObject(null);
            return true;
        }
        return false;
    }

    public Transform GetHoldingPoint() {
        return counterTopPoint;
    }

    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }


}
