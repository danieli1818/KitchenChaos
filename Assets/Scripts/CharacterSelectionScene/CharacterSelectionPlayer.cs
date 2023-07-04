using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionPlayer : MonoBehaviour
{

    [SerializeField] private int playerIndex;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private TextMeshPro readyText;
    [SerializeField] private Button kickPlayerButton;

    private void Start() {
        PlayerReadyStatusManager.Instance.OnPlayerReadyChanged += PlayerReadyStatusManager_OnPlayerReadyChanged;
        MultiplayerManager.Instance.OnPlayersDataListChanged += MultiplayerManager_OnPlayersDataListChanged;

        bool shouldAllowKickOption = NetworkManager.Singleton.IsServer && playerIndex != 0;
        if (shouldAllowKickOption) {
            kickPlayerButton.gameObject.SetActive(true);
            kickPlayerButton.onClick.AddListener(() => {
                Debug.Log("Yay detected clicking on the kick player button!");
                MultiplayerManager.Instance.KickPlayer(MultiplayerManager.Instance.GetClientIdFromPlayerIndex(playerIndex));
            });
        } else {
            kickPlayerButton.gameObject.SetActive(false);
        }

        UpdatePlayer();
    }

    private void PlayerReadyStatusManager_OnPlayerReadyChanged(object sender, PlayerReadyStatusManager.OnPlayerReadyChangedEventArgs e) {
        if (MultiplayerManager.Instance.GetPlayerIndexFromClientId(e.clientId) == playerIndex) {
            UpdatePlayer();
        }
    }

    private void MultiplayerManager_OnPlayersDataListChanged(object sender, System.EventArgs e) {
        Debug.Log("Updating player since players data list changed!");
        UpdatePlayer();
    }

    private void UpdatePlayer() {
        if (!MultiplayerManager.Instance.IsPlayerConnected(playerIndex)) {
            Hide();
        } else {
            Show();
            PlayerData playerData = MultiplayerManager.Instance.GetPlayerData(playerIndex);
            playerVisual.SetPlayerColor(MultiplayerManager.Instance.GetColorByIndex(playerData.colorIndex));
            readyText.gameObject.SetActive(PlayerReadyStatusManager.Instance.IsPlayerReady(playerData.clientId));
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        MultiplayerManager.Instance.OnPlayersDataListChanged -= MultiplayerManager_OnPlayersDataListChanged;
    }

}
