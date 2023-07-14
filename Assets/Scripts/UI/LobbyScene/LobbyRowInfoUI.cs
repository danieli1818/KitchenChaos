using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRowInfoUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playersInLobbyText;

    private Lobby lobby;

    private void Awake() {
        GetComponent<Button>().onClick.AddListener(() => {
            if (lobby != null) {
                LobbyManager.Instance.JoinLobbyById(lobby.Id);
            }
        });
    }

    public void SetLobby(Lobby lobby) {
        this.lobby = lobby;
        lobbyNameText.text = lobby.Name;
        playersInLobbyText.text = lobby.Players.Count.ToString() + "/" + lobby.MaxPlayers.ToString();
    }

}
