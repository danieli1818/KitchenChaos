using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI recipesDeliveredNumberText;

    private void Start() {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

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
