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
        SpawnKitchenObjectServerRpc(kitchenObjectsListSO.GetKitchenObjectSOIndex(kitchenObjectSO), holder.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectHolderNetworkObjectReference) {
        Transform kitchenObjectTransform = Instantiate(kitchenObjectsListSO.GetKitchenObjectSO(kitchenObjectSOIndex).GetPrefab());

        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        if (!kitchenObjectHolderNetworkObjectReference.TryGet(out NetworkObject kitchenObjectHolderNetworkObject)) {
            Debug.LogError("Couldn't get Kitchen Object Holder's Network Object from Reference!");
        }
        kitchenObject.SetKitchenObjectHolder(kitchenObjectHolderNetworkObject.GetComponent<IKitchenObjectHolder>());
    }

}
