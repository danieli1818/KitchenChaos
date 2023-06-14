using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class KitchenObjectsListSO : ScriptableObject
{

    [SerializeField] private List<KitchenObjectSO> kitchenObjectsList;

    public KitchenObjectSO GetKitchenObjectSO(int index) {
        return kitchenObjectsList[index];
    }

    public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO) {
        return kitchenObjectsList.IndexOf(kitchenObjectSO);
    }

}
