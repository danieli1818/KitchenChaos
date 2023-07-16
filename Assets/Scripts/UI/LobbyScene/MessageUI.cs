using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour
{

    public event EventHandler OnCloseUI;

    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Start() {
        closeButton.onClick.AddListener(() => {
            MultiplayerManager.Instance.ShutdownAndDestroyNetworkManager();
            SceneLoader.LoadScene(SceneLoader.Scene.LobbyScene);
        });
        LobbyManager.Instance.OnTryingCreateLobby += LobbyManager_OnTryingCreateLobby;
        LobbyManager.Instance.OnFailedCreateLobby += LobbyManager_OnFailedCreateLobby;
        LobbyManager.Instance.OnTryingJoinLobby += LobbyManager_OnTryingJoinLobby;
        LobbyManager.Instance.OnFailedJoinLobby += LobbyManager_OnFailedJoinLobby;
        LobbyManager.Instance.OnTryingQuickJoinLobby += LobbyManager_OnTryingQuickJoinLobby;
        LobbyManager.Instance.OnFailedQuickJoinLobby += LobbyManager_OnFailedQuickJoinLobby;

        MultiplayerManager.Instance.OnTryingToConnect += MultiplayerManager_OnTryingToConnect;
        MultiplayerManager.Instance.OnStartingHost += MultiplayerManager_OnStartingHost;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

        Hide();
    }

    private void MultiplayerManager_OnStartingHost(object sender, System.EventArgs e) {
        SetMessage("Starting Game...");
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        if (clientId == NetworkManager.ServerClientId) {
            SetMessage("Host Disconnected!", true);
        } else {
            if (clientId == NetworkManager.Singleton.LocalClientId) {
                SetMessage("Disconnected!", true);
            }
        }
    }

    private void MultiplayerManager_OnTryingToConnect(object sender, System.EventArgs e) {
        SetMessage("Connecting...");
    }

    private void LobbyManager_OnFailedQuickJoinLobby(object sender, System.EventArgs e) {
        SetMessage("Failed To Find A Lobby!", true);
    }

    private void LobbyManager_OnTryingQuickJoinLobby(object sender, System.EventArgs e) {
        SetMessage("Searching And Quick Joining Lobby...");
    }

    private void LobbyManager_OnFailedJoinLobby(object sender, System.EventArgs e) {
        SetMessage("Failed To Join Lobby!", true);
    }

    private void LobbyManager_OnTryingJoinLobby(object sender, System.EventArgs e) {
        SetMessage("Joining Lobby...");
    }

    private void LobbyManager_OnFailedCreateLobby(object sender, System.EventArgs e) {
        SetMessage("Failed To Create Lobby!", true);
    }

    private void LobbyManager_OnTryingCreateLobby(object sender, System.EventArgs e) {
        SetMessage("Creating Lobby...");
    }

    private void SetMessage(string message, bool showCloseButton = false) {
        messageText.text = message;
        if (showCloseButton) {
            ShowCloseButton();
        } else {
            HideCloseButton();
        }
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);

        closeButton.Select();
    }

    private void Hide() {
        gameObject.SetActive(false);

        OnCloseUI?.Invoke(this, EventArgs.Empty);
    }

    private void ShowCloseButton() {
        closeButton.gameObject.SetActive(true);
    }

    private void HideCloseButton() {
        closeButton.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        if (LobbyManager.Instance != null) {
            LobbyManager.Instance.OnTryingCreateLobby -= LobbyManager_OnTryingCreateLobby;
            LobbyManager.Instance.OnFailedCreateLobby -= LobbyManager_OnFailedCreateLobby;
            LobbyManager.Instance.OnTryingJoinLobby -= LobbyManager_OnTryingJoinLobby;
            LobbyManager.Instance.OnFailedJoinLobby -= LobbyManager_OnFailedJoinLobby;
            LobbyManager.Instance.OnTryingQuickJoinLobby -= LobbyManager_OnTryingQuickJoinLobby;
            LobbyManager.Instance.OnFailedQuickJoinLobby -= LobbyManager_OnFailedQuickJoinLobby;
        }
        if (MultiplayerManager.Instance != null) {
            MultiplayerManager.Instance.OnTryingToConnect -= MultiplayerManager_OnTryingToConnect;
            MultiplayerManager.Instance.OnStartingHost -= MultiplayerManager_OnStartingHost;
        }
        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
        }
    }

}
