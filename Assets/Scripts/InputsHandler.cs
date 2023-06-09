using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputsHandler : MonoBehaviour
{

    public const string PLAYER_PREFS_BINDINGS = "InputBindings";

    public static InputsHandler Instance { get; private set; }

    public event EventHandler OnReboundBinding;

    public enum Binding {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Interact,
        InteractAlternate,
        Pause,
        GamepadInteract,
        GamepadInteractAlternate,
        GamepadPause
    }

    public event EventHandler OnInteract;
    public event EventHandler OnInteractAlternate;
    public event EventHandler OnPause;

    private InputsActions inputsActions;

    private void Awake() {
        Instance = this;

        inputsActions = new InputsActions();
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS)) {
            inputsActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }

        inputsActions.Player.Enable();

        inputsActions.Player.Interact.performed += OnInteractionPerformed;
        inputsActions.Player.InteractAlternate.performed += OnAlternateInteractionPerformed;
        inputsActions.Player.Pause.performed += OnPausePerformed;
    }

    private void OnDestroy() {
        inputsActions.Dispose();
    }

    private void OnPausePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPause?.Invoke(this, EventArgs.Empty);
    }

    private void OnAlternateInteractionPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAlternate?.Invoke(this, EventArgs.Empty);
    }

    private void OnInteractionPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteract?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetInputVectorNormalized() {

        Vector2 inputVector = inputsActions.Player.Move.ReadValue<Vector2>();

        return inputVector;
    }

    public string GetKeyBindingText(Binding binding) {
        return binding switch {
            Binding.MoveUp => inputsActions.Player.Move.bindings[1].ToDisplayString(),
            Binding.MoveDown => inputsActions.Player.Move.bindings[2].ToDisplayString(),
            Binding.MoveLeft => inputsActions.Player.Move.bindings[3].ToDisplayString(),
            Binding.MoveRight => inputsActions.Player.Move.bindings[4].ToDisplayString(),
            Binding.Interact => inputsActions.Player.Interact.bindings[0].ToDisplayString(),
            Binding.InteractAlternate => inputsActions.Player.InteractAlternate.bindings[0].ToDisplayString(),
            Binding.Pause => inputsActions.Player.Pause.bindings[0].ToDisplayString(),
            Binding.GamepadInteract => inputsActions.Player.Interact.bindings[1].ToDisplayString(),
            Binding.GamepadInteractAlternate => inputsActions.Player.InteractAlternate.bindings[1].ToDisplayString(),
            Binding.GamepadPause => inputsActions.Player.Pause.bindings[2].ToDisplayString(),
            _ => null,
        };
    }

    public void RebindBinding(Binding binding, Action<String> onRebindingCompletedAction) {
        inputsActions.Player.Disable();
        InputAction inputAction;
        int inputActionIndex;
        switch (binding) {
            case Binding.MoveUp:
                inputAction = inputsActions.Player.Move;
                inputActionIndex = 1;
                break;
            case Binding.MoveDown:
                inputAction = inputsActions.Player.Move;
                inputActionIndex = 2;
                break;
            case Binding.MoveLeft:
                inputAction = inputsActions.Player.Move;
                inputActionIndex = 3;
                break;
            case Binding.MoveRight:
                inputAction = inputsActions.Player.Move;
                inputActionIndex = 4;
                break;
            case Binding.Interact:
                inputAction = inputsActions.Player.Interact;
                inputActionIndex = 0;
                break;
            case Binding.InteractAlternate:
                inputAction = inputsActions.Player.InteractAlternate;
                inputActionIndex = 0;
                break;
            case Binding.Pause:
                inputAction = inputsActions.Player.Pause;
                inputActionIndex = 0;
                break;
            case Binding.GamepadInteract:
                inputAction = inputsActions.Player.Interact;
                inputActionIndex = 1;
                break;
            case Binding.GamepadInteractAlternate:
                inputAction = inputsActions.Player.InteractAlternate;
                inputActionIndex = 1;
                break;
            case Binding.GamepadPause:
                inputAction = inputsActions.Player.Pause;
                inputActionIndex = 2;
                break;
            default:
                Debug.LogError("Invalid binding: " + binding.ToString());
                return;
        }
        inputAction.PerformInteractiveRebinding(inputActionIndex).OnComplete(callback => {
            string newKey = callback.action.bindings[inputActionIndex].ToDisplayString();
            callback.Dispose();
            inputsActions.Player.Enable();
            PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, inputsActions.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();
            onRebindingCompletedAction(newKey);
        }).Start();
        OnReboundBinding?.Invoke(this, EventArgs.Empty);
    }

    public void ResetBindings() {
        inputsActions.Player.Disable();
        inputsActions.Player.Move.RemoveAllBindingOverrides();
        inputsActions.Player.Interact.RemoveAllBindingOverrides();
        inputsActions.Player.InteractAlternate.RemoveAllBindingOverrides();
        inputsActions.Player.Pause.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey(PLAYER_PREFS_BINDINGS);
        PlayerPrefs.Save();
        inputsActions.Player.Enable();
        OnReboundBinding?.Invoke(this, EventArgs.Empty);
    }

}
