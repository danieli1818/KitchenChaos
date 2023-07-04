using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelectionErrorUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI errorMessageText;
    [SerializeField] private Button closeButton;

    private void Awake() {
        closeButton.onClick.AddListener(() => {
            Hide();
        });
        MultiplayerManager.Instance.OnPlayerTryingToSelectTheSameColor += MultiplayerManager_OnPlayerTryingToSelectTheSameColor;
        MultiplayerManager.Instance.OnPlayerTryingToSelectColorSelectedByAnotherPlayer += MultiplayerManager_OnPlayerTryingToSelectColorSelectedByAnotherPlayer;
        Hide();
    }

    private void MultiplayerManager_OnPlayerTryingToSelectColorSelectedByAnotherPlayer(object sender, System.EventArgs e) {
        errorMessageText.text = "Color already selected by another player, please choose another one";
        Show();
    }

    private void MultiplayerManager_OnPlayerTryingToSelectTheSameColor(object sender, System.EventArgs e) {
        errorMessageText.text = "Already selected this color";
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        MultiplayerManager.Instance.OnPlayerTryingToSelectTheSameColor -= MultiplayerManager_OnPlayerTryingToSelectTheSameColor;
        MultiplayerManager.Instance.OnPlayerTryingToSelectColorSelectedByAnotherPlayer -= MultiplayerManager_OnPlayerTryingToSelectColorSelectedByAnotherPlayer;
    }

}
