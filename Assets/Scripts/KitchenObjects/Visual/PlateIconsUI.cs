using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private Transform iconTemplate;

    private void Start() {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e) {
        Transform newIngredientIcon = Instantiate(iconTemplate, transform);
        newIngredientIcon.GetComponent<PlateIconsSingleIconUI>().UpdateToKitchenObjectSO(e.ingredientKitchenObjectSO);
        newIngredientIcon.gameObject.SetActive(true);
    }
}
