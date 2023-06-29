using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{

    private void Start() {
        MultiplayerManager.Instance.OnTryingToConnect += MultiplayerManager_OnTryingToConnect;
        MultiplayerManager.Instance.OnClientDisconnected += MultiplayerManager_OnClientDisconnected;

        Hide();
    }

    private void MultiplayerManager_OnClientDisconnected(object sender, System.EventArgs e) {
        Hide();
    }

    private void MultiplayerManager_OnTryingToConnect(object sender, System.EventArgs e) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        MultiplayerManager.Instance.OnTryingToConnect -= MultiplayerManager_OnTryingToConnect;
        MultiplayerManager.Instance.OnClientDisconnected -= MultiplayerManager_OnClientDisconnected;
    }

}
