using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingGameUI : MonoBehaviour
{

    private void Start() {
        MultiplayerManager.Instance.OnStartingHost += MultiplayerManager_OnStartingHost;

        Hide();
    }

    private void MultiplayerManager_OnStartingHost(object sender, System.EventArgs e) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        MultiplayerManager.Instance.OnStartingHost -= MultiplayerManager_OnStartingHost;
    }

}
