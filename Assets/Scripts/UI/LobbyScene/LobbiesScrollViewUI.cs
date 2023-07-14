using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesScrollViewUI : MonoBehaviour
{

    [SerializeField] private Transform contentTransform;
    [SerializeField] private Transform lobbyRowInfoTemplate;

    private void Start() {
        LobbyManager.Instance.OnLobbyListUpdated += LobbyManager_OnLobbyListUpdated;
    }

    private void LobbyManager_OnLobbyListUpdated(object sender, LobbyManager.OnLobbyListUpdatedEventArgs e) {
        ClearLobbyRows();
        foreach (Lobby lobby in e.lobbies) {
            CreateLobbyRowInfo(lobby);
        }
    }

    private void ClearLobbyRows() {
        foreach (LobbyRowInfoUI lobbyRowInfoUI in GetComponentsInChildren<LobbyRowInfoUI>()) {
            if (lobbyRowInfoUI.transform == lobbyRowInfoTemplate) {
                continue;
            }
            Destroy(lobbyRowInfoUI.gameObject);
        }
    }

    private void CreateLobbyRowInfo(Lobby lobby) {
        Transform lobbyRowInfoTransform = Instantiate(lobbyRowInfoTemplate, contentTransform);
        lobbyRowInfoTransform.GetComponent<LobbyRowInfoUI>().SetLobby(lobby);
        lobbyRowInfoTransform.gameObject.SetActive(true);
    }

    private void OnDestroy() {
        LobbyManager.Instance.OnLobbyListUpdated -= LobbyManager_OnLobbyListUpdated;
    }

}
