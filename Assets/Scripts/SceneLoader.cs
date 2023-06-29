using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    
    public enum Scene {
        MainMenuScene,
        LoadingScene,
        LobbyScene,
        CharacterSelectionScene,
        GameScene
    }

    private static Scene loadingScene;

    public static void LoadScene(Scene scene) {
        loadingScene = scene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoadNetworkScene(Scene scene) {
        NetworkManager.Singleton.SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallback() {
        SceneManager.LoadScene(loadingScene.ToString());
    }

    public static bool IsCurrentScene(Scene scene) {
        return SceneManager.GetActiveScene().name == scene.ToString();
    }

    public static Scene GetCurrentScene() {
        return (Scene)Enum.Parse(typeof(Scene), SceneManager.GetActiveScene().name);
    }

}
