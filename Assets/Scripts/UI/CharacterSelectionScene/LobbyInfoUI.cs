using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyInfoUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    private void Start() {
        lobbyNameText.text = LobbyManager.Instance.GetLobby().Name;
        lobbyCodeText.text = LobbyManager.Instance.GetLobby().LobbyCode;
    }

}
