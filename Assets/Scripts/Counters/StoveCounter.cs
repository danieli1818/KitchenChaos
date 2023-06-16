using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress, IHasWarningAlert
{

    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);
    [SerializeField] private TimerRecipesSO fryingRecipes;
    [SerializeField] private TimerRecipesSO burningRecipes;

    // Timer
    private NetworkVariable<float> stoveTimer = new NetworkVariable<float>(0f);

    private NetworkVariable<float> recipeTime = new NetworkVariable<float>(0f);

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
        isWarningOn = false;
    }

    private void Start() {
        ResetStoveTimer();
    }

    public override void OnNetworkSpawn() {
        state.OnValueChanged += State_OnValueChanged;
        stoveTimer.OnValueChanged += StoveTimer_OnValueChanged;
    }

    private void State_OnValueChanged(State prevState, State newState) {
        if (prevState == State.Burning && isWarningOn) {
            isWarningOn = false;
            OnWarningStateChanged?.Invoke(this, new IHasWarningAlert.OnWarningStateChangedEventArgs() {
                isWarningOn = isWarningOn
            });
        }
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs() {
            prevState = prevState,
            nextState = newState
        });
    }

    private void StoveTimer_OnValueChanged(float prevValue, float newValue) {
        float maxTime = recipeTime.Value != 0 ? recipeTime.Value : 1;
        OnProgressUpdate?.Invoke(this, new IHasProgress.OnProgressUpdateEventArgs() {
            progress = stoveTimer.Value / maxTime
        });
    }

    private void Update() {
        if (!IsServer) {
            return;
        }
        if (HasHeldKitchenObject()) {
            switch (state.Value) {
                case State.Frying:
                    UpdateStoveTimer();
                    if (stoveTimer.Value >= recipeTime.Value) {
                        ChangeKitchenObjectAccordingToRecipe(fryingRecipes.GetCounterRecipeSO(GetKitchenObject().GetKitchenObjectSO()));
                        KitchenObjectSO kitchenObjectSO = GetKitchenObject().GetKitchenObjectSO();
                        if (fryingRecipes.HasRecipe(kitchenObjectSO)) {
                            recipeTime.Value = fryingRecipes.GetCounterRecipeSO(kitchenObjectSO).GetRecipeTime();
                            // Keep it in Frying state and initialize the timer
                            ResetStoveTimer();
                        } else if (burningRecipes.HasRecipe(kitchenObjectSO)) {
                            recipeTime.Value = burningRecipes.GetCounterRecipeSO(kitchenObjectSO).GetRecipeTime();
                            SetState(State.Burning);
                        } else {
                            SetState(State.Idle);
                        }
                    }
                    break;
                case State.Burning:
                    UpdateStoveTimer();

                    // Handle warning
                    if (!isWarningOn && stoveTimer.Value >= (recipeTime.Value / 2.0)) {
                        SetWarningOnServerRpc();
                    }

                    // Handle state
                    if (stoveTimer.Value >= recipeTime.Value) {
                        ChangeKitchenObjectAccordingToRecipe(burningRecipes.GetCounterRecipeSO(GetKitchenObject().GetKitchenObjectSO()));
                        KitchenObjectSO kitchenObjectSO = GetKitchenObject().GetKitchenObjectSO();
                        if (burningRecipes.HasRecipe(kitchenObjectSO)) {
                            recipeTime.Value = burningRecipes.GetCounterRecipeSO(kitchenObjectSO).GetRecipeTime();
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

    [ServerRpc]
    private void SetWarningOnServerRpc() {
        SetWarningOnClientRpc();
    }

    [ClientRpc]
    private void SetWarningOnClientRpc() {
        isWarningOn = true;
        OnWarningStateChanged?.Invoke(this, new IHasWarningAlert.OnWarningStateChangedEventArgs() {
            isWarningOn = isWarningOn
        });
    }

    public override void Interact(PlayerController player) {
        if (player.HasHeldKitchenObject()) {
            if (!HasHeldKitchenObject()) {
                KitchenObject kitchenObject = player.GetKitchenObject();
                if (fryingRecipes.HasRecipe(kitchenObject.GetKitchenObjectSO())) {
                    kitchenObject.SetKitchenObjectHolder(this);
                    PlaceKitchenObjectServerRpc(KitchenObjectMultiplayerGameObject.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO()));
                }
            } else {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                        SetStateServerRpc(State.Idle);
                    }
                }
            }
        } else {
            if (HasHeldKitchenObject()) {
                GetKitchenObject().SetKitchenObjectHolder(player);
                // recipeTime = 0f;
                SetStateServerRpc(State.Idle);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlaceKitchenObjectServerRpc(int kitchenObjectSOIndex) {
        recipeTime.Value = fryingRecipes.GetCounterRecipeSO(KitchenObjectMultiplayerGameObject.Instance.GetKitchenObjectSO(kitchenObjectSOIndex)).GetRecipeTime();
        SetState(State.Frying);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateServerRpc(State state) {
        SetState(state);
    }

    // Server only
    private void SetState(State state) {
        this.state.Value = state;
        ResetStoveTimer();
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
        stoveTimer.Value += Time.deltaTime;
    }

    private void ResetStoveTimer() {
        stoveTimer.Value = 0f;
        OnProgressUpdate?.Invoke(this, new IHasProgress.OnProgressUpdateEventArgs() {
            progress = 0f
        });
    }

    public bool IsWarningAlertOn() {
        return isWarningOn;
    }

}
