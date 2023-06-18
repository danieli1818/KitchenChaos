using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
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
        recipesSpawnTimer = recipeSpawnTime;
        // ResetRecipesSpawnTimer();
    }

    private void Update() {
        if (!IsServer || !GameManager.Instance.IsGamePlaying()) {
            return;
        }
        if (waitingRecipes.Count < maxRecipes) {
            UpdateRecipesSpawnTimer();
            if (recipesSpawnTimer >= recipeSpawnTime) {
                int recipeIndex = GetRandomRecipeIndex();

                AddWaitingRecipeServerRpc(recipeIndex);

                ResetRecipesSpawnTimer();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddWaitingRecipeServerRpc(int waitingRecipeIndex) {
        AddWaitingRecipeClientRpc(waitingRecipeIndex);
    }

    [ClientRpc]
    private void AddWaitingRecipeClientRpc(int waitingRecipeIndex) {
        waitingRecipes.Add(spawningRecipes.GetRecipes()[waitingRecipeIndex]);
        OnOrderAdded?.Invoke(this, EventArgs.Empty);
    }

    public bool DeliverDish(PlateKitchenObject plate) {
        List<KitchenObjectSO> plateKitchenObjectSOs = plate.GetKitchenObjectSOsOnPlate();
        int recipeIndex = 0;
        foreach (DeliveryRecipeSO recipe in waitingRecipes) {
            List<KitchenObjectSO> recipeKitchenObjectSOs = recipe.GetRecipe();
            if (DoKitchenObjectsSOsMatch(plateKitchenObjectSOs, recipeKitchenObjectSOs)) {
                RemoveWaitingRecipeServerRpc(recipeIndex);
                return true;
            }
            recipeIndex++;
        }
        return false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemoveWaitingRecipeServerRpc(int waitingRecipeIndex) {
        RemoveWaitingRecipeClientRpc(waitingRecipeIndex);
    }

    [ClientRpc]
    private void RemoveWaitingRecipeClientRpc(int waitingRecipeIndex) {
        DeliveryRecipeSO recipe = waitingRecipes[waitingRecipeIndex];
        RemoveWaitingRecipe(recipe);
    }

    public List<DeliveryRecipeSO> GetWaitingRecipesSOs() {
        return waitingRecipes;
    }

    private DeliveryRecipeSO GetRandomRecipe() {
        List<DeliveryRecipeSO> recipes = spawningRecipes.GetRecipes();
        return recipes[UnityEngine.Random.Range(0, recipes.Count)];
    }

    private int GetRandomRecipeIndex() {
        List<DeliveryRecipeSO> recipes = spawningRecipes.GetRecipes();
        return UnityEngine.Random.Range(0, recipes.Count);
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
