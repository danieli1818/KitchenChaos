using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingMultiplayerUI : MonoBehaviour
{

    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Start() {
        hostButton.onClick.AddListener(() => {
            Debug.Log("HOST");
            MultiplayerManager.Instance.StartHost();
            Hide();
        });
        clientButton.onClick.AddListener(() => {
            Debug.Log("CLIENT");
            MultiplayerManager.Instance.StartClient();
            Hide();
        });
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

}
