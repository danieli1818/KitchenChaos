using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField] private Button playMultiplayerButton;
    [SerializeField] private Button playSingleplayerButton;
    [SerializeField] private Button quitButton;

    private void Start() {
        playMultiplayerButton.onClick.AddListener(() => {
            MultiplayerManager.IsMultiplayer = true;
            SceneLoader.LoadScene(SceneLoader.Scene.LobbyScene);
        });
        playSingleplayerButton.onClick.AddListener(() => {
            MultiplayerManager.IsMultiplayer = false;
            SceneLoader.LoadScene(SceneLoader.Scene.LobbyScene);
        });
        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
    }

}
