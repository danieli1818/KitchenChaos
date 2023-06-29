using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{

    // Keyboard
    [SerializeField] private TextMeshProUGUI moveLeftKeyText;
    [SerializeField] private TextMeshProUGUI moveUpKeyText;
    [SerializeField] private TextMeshProUGUI moveDownKeyText;
    [SerializeField] private TextMeshProUGUI moveRightKeyText;
    [SerializeField] private TextMeshProUGUI interactKeyText;
    [SerializeField] private TextMeshProUGUI interactAlternateKeyText;
    [SerializeField] private TextMeshProUGUI pauseKeyText;

    // Gamepad
    [SerializeField] private TextMeshProUGUI gamepadInteractKeyText;
    [SerializeField] private TextMeshProUGUI gamepadInteractAlternateKeyText;
    [SerializeField] private TextMeshProUGUI gamepadPauseKeyText;

    private void Start() {
        GameManager.Instance.OnLocalPlayerReadyChanged += GameManager_OnLocalPlayerReadyChanged;
        InputsHandler.Instance.OnReboundBinding += InputsHandler_OnReboundBinding;

        UpdateVisual();

        Show();
    }

    private void GameManager_OnLocalPlayerReadyChanged(object sender, GameManager.OnLocalPlayerReadyChangedEventArgs e) {
        Hide();
    }

    private void InputsHandler_OnReboundBinding(object sender, System.EventArgs e) {
        UpdateVisual();
    }

    private void UpdateVisual() {
        moveLeftKeyText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.MoveLeft);
        moveUpKeyText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.MoveUp);
        moveDownKeyText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.MoveDown);
        moveRightKeyText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.MoveRight);
        interactKeyText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.Interact);
        interactAlternateKeyText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.InteractAlternate);
        pauseKeyText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.Pause);

        gamepadInteractKeyText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.GamepadInteract);
        gamepadInteractAlternateKeyText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.GamepadInteractAlternate);
        gamepadPauseKeyText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.GamepadPause);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

}
