using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{

    [SerializeField] private Transform recipesContainer;
    [SerializeField] private Transform recipeTemplate;

    private void Awake() {
        recipeTemplate.gameObject.SetActive(false);
    }

    private void Start() {
        DeliveryManager.Instance.OnOrderAdded += DeliveryManager_OnOrderAdded;
        DeliveryManager.Instance.OnOrderCompleted += DeliveryManager_OnOrderCompleted;
    }

    private void DeliveryManager_OnOrderCompleted(object sender, System.EventArgs e) {
        UpdateVisual();
    }

    private void DeliveryManager_OnOrderAdded(object sender, System.EventArgs e) {
        UpdateVisual();
    }

    private void UpdateVisual() {
        foreach (Transform recipeTransform in recipesContainer) {
            if (recipeTransform == recipeTemplate) {
                continue;
            }
            Destroy(recipeTransform.gameObject);
        }
        foreach (DeliveryRecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipesSOs()) {
            Transform recipeTransform = Instantiate(recipeTemplate, recipesContainer);
            recipeTransform.GetComponent<DeliveryManagerSingleRecipeUI>().SetRecipeSO(recipeSO);
            recipeTransform.gameObject.SetActive(true);
        }
    }

}
