using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{

    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;

    [SerializeField] private TMP_InputField joinByCodeInputField;
    [SerializeField] private Button joinByCodeButton;

    [SerializeField] private CreatingLobbyUI creatingLobbyUI;

    [SerializeField] private TMP_InputField playerNameInputField;

    [SerializeField] private MessageUI messageUI;

    private void Start() {
        creatingLobbyUI.OnCloseUI += CreatingLobbyUI_OnCloseUI;
        messageUI.OnCloseUI += MessageUI_OnCloseUI;
        mainMenuButton.onClick.AddListener(() => {
            MultiplayerManager.Instance.ShutdownAndDestroyNetworkManager();
            SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene);
        });
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

    private void MessageUI_OnCloseUI(object sender, System.EventArgs e) {
        createLobbyButton.Select();
    }

    private void CreatingLobbyUI_OnCloseUI(object sender, System.EventArgs e) {
        createLobbyButton.Select();
    }

    private void JoinByCode() {
        string lobbyCode = joinByCodeInputField.text;
        LobbyManager.Instance.JoinLobbyByCode(lobbyCode);
    }

}
