using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStoveCounterRecipeSO
{
    public KitchenObjectSO GetInput();

    public KitchenObjectSO GetOutput();

    public float GetRecipeTime();
}
