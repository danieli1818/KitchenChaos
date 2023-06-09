using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TimerRecipeSO : InputOutputRecipeSO 
{
    [SerializeField] private float recipeTime;

    public float GetRecipeTime() {
        return recipeTime;
    }
}
