using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatingLobbyUI : MonoBehaviour
{

    public event EventHandler OnCloseUI;

    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button createLobbyButton;

    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private CheckboxButton isPrivateCheckboxButton;
    [SerializeField] private TMP_Dropdown maxPlayersDropdown;

    private void Start() {
        mainMenuButton.onClick.AddListener(() => {
            MultiplayerManager.Instance.ShutdownAndDestroyNetworkManager();
            SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene);
        });
        closeButton.onClick.AddListener(() => {
            Hide();
        });
        createLobbyButton.onClick.AddListener(() => {
            CreateLobby();
        });

        Hide();
    }

    private void CreateLobby() {
        string lobbyName = lobbyNameInputField.text;
        if (lobbyName == "") {
            lobbyName = "Lobby " + UnityEngine.Random.Range(1, 10000).ToString();
        }
        bool isPrivate = isPrivateCheckboxButton.IsChecked;
        if (!int.TryParse(maxPlayersDropdown.captionText.text, out int maxPlayers)) {
            Debug.LogError("Invalid max players option (Not an int), value: " + maxPlayersDropdown.captionText.text);
            return;
        }
        LobbyManager.Instance.CreateLobby(lobbyName, maxPlayers, isPrivate);
    }

    public void Show() {
        gameObject.SetActive(true);

        createLobbyButton.Select();
    }

    private void Hide() {
        gameObject.SetActive(false);
        OnCloseUI?.Invoke(this, EventArgs.Empty);
    }

}
