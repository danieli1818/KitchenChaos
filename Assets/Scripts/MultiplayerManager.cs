using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    
    public static MultiplayerManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Trying to create more than one instance of MultiplayerManager!");
            return;
        }
        Instance = this;
    }

    public void StartHost() {
        NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
        if (GameManager.Instance.IsWaitingToStart()) {
            response.Approved = true;
            response.CreatePlayerObject = true;
        } else {
            response.Approved = false;
        }
    }

    public void StartClient() {
        NetworkManager.Singleton.StartClient();
    }

    public void ShutdownAndDestroyNetworkManager() {
        Destroy(NetworkManager.Singleton.gameObject);
        NetworkManager.Singleton.Shutdown();
    }

}
