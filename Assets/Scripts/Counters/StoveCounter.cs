using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress, IHasWarningAlert
{

    private State state;
    [SerializeField] private TimerRecipesSO fryingRecipes;
    [SerializeField] private TimerRecipesSO burningRecipes;

    // Timer
    private float stoveTimer;

    private float recipeTime;

    private bool isWarningOn;

    public event EventHandler<IHasProgress.OnProgressUpdateEventArgs> OnProgressUpdate;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public event EventHandler<IHasWarningAlert.OnWarningStateChangedEventArgs> OnWarningStateChanged;

    public class OnStateChangedEventArgs : EventArgs {
        public State prevState;
        public State nextState;
    }

    public enum State {
        Idle,
        Frying,
        Burning,
        Burned
    }

    private void Awake() {
        state = State.Idle;
        isWarningOn = false;
    }

    private void Start() {
        ResetStoveTimer();
    }

    private void Update() {
        if (HasHeldKitchenObject()) {
            switch (state) {
                case State.Frying:
                    UpdateStoveTimer();
                    if (stoveTimer >= recipeTime) {
                        ChangeKitchenObjectAccordingToRecipe(fryingRecipes.GetCounterRecipeSO(GetKitchenObject().GetKitchenObjectSO()));
                        KitchenObjectSO kitchenObjectSO = GetKitchenObject().GetKitchenObjectSO();
                        if (fryingRecipes.HasRecipe(kitchenObjectSO)) {
                            recipeTime = fryingRecipes.GetCounterRecipeSO(kitchenObjectSO).GetRecipeTime();
                            // Keep it in Frying state and initialize the timer
                            ResetStoveTimer();
                        } else if (burningRecipes.HasRecipe(kitchenObjectSO)) {
                            recipeTime = burningRecipes.GetCounterRecipeSO(kitchenObjectSO).GetRecipeTime();
                            SetState(State.Burning);
                        } else {
                            SetState(State.Idle);
                        }
                    }
                    break;
                case State.Burning:
                    UpdateStoveTimer();

                    // Handle warning
                    if (!isWarningOn && stoveTimer >= (recipeTime / 2.0)) {
                        isWarningOn = true;
                        OnWarningStateChanged?.Invoke(this, new IHasWarningAlert.OnWarningStateChangedEventArgs() {
                            isWarningOn = isWarningOn
                        });
                    }

                    // Handle state
                    if (stoveTimer >= recipeTime) {
                        ChangeKitchenObjectAccordingToRecipe(burningRecipes.GetCounterRecipeSO(GetKitchenObject().GetKitchenObjectSO()));
                        KitchenObjectSO kitchenObjectSO = GetKitchenObject().GetKitchenObjectSO();
                        if (burningRecipes.HasRecipe(kitchenObjectSO)) {
                            recipeTime = burningRecipes.GetCounterRecipeSO(kitchenObjectSO).GetRecipeTime();
                            // Keep it in Frying state and initialize the timer
                            ResetStoveTimer();
                        } else {
                            SetState(State.Burned);
                        }
                    }
                    break;
                default: // Idle or Burned
                    break;
            }
        }
    }

    public override void Interact(PlayerController player) {
        if (player.HasHeldKitchenObject()) {
            if (!HasHeldKitchenObject()) {
                KitchenObject kitchenObject = player.GetKitchenObject();
                if (fryingRecipes.HasRecipe(kitchenObject.GetKitchenObjectSO())) {
                    player.GetKitchenObject().SetKitchenObjectHolder(this);
                    recipeTime = fryingRecipes.GetCounterRecipeSO(kitchenObject.GetKitchenObjectSO()).GetRecipeTime();
                    SetState(State.Frying);
                }
            } else {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                        SetState(State.Idle);
                    }
                }
            }
        } else {
            if (HasHeldKitchenObject()) {
                GetKitchenObject().SetKitchenObjectHolder(player);
                recipeTime = 0f;
                SetState(State.Idle);
            }
        }
    }

    private void SetState(State state) {
        State prevState = this.state;
        this.state = state;
        ResetStoveTimer();
        if (prevState == State.Burning && isWarningOn) {
            isWarningOn = false;
            OnWarningStateChanged?.Invoke(this, new IHasWarningAlert.OnWarningStateChangedEventArgs() {
                isWarningOn = isWarningOn
            });
        }
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs() {
            prevState = prevState,
            nextState = state
        });
    }

    private bool ChangeKitchenObjectAccordingToRecipe(TimerRecipeSO recipe) {
        if (HasHeldKitchenObject()) {
            KitchenObject kitchenObject = GetKitchenObject();
            if (kitchenObject.GetKitchenObjectSO() == recipe.GetInput()) {
                kitchenObject.DestroySelf();
                KitchenObject.SpawnKitchenObject(recipe.GetOutput(), this);
                return true;
            }
        }
        return false;
    }

    private void UpdateStoveTimer() {
        stoveTimer += Time.deltaTime;
        OnProgressUpdate?.Invoke(this, new IHasProgress.OnProgressUpdateEventArgs() {
            progress = stoveTimer / recipeTime
        });
    }

    private void ResetStoveTimer() {
        stoveTimer = 0f;
        OnProgressUpdate?.Invoke(this, new IHasProgress.OnProgressUpdateEventArgs() {
            progress = 0f
        });
    }

    public bool isWarningAlertOn() {
        return isWarningOn;
    }

}
