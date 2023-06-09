using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleRecipeUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Transform iconsContainer;
    [SerializeField] private Transform iconTemplate;

    private void Awake() {
        iconTemplate.gameObject.SetActive(false);
    }

    public void SetRecipeSO(DeliveryRecipeSO recipeSO) {
        recipeNameText.text = recipeSO.name;
        UpdateIcons(recipeSO);
    }

    private void UpdateIcons(DeliveryRecipeSO recipeSO) {
        foreach (Transform iconTransform in iconsContainer) {
            if (iconTransform == iconTemplate) {
                continue;
            }
            Destroy(iconTransform.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.GetRecipe()) {
            CreateIcon(kitchenObjectSO);
        }
    }

    private void CreateIcon(KitchenObjectSO kitchenObjectSO) {
        Transform iconTransform = Instantiate(iconTemplate, iconsContainer);
        iconTransform.GetComponent<Image>().sprite = kitchenObjectSO.GetIcon();
        iconTransform.gameObject.SetActive(true);
    }

}
