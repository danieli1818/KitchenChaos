using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DisconnectedUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI disconnectReasonText;
    [SerializeField] private Button closeButton;

    private void Start() {
        MultiplayerManager.Instance.OnClientDisconnected += MultiplayerManager_OnConnectionFailed;
        closeButton.onClick.AddListener(() => {
            Hide();
        });

        Hide();
    }

    private void MultiplayerManager_OnConnectionFailed(object sender, System.EventArgs e) {
        string disconnectReason = NetworkManager.Singleton.DisconnectReason;
        if (disconnectReason == null || disconnectReason == "") {
            disconnectReason = "Failed to connect";
        }
        disconnectReasonText.text = disconnectReason;

        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        MultiplayerManager.Instance.OnClientDisconnected -= MultiplayerManager_OnConnectionFailed;
    }

}
