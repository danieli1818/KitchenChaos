using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class DeliveryRecipeSO : ScriptableObject
{
    [SerializeField] private List<KitchenObjectSO> recipe;
    [SerializeField] private string recipeName;

    public List<KitchenObjectSO> GetRecipe() {
        return recipe;
    }

    public string GetRecipeName() {
        return recipeName;
    }

}
