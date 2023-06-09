using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    
    public enum Scene {
        MainMenuScene,
        LoadingScene,
        GameScene
    }

    private static Scene loadingScene;

    public static void LoadScene(Scene scene) {
        loadingScene = scene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoaderCallback() {
        SceneManager.LoadScene(loadingScene.ToString());
    }

}
