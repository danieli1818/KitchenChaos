using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{

    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;

    [SerializeField] private TMP_InputField joinByCodeInputField;
    [SerializeField] private Button joinByCodeButton;

    [SerializeField] private CreatingLobbyUI creatingLobbyUI;

    [SerializeField] private TMP_InputField playerNameInputField;

    private void Start() {
        createLobbyButton.onClick.AddListener(() => {
            creatingLobbyUI.Show();
        });
        quickJoinButton.onClick.AddListener(() => {
            LobbyManager.Instance.QuickJoinLobby();
        });
        joinByCodeButton.onClick.AddListener(() => {
            JoinByCode();
        });
        playerNameInputField.text = MultiplayerManager.Instance.GetLocalPlayerName();
        playerNameInputField.onValueChanged.AddListener((string playerName) => {
            MultiplayerManager.Instance.SetLocalPlayerName(playerName);
        });
    }

    private void JoinByCode() {
        string lobbyCode = joinByCodeInputField.text;
        LobbyManager.Instance.JoinLobbyByCode(lobbyCode);
    }

}
