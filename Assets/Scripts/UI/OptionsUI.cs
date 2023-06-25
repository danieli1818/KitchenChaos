using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{

    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Slider soundEffectsVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TextMeshProUGUI soundEffectsVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private Button closeButton;

    // Key Binding
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAlternateButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button gamepadInteractButton;
    [SerializeField] private Button gamepadInteractAlternateButton;
    [SerializeField] private Button gamepadPauseButton;
    [SerializeField] private TextMeshProUGUI moveUpButtonText;
    [SerializeField] private TextMeshProUGUI moveDownButtonText;
    [SerializeField] private TextMeshProUGUI moveLeftButtonText;
    [SerializeField] private TextMeshProUGUI moveRightButtonText;
    [SerializeField] private TextMeshProUGUI interactButtonText;
    [SerializeField] private TextMeshProUGUI interactAlternateButtonText;
    [SerializeField] private TextMeshProUGUI pauseButtonText;
    [SerializeField] private TextMeshProUGUI gamepadInteractButtonText;
    [SerializeField] private TextMeshProUGUI gamepadInteractAlternateButtonText;
    [SerializeField] private TextMeshProUGUI gamepadPauseButtonText;

    [SerializeField] private Transform pressKeyToRebindScreenTransform;

    [SerializeField] private Button resetControlsButton;

    private Action onCloseAction;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        soundEffectsVolumeSlider.onValueChanged.AddListener((float sliderValue) => {
            SoundManager.Instance.ChangeVolume(sliderValue);
        });
        musicVolumeSlider.onValueChanged.AddListener((float sliderValue) => {
            MusicManager.Instance.ChangeVolume(sliderValue);
        });

        SoundManager.Instance.OnVolumeChanged += SoundManager_OnVolumeChanged;
        MusicManager.Instance.OnVolumeChanged += MusicManager_OnVolumeChanged;

        closeButton.onClick.AddListener(Hide);

        GameManager.Instance.OnLocalGameUnpaused += GameManager_OnGameUnpaused;

        moveUpButton.onClick.AddListener(() => {
            RebindBinding(InputsHandler.Binding.MoveUp, moveUpButtonText);
        });
        moveDownButton.onClick.AddListener(() => {
            RebindBinding(InputsHandler.Binding.MoveDown, moveDownButtonText);
        });
        moveLeftButton.onClick.AddListener(() => {
            RebindBinding(InputsHandler.Binding.MoveLeft, moveLeftButtonText);
        });
        moveRightButton.onClick.AddListener(() => {
            RebindBinding(InputsHandler.Binding.MoveRight, moveRightButtonText);
        });
        interactButton.onClick.AddListener(() => {
            RebindBinding(InputsHandler.Binding.Interact, interactButtonText);
        });
        interactAlternateButton.onClick.AddListener(() => {
            RebindBinding(InputsHandler.Binding.InteractAlternate, interactAlternateButtonText);
        });
        pauseButton.onClick.AddListener(() => {
            RebindBinding(InputsHandler.Binding.Pause, pauseButtonText);
        });
        gamepadInteractButton.onClick.AddListener(() => {
            RebindBinding(InputsHandler.Binding.GamepadInteract, gamepadInteractButtonText);
        });
        gamepadInteractAlternateButton.onClick.AddListener(() => {
            RebindBinding(InputsHandler.Binding.GamepadInteractAlternate, gamepadInteractAlternateButtonText);
        });
        gamepadPauseButton.onClick.AddListener(() => {
            RebindBinding(InputsHandler.Binding.GamepadPause, gamepadPauseButtonText);
        });

        resetControlsButton.onClick.AddListener(() => {
            InputsHandler.Instance.ResetBindings();
            UpdateVisual();
        });

        pressKeyToRebindScreenTransform.gameObject.SetActive(false);

        UpdateVisual();

        Hide();
    }

    private void UpdateVisual() {
        UpdateSoundEffectsVolume(SoundManager.Instance.GetVolume());
        UpdateMusicVolume(MusicManager.Instance.GetVolume());
        UpdateKeyBinding();
    }

    private void UpdateMusicVolume(float normalizedMusicVolume) {
        musicVolumeText.text = string.Format("Music Volume: {0}%", (int)(normalizedMusicVolume * 100));
        musicVolumeSlider.SetValueWithoutNotify(normalizedMusicVolume);
    }

    private void UpdateSoundEffectsVolume(float normalizedSoundEffectsVolume) {
        soundEffectsVolumeText.text = string.Format("Sound Effects Volume: {0}%", (int)(normalizedSoundEffectsVolume * 100));
        soundEffectsVolumeSlider.SetValueWithoutNotify(normalizedSoundEffectsVolume);
    }

    private void UpdateKeyBinding() {
        moveUpButtonText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.MoveUp);
        moveDownButtonText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.MoveDown);
        moveLeftButtonText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.MoveLeft);
        moveRightButtonText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.MoveRight);
        interactButtonText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.Interact);
        interactAlternateButtonText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.InteractAlternate);
        pauseButtonText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.Pause);
        gamepadInteractButtonText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.GamepadInteract);
        gamepadInteractAlternateButtonText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.GamepadInteractAlternate);
        gamepadPauseButtonText.text = InputsHandler.Instance.GetKeyBindingText(InputsHandler.Binding.GamepadPause);
    }

    private void GameManager_OnGameUnpaused(object sender, EventArgs e) {
        Hide();
    }

    private void MusicManager_OnVolumeChanged(object sender, MusicManager.OnVolumeChangedEventArgs e) {
        UpdateMusicVolume(e.newVolume);
    }

    private void SoundManager_OnVolumeChanged(object sender, SoundManager.OnVolumeChangedEventArgs e) {
        UpdateSoundEffectsVolume(e.newVolume);
    }

    public void Show(Action onCloseAction) {
        this.onCloseAction = onCloseAction;

        gameObject.SetActive(true);

        soundEffectsVolumeSlider.Select();
    }

    private void Hide() {
        gameObject.SetActive(false);

        onCloseAction?.Invoke();
    }

    private void RebindBinding(InputsHandler.Binding binding, TextMeshProUGUI bindingKeyText) {
        pressKeyToRebindScreenTransform.gameObject.SetActive(true);
        InputsHandler.Instance.RebindBinding(binding, newKey => {
            bindingKeyText.text = newKey;
            pressKeyToRebindScreenTransform.gameObject.SetActive(false);
        });
    }

}
