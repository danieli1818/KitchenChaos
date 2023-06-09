using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputOutputRecipeSO : ScriptableObject
{
    [SerializeField] private KitchenObjectSO input;
    [SerializeField] private KitchenObjectSO output;

    public KitchenObjectSO GetInput() {
        return input;
    }

    public KitchenObjectSO GetOutput() {
        return output;
    }
}
