using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyInfoUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    private void Start() {
        Lobby lobby = LobbyManager.Instance.GetLobby();
        if (lobby != null) {
            lobbyNameText.text = LobbyManager.Instance.GetLobby().Name;
            lobbyCodeText.text = LobbyManager.Instance.GetLobby().LobbyCode;
        } else {
            lobbyNameText.text = "";
            lobbyCodeText.text = "";
        }
    }

}
