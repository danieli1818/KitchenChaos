using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStoveCounterRecipesSO
{
    public IStoveCounterRecipeSO GetCounterRecipeSO(KitchenObjectSO input);

    public KitchenObjectSO GetOutput(KitchenObjectSO input);

    public bool HasRecipe(KitchenObjectSO input);
}
