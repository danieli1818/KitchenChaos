using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelectionButton : MonoBehaviour
{

    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;

    private int colorIndex = 0;

    private void Awake() {
        GetComponent<Button>().onClick.AddListener(() => {
            MultiplayerManager.Instance.SetLocalPlayerColor(colorIndex);
        });
        MultiplayerManager.Instance.OnPlayersDataListChanged += MultiplayerManager_OnPlayersDataListChanged;
    }

    private void MultiplayerManager_OnPlayersDataListChanged(object sender, System.EventArgs e) {
        UpdateColorSelectionButton();
    }

    private void Start() {
        image.color = MultiplayerManager.Instance.GetColorByIndex(colorIndex);
        UpdateColorSelectionButton();
    }

    public void Select() {
        ShowSelected();
    }

    public void Deselect() {
        HideSelected();
    }

    public void SetColorIndex(int colorIndex) {
        this.colorIndex = colorIndex;
        image.color = MultiplayerManager.Instance.GetColorByIndex(colorIndex);
    }

    private void UpdateColorSelectionButton() {
        PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
        if (playerData.colorIndex == colorIndex) {
            ShowSelected();
        } else {
            HideSelected();
        }
    }

    private void ShowSelected() {
        selectedGameObject.SetActive(true);
    }

    private void HideSelected() {
        selectedGameObject.SetActive(false);
    }

    private void OnDestroy() {
        MultiplayerManager.Instance.OnPlayersDataListChanged -= MultiplayerManager_OnPlayersDataListChanged;
    }

}
