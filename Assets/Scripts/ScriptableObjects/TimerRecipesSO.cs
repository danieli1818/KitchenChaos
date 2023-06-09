using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TimerRecipesSO : ScriptableObject
{
    [SerializeField] private List<TimerRecipeSO> recipes;

    public TimerRecipeSO GetCounterRecipeSO(KitchenObjectSO input) {
        foreach (TimerRecipeSO recipe in recipes) {
            if (recipe.GetInput() == input) {
                return recipe;
            }
        }
        return null;
    }

    public KitchenObjectSO GetOutput(KitchenObjectSO input) {
        TimerRecipeSO recipe = GetCounterRecipeSO(input);
        if (recipe != null) {
            return recipe.GetOutput();
        }
        return null;
    }

    public bool HasRecipe(KitchenObjectSO input) {
        return GetCounterRecipeSO(input) != null;
    }
}
