using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ProgressRecipeSO : InputOutputRecipeSO
{
    [SerializeField] private int maxProgress;

    public int GetMaxProgress() {
        return maxProgress;
    }
}
