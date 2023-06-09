using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{

    public static DeliveryManager Instance { get; private set; }

    public event EventHandler OnOrderAdded;
    public event EventHandler OnOrderCompleted;

    [SerializeField] private DeliveryRecipesSO spawningRecipes;
    [SerializeField] private float recipeSpawnTime;
    [SerializeField] private int maxRecipes;

    private List<DeliveryRecipeSO> waitingRecipes;
    private float recipesSpawnTimer;
    private int successfulDeliveryCounter;

    private void Awake() {
        Instance = this;
        waitingRecipes = new List<DeliveryRecipeSO>();
        ResetRecipesSpawnTimer();
    }

    private void Update() {
        if (waitingRecipes.Count < maxRecipes) {
            UpdateRecipesSpawnTimer();
            if (recipesSpawnTimer >= recipeSpawnTime) {
                waitingRecipes.Add(GetRandomRecipe());
                OnOrderAdded?.Invoke(this, EventArgs.Empty);
                ResetRecipesSpawnTimer();
            }
        }
    }

    public bool DeliverDish(PlateKitchenObject plate) {
        List<KitchenObjectSO> plateKitchenObjectSOs = plate.GetKitchenObjectSOsOnPlate();
        foreach (DeliveryRecipeSO recipe in waitingRecipes) {
            List<KitchenObjectSO> recipeKitchenObjectSOs = recipe.GetRecipe();
            if (DoKitchenObjectsSOsMatch(plateKitchenObjectSOs, recipeKitchenObjectSOs)) {
                RemoveWaitingRecipe(recipe);
                return true;
            }
        }
        return false;
    }

    public List<DeliveryRecipeSO> GetWaitingRecipesSOs() {
        return waitingRecipes;
    }

    private DeliveryRecipeSO GetRandomRecipe() {
        List<DeliveryRecipeSO> recipes = spawningRecipes.GetRecipes();
        return recipes[UnityEngine.Random.Range(0, recipes.Count)];
    }

    private void UpdateRecipesSpawnTimer() {
        recipesSpawnTimer += Time.deltaTime;
    }

    private void ResetRecipesSpawnTimer() {
        recipesSpawnTimer = 0f;
    }

    private void RemoveWaitingRecipe(DeliveryRecipeSO recipe) {
        waitingRecipes.Remove(recipe);
        successfulDeliveryCounter++;
        OnOrderCompleted?.Invoke(this, EventArgs.Empty);
    }

    private bool DoKitchenObjectsSOsMatch(List<KitchenObjectSO> kitchenObjectSOs1, List<KitchenObjectSO> kitchenObjectSOs2) {
        if (kitchenObjectSOs1.Count != kitchenObjectSOs2.Count) {
            return false;
        }
        foreach (KitchenObjectSO kitchenObjectSO in kitchenObjectSOs1) {
            if (!kitchenObjectSOs2.Contains(kitchenObjectSO)) { // This works since there are no duplicates
                return false;
            }
        }
        return true;
    }

    public int GetNumberOfSuccessfulRecipesDeliveries() {
        return successfulDeliveryCounter;
    }

}
