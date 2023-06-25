using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI recipesDeliveredNumberText;
    [SerializeField] private Button playAgainButton;

    private void Start() {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        playAgainButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
        });

        HideGameOverUI();
    }

    private void GameManager_OnStateChanged(object sender, GameManager.OnStateChangedEventArgs e) {
        if (e.newState == GameManager.State.GameOver) {
            recipesDeliveredNumberText.text = DeliveryManager.Instance.GetNumberOfSuccessfulRecipesDeliveries().ToString();
            ShowGameOverUI();
        } else {
            HideGameOverUI();
        }
    }

    private void ShowGameOverUI() {
        gameObject.SetActive(true);
    }

    private void HideGameOverUI() {
        gameObject.SetActive(false);
    }

}
