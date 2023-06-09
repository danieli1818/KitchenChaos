using System;
using System.Collections;
using System.Collections.Generic;
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
                progress = 0;
                OnProgressUpdate?.Invoke(this, new IHasProgress.OnProgressUpdateEventArgs {
                    progress = progress
                });
            } else {
                if (GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) {
                        player.GetKitchenObject().DestroySelf();
                    }
                } else {
                    if (player.GetKitchenObject().TryGetPlate(out plateKitchenObject)) {
                        if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                            GetKitchenObject().DestroySelf();
                            progress = 0;
                            OnProgressUpdate?.Invoke(this, new IHasProgress.OnProgressUpdateEventArgs {
                                progress = progress
                            });
                        }
                    }
                }
            }
        } else {
            if (player.HasHeldKitchenObject()) {
                player.GetKitchenObject().SetKitchenObjectHolder(this);
                progress = 0;
                OnProgressUpdate?.Invoke(this, new IHasProgress.OnProgressUpdateEventArgs {
                    progress = progress
                });
            }
        }
    }

    public override void InteractAlternate(PlayerController player) {
        if (HasHeldKitchenObject()) {
            ProgressRecipeSO recipe = recipes.GetCounterRecipeSO(GetKitchenObject().GetKitchenObjectSO());
            if (recipe != null) {
                progress += 1;
                OnCut?.Invoke(this, EventArgs.Empty);
                OnAnyCut?.Invoke(this, EventArgs.Empty);
                CallOnProgressUpdateEvent(recipe.GetMaxProgress());
                if (progress >= recipe.GetMaxProgress()) {
                    GetKitchenObject().DestroySelf();
                    KitchenObject.SpawnKitchenObject(recipe.GetOutput(), this);
                }
            }
        }
    }

    private void CallOnProgressUpdateEvent(int maxProgress) {
        OnProgressUpdate?.Invoke(this, new IHasProgress.OnProgressUpdateEventArgs {
            progress = ((float)progress) / maxProgress
        });
    }

}
