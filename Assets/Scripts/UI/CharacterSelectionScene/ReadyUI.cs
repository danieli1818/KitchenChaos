using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadyUI : MonoBehaviour
{

    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI readyButtonText;
    [SerializeField] private Color readyButtonColor;
    [SerializeField] private Color notReadyButtonColor;

    [SerializeField] private ColorSelectionErrorUI colorSelectionErrorUI;

    private void Start() {
        PlayerReadyStatusManager.Instance.OnLocalPlayerReadyChanged += PlayerReadyStatusManager_OnLocalPlayerReadyChanged;
        colorSelectionErrorUI.OnCloseUI += ColorSelectionErrorUI_OnCloseUI;
        mainMenuButton.onClick.AddListener(() => {
            MultiplayerManager.Instance.ShutdownAndDestroyNetworkManager();
            SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene);
        });
        readyButton.onClick.AddListener(() => {
            PlayerReadyStatusManager.Instance.TogglePlayerReadyState();
        });
        if (PlayerReadyStatusManager.Instance.IsLocalPlayerReady()) {
            SetReadyButton();
        } else {
            SetNotReadyButton();
        }
    }

    private void ColorSelectionErrorUI_OnCloseUI(object sender, System.EventArgs e) {
        throw new System.NotImplementedException();
    }

    private void PlayerReadyStatusManager_OnLocalPlayerReadyChanged(object sender, PlayerReadyStatusManager.OnLocalPlayerReadyChangedEventArgs e) {
        if (e.isReady) {
            SetReadyButton();
        } else {
            SetNotReadyButton();
        }
    }

    private void SetReadyButton() {
        readyButton.image.color = readyButtonColor;
        readyButtonText.text = "READY";
    }

    private void SetNotReadyButton() {
        readyButton.image.color = notReadyButtonColor;
        readyButtonText.text = "NOT READY";
    }

}
