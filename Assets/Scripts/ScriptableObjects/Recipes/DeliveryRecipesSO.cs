using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class DeliveryRecipesSO : ScriptableObject
{

    [SerializeField] private List<DeliveryRecipeSO> recipes;

    public List<DeliveryRecipeSO> GetRecipes() {
        return recipes;
    }

}
