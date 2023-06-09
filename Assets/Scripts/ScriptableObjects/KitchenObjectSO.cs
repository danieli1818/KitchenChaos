using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class KitchenObjectSO : ScriptableObject
{

    [SerializeField] private Transform prefab;
    [SerializeField] private Sprite icon;
    [SerializeField] private string objectName;

    public Transform GetPrefab() {
        return prefab;
    }

    public Sprite GetIcon() {
        return icon;
    }

    public string GetObjectName() {
        return objectName;
    }

}
