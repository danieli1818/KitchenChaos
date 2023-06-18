using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private FadeEffect plateKitchenObjectFadeEffect;
    [SerializeField] private Transform iconTemplate;

    private CanvasGroup canvasGroup;

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start() {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
        plateKitchenObjectFadeEffect.OnFadeEffectProgressChanged += PlateKitchenObject_OnFadeEffectProgressChanged;
    }

    private void PlateKitchenObject_OnFadeEffectProgressChanged(object sender, FadeEffect.OnFadeEffectProgressChangedEventArgs e) {
        canvasGroup.alpha = 1 - e.progress;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e) {
        Transform newIngredientIcon = Instantiate(iconTemplate, transform);
        newIngredientIcon.GetComponent<PlateIconsSingleIconUI>().UpdateToKitchenObjectSO(e.ingredientKitchenObjectSO);
        newIngredientIcon.gameObject.SetActive(true);
    }
}
