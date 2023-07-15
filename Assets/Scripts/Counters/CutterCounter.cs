using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CutterCounter : BaseCounter, IHasProgress
{

    public static event EventHandler OnAnyCut;

    public event EventHandler<IHasProgress.OnProgressUpdateEventArgs> OnProgressUpdate;
    public event EventHandler OnCut;

    [SerializeField] private ProgressRecipesSO recipes;

    private int progress;

    new public static void ResetStaticData() {
        OnAnyCut = null;
    }

    public override void Interact(PlayerController player) {
        if (HasHeldKitchenObject()) {
            if (!player.HasHeldKitchenObject()) {
                GetKitchenObject().SetKitchenObjectHolder(player);
                ResetProgressServerRpc();
            } else {
                if (GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) {
                        player.GetKitchenObject().DestroySelf();
                    }
                } else {
                    if (player.GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
                        if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                            GetKitchenObject().DestroySelf();
                            ResetProgressServerRpc();
                        }
                    }
                }
            }
        } else {
            if (player.HasHeldKitchenObject()) {
                player.GetKitchenObject().SetKitchenObjectHolder(this);
                ResetProgressServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResetProgressServerRpc() {
        ResetProgressClientRpc();
    }

    [ClientRpc]
    private void ResetProgressClientRpc() {
        progress = 0;
        OnProgressUpdate?.Invoke(this, new IHasProgress.OnProgressUpdateEventArgs {
            progress = progress
        });
    }

    public override void InteractAlternate(PlayerController player) {
        if (HasHeldKitchenObject() && recipes.HasRecipe(GetKitchenObject().GetKitchenObjectSO())) {
            InteractAlternateLogicServerRpc();
        }
    }

    private void CallOnProgressUpdateEvent(int maxProgress) {
        OnProgressUpdate?.Invoke(this, new IHasProgress.OnProgressUpdateEventArgs {
            progress = ((float)progress) / maxProgress
        });
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractAlternateLogicServerRpc() {
        if (HasHeldKitchenObject() && recipes.HasRecipe(GetKitchenObject().GetKitchenObjectSO())) { // Else the player already cut and has lags
            CutObjectClientRpc();
            CheckCutDone();
        }
    }

    [ClientRpc]
    private void CutObjectClientRpc() {
        ProgressRecipeSO recipe = recipes.GetCounterRecipeSO(GetKitchenObject().GetKitchenObjectSO());
        progress += 1;
        OnCut?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);
        CallOnProgressUpdateEvent(recipe.GetMaxProgress());
    }

    private void CheckCutDone() {
        ProgressRecipeSO recipe = recipes.GetCounterRecipeSO(GetKitchenObject().GetKitchenObjectSO());
        if (progress >= recipe.GetMaxProgress()) {
            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(recipe.GetOutput(), this);
        }
    }

}
