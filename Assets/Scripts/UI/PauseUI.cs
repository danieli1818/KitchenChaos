using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button mainMenuButton;

    private void Start() {
        GameManager.Instance.OnLocalGamePaused += GameManager_OnLocalGamePaused;
        GameManager.Instance.OnLocalGameUnpaused += GameManager_OnLocalGameUnpaused;

        resumeButton.onClick.AddListener(() => {
            GameManager.Instance.TogglePause();
        });
        optionsButton.onClick.AddListener(() => {
            Hide();
            OptionsUI.Instance.Show(Show);
        });
        mainMenuButton.onClick.AddListener(() => {
            GameManager.Instance.TogglePause();
            SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene);
        });

        Hide();
    }

    private void GameManager_OnLocalGameUnpaused(object sender, System.EventArgs e) {
        Hide();
    }

    private void GameManager_OnLocalGamePaused(object sender, System.EventArgs e) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);

        resumeButton.Select();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

}
