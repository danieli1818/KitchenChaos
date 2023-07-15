using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenObjectMultiplayerGameObject : NetworkBehaviour
{
    
    public static KitchenObjectMultiplayerGameObject Instance { get; private set; }

    [SerializeField] private KitchenObjectsListSO kitchenObjectsListSO;

    private void Awake() {
        if (Instance != null) {
            return;
        }
        Instance = this;
    }

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectHolder holder) {
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), holder.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectHolderNetworkObjectReference) {
        Transform kitchenObjectTransform = Instantiate(GetKitchenObjectSO(kitchenObjectSOIndex).GetPrefab());

        if (!kitchenObjectHolderNetworkObjectReference.TryGet(out NetworkObject kitchenObjectHolderNetworkObject)) {
            Debug.LogError("Couldn't get Kitchen Object Holder's Network Object from Reference!");
        }
        IKitchenObjectHolder kitchenObjectHolder = kitchenObjectHolderNetworkObject.GetComponent<IKitchenObjectHolder>();

        if (kitchenObjectHolder == null || kitchenObjectHolder.HasHeldKitchenObject()) {
            return; // Spawn Lag
        }

        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        Debug.Log("In Spawn, Kitchen Object Holder: " + kitchenObjectHolder + " and its Network Object: " + kitchenObjectHolder.GetNetworkObject());
        kitchenObject.SetKitchenObjectHolder(kitchenObjectHolder);
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject) {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) {
        ClearKitchenObjectFromHolderClientRpc(kitchenObjectNetworkObjectReference);
        if (!kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject)) {
            Debug.Log("Couldn't get the Kitchen Object's Network Object from its reference!");
        }
        if (kitchenObjectNetworkObject == null) {
            return; // Kitchen Object has already been destroyed
        }
        kitchenObjectNetworkObject.Despawn();
    }

    [ClientRpc]
    private void ClearKitchenObjectFromHolderClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) {
        if (!kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject)) {
            Debug.Log("Couldn't get the Kitchen Object's Network Object from its reference!");
        }
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        kitchenObject.ClearKitchenObjectFromHolder();
    }

    public KitchenObjectSO GetKitchenObjectSO(int index) {
        return kitchenObjectsListSO.GetKitchenObjectSO(index);
    }

    public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO) {
        return kitchenObjectsListSO.GetKitchenObjectSOIndex(kitchenObjectSO);
    }

}
