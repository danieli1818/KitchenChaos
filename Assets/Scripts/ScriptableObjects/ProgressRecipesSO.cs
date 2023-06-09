using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ProgressRecipesSO : ScriptableObject
{
    [SerializeField] private List<ProgressRecipeSO> recipes;

    public ProgressRecipeSO GetCounterRecipeSO(KitchenObjectSO input) {
        foreach (ProgressRecipeSO recipe in recipes) {
            if (recipe.GetInput() == input) {
                return recipe;
            }
        }
        return null;
    }

    public KitchenObjectSO GetOutput(KitchenObjectSO input) {
        ProgressRecipeSO recipe = GetCounterRecipeSO(input);
        if (recipe != null) {
            return recipe.GetOutput();
        }
        return null;
    }

    public bool HasRecipe(KitchenObjectSO input) {
        return GetCounterRecipeSO(input) != null;
    }
}
